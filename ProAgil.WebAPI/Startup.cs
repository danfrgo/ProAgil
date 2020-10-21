using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProAgil.Domain.Identity;
using ProAgil.Repository;

namespace ProAgil.WebAPI
{
    public class Startup
    {
        //capacidade para ler os appsettings (desenvolvimento e produçao)
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ProAgilContext>(
                x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            // para controlar algumas configurações relativas às passwords
            IdentityBuilder builder = services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = false; // nao ha necessidade de caracteres especiais
            options.Password.RequireNonAlphanumeric = false; // nao ha necessidade de letras e numeros
            options.Password.RequireLowercase = false; // nao ha necessidade de letras lowercase
            options.Password.RequireUppercase = false; // nao ha necessidade de letras uppercase
            options.Password.RequiredLength = 4; // maximo caracteres a password
        });

        // builder.Services é o "IdentityBuilder builder = services.AddIdentityCore<User>(options =>"
        // configuraçoes de Context, roles e users
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<ProAgilContext>(); // para levar em consideraçao o Context
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>(); // gestor dos roles
            builder.AddSignInManager<SignInManager<User>>();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // validador do emissor que é True (é a propria API)
                            ValidateIssuerSigningKey = true, // emissor (para validar pela configuraçao da chave do emissor)
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                                .GetBytes(Configuration.GetSection("AppSettings:Token").Value)), // assinatura da chave do emissor
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    }
                );


                /* 
                Politica de autenticação

                para determinar qual o controller que vai ser chamada...
                criei uma politica/autorização para determinar
                ...sempre que eu chamar uma controller ela vai ter que respeitar automaticamente 
                as configuraçoes */
             services.AddMvc(options => {
                 // sempre que um user chamar uma rota, ele vai ter que estar autenticado
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build(); //build ao que acabamos de criar
                    options.Filters.Add(new AuthorizeFilter(policy)); // passar por parametro a politica
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling =
                 Newtonsoft.Json.ReferenceLoopHandling.Ignore); /* para controlar qualquer 
                 redundancia em relaçao ao retorno da serializaçao dos items */

            services.AddScoped<IProAgilRepository, ProAgilRepository>();
            services.AddAutoMapper();
            services.AddCors(); //requisiçao cruzada
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //pagina que serve para demonstrar erros (como se tratasse de um ecra amigavel)
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication(); // tem que ser autenticado

            //app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseStaticFiles(); //para encontrar imagens dentro do servidor, ficam dentro do wwwroot
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                RequestPath = new PathString("/Resources")
            });
            app.UseMvc();
        }
    }
}
