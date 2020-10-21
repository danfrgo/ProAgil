using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;
using ProAgil.Domain.Identity;

namespace ProAgil.Repository
{
    //vai criar automaticamente as tabelas respetivas à autenticaçao e autorizaçao...
    //... e inserindo tambem o user, role e o relacionamento entre elas
    public class ProAgilContext : IdentityDbContext<User, Role, int, 
                        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
                         IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ProAgilContext(DbContextOptions<ProAgilContext> options) : base (options){}
        
            public DbSet<Evento> Eventos { get; set; }
            public DbSet<Palestrante> Palestrantes { get; set; }
            public DbSet<PalestranteEvento> PalestranteEventos { get; set; }
            public DbSet<Lote> Lotes { get; set; }
            public DbSet<RedeSocial> RedeSociais { get; set; }
            
            //subscrever o metodo OnModelCreating, para especificar a relaçao de N para N evento com palestrantes
            //especificar as chaves eventoId e palestranteId
            protected override void OnModelCreating(ModelBuilder modelBuilder){

                base.OnModelCreating(modelBuilder);

                //relacionamento entre o User e o Role (o userRole tem uma chave => Haskey)
                modelBuilder.Entity<UserRole>(userRole => {
                    userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                    userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                     userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
                });

                modelBuilder.Entity<PalestranteEvento>()
                .HasKey(PE => new {PE.EventoId, PE.PalestranteId});
            }
    }
}