using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProAgil.Domain.Identity;
using ProAgil.WebAPI.Dtos;


namespace ProAgil.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase //para trabalhar com http e outros detalhes
    {
        private readonly IConfiguration _config; // do extension configurantion
        private readonly UserManager<User> _userManager; // para controlar users
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public UserController(IConfiguration config,
                             UserManager<User> userManager, // para controlar users
                             SignInManager<User> signInManager,
                             IMapper mapper)
        {
            // os  parametros sao passados via injeçao de dependencia
            _signInManager = signInManager;
            _mapper = mapper;
            _config = config;
            _userManager = userManager;
        }

        [HttpGet("GetUser")]
          /* para utilizadores nao registados terem tambem acesso à plataforma 
        -> nao ha necessidade de efetuar um registo no sistema */
        // [AllowAnonymous]
        public async Task<IActionResult> GetUser()
        {
            return Ok(new UserDto());
        }


        [HttpPost("Register")]
        /* para utilizadores nao registados terem tambem acesso à plataforma 
        -> nao ha necessidade de efetuar um registo no sistema */
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                var user = _mapper.Map<User>(userDto); /* criar o mapeamento/match entre o user e o 
                userDto ( que é o user que recebemos) */

                var result = await _userManager.CreateAsync(user, userDto.Password); /* ele espera um identity user, 
                logo necessita do user e da pw  -> regista na base de dados*/

                var userToReturn = _mapper.Map<UserDto>(user); // mapear e retornar o user

                if (result.Succeeded)
                {
                    return Created("GetUser", userToReturn);
                }

                return BadRequest(result.Errors);
            }
            catch (System.Exception ex)
            {
                //StatusCode 500 para verificar o possivel erro
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco Dados Falhou {ex.Message}");
            }
        }

        [HttpPost("Login")]
          /* para utilizadores nao registados terem tambem acesso à plataforma 
        -> nao ha necessidade de efetuar um registo no sistema */
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            try
            {
                // certificar se o UserName existe na BD
                var user = await _userManager.FindByNameAsync(userLogin.UserName);

                // se encontrar o User nd BD, depois vai checkar a password do User
                // false para nao ativar o bloqueio de conta de por exemplo o user errar a pw 3 vezes
                var result = await _signInManager.CheckPasswordSignInAsync(user, userLogin.Password, false);

                if (result.Succeeded)
                {
                    var appUser = await _userManager.Users // retorna o User que foi encontrado na linha de cima
                        .FirstOrDefaultAsync(u => u.NormalizedUserName == userLogin.UserName.ToUpper());

                    var userToReturn = _mapper.Map<UserLoginDto>(appUser); // retorna o user

                    return Ok(new
                    {
                        // GenerateJWToken é o metodo para passar o token
                        token = GenerateJWToken(appUser).Result, // gera o token baseado no User que foi enontrado pelo userManager (FirstOrDefault retorna o primeiro User)
                        user = userToReturn
                    });
                }

                return Unauthorized(); // se o login falhar, retorna um nao autorizado
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco Dados Falhou {ex.Message}");
            }
        }


        //Criar o token
        private async Task<string> GenerateJWToken(User user)
        {
            // reinvidicar determinadas autorizaçoes/possibilidades -> autorizado nao é autenticaçao -> para obter autorizaçoes
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

            var roles = await _userManager.GetRolesAsync(user); // retornar os papeis/roles que o User possui

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // forma/ para criptograr/discriptografar
            var key = new SymmetricSecurityKey(Encoding.ASCII
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            // Algoritmo para criptografar
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // montar o token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), // data expirar
                SigningCredentials = creds 
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor); // Criar o token de acordo com o CreateToken

            return tokenHandler.WriteToken(token);
        }
    }

    
}