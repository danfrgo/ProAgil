using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.Domain;
using ProAgil.Repository;
using ProAgil.WebAPI.Dtos;

namespace ProAgil.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase //para trabalhar com http e outros detalhes
    {
        //Construtor
        public readonly IProAgilRepository _repo;
        //injeçao de dependencia => para injetar o repository (todos os metodos desenvolvidos)
        //no startup ja nao requisita o context mas requita sim a interface de repositorio
        public readonly IMapper _mapper;
        public EventoController(IProAgilRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        // GET api/values
        //retornar todos os eventos
        [HttpGet]
        //public ActionResult<IEnumerable<Evento>> Get()//"ActionResult" é padrao do MVC utilizando RAZOR e ja retorna uma View
        public async Task<IActionResult> Get()
        {
            try
            {
                var eventos = await _repo.GetAllEventoAsync(true);
                //IEnumerable porque quero retornar uma lista
                var results = _mapper.Map<EventoDto[]>(eventos);
                
                return Ok(results); //Status code 200 do Https
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Base de dados falhou {ex.Message}");
            } 
        }

        /* Gets
        ***********************************
        ***********************************
        ***********************************
        */

        // GET api/values/5
        //retornar evento por ID
        [HttpGet("{EventoId}")]
        //public ActionResult<IEnumerable<Evento>> Get()//"ActionResult" é padrao do MVC utilizando RAZOR e ja retorna uma View
        public async Task<IActionResult> Get(int EventoId)
        {
            try
            {
                var evento = await _repo.GetEventoAsyncById(EventoId, true);

                var results = _mapper.Map<EventoDto>(evento);

                return Ok(results); //Status code 200 do Https
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Base de dados falhou");
            }
        }

        // GET api/values/5
        //retornar evento por tema
        [HttpGet("getByTema/{tema}")]
        //public ActionResult<IEnumerable<Evento>> Get()//"ActionResult" é padrao do MVC utilizando RAZOR e ja retorna uma View
        public async Task<IActionResult> Get(string tema)
        {
            try
            {
                var eventos = await _repo.GetAllEventoAsyncByTema(tema, true);

                var results = _mapper.Map<EventoDto[]>(eventos);

                return Ok(results); //Status code 200 do Https
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Base de dados falhou");
            }
        }

        /* Post
        ***********************************
        ***********************************
        ***********************************
        */

        // Post api/values/5
        //criar evento
        [HttpPost]
        //public ActionResult<IEnumerable<Evento>> Get()//"ActionResult" é padrao do MVC utilizando RAZOR e ja retorna uma View
        public async Task<IActionResult> Post(EventoDto model)
        {
            try
            {
                var evento = _mapper.Map<Evento>(model);

                _repo.Add(evento);

                if (await _repo.SaveChangesAsync())
                {
                    //tou a chamar a rota [HttpGet("{EventoId}")], porque estou a..
                    //..utilizar o Created
                    return Created($"/api/evento/{model.Id}", _mapper.Map<EventoDto>(evento));
                }
            }

            catch (System.Exception ex) 
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, 
                $"Base de dados falhou {ex.Message}");
            }
            return BadRequest();
        }


        /* Put
            ***********************************
            ***********************************
            ***********************************
            */

        // Put api/values/5
        //atualizar evento
        [HttpPut("{EventoId}")]
        //public ActionResult<IEnumerable<Evento>> Get()//"ActionResult" é padrao do MVC utilizando RAZOR e ja retorna uma View
        public async Task<IActionResult> Put(int EventoId, EventoDto model)
        {
            try
            {
                var evento = await _repo.GetEventoAsyncById(EventoId, false);
                if (evento == null) return NotFound();
                

                _mapper.Map(model, evento);

                _repo.Update(model);

                //se tiver encontrado/atualizado
                if (await _repo.SaveChangesAsync())
                {
                    //tou a chamar a rota [HttpGet("{EventoId}")], porque estou a..
                    //..utilizar o Created
                    return Created($"/api/evento/{model.Id}", _mapper.Map<EventoDto>(evento));
                }
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Base de dados falhou {ex.Message}");
            }
            return BadRequest();
        }
        /* Delete
        ***********************************
        ***********************************
        ***********************************
        */

        // Delete api/values/5
        //apagar evento
        [HttpDelete("{EventoId}")]
        //public ActionResult<IEnumerable<Evento>> Get()//"ActionResult" é padrao do MVC utilizando RAZOR e ja retorna uma View
        public async Task<IActionResult> Delete(int EventoId)
        {
            try
            {
                var evento = await _repo.GetEventoAsyncById(EventoId, false);
                if (evento == null)
                { //se nao for encontrado nenhum evento, entao da um NotFound
                    return NotFound();
                }

                _repo.Delete(evento);

                //se tiver encontrado/atualizado
                if (await _repo.SaveChangesAsync())
                {
                    //tou a chamar a rota [HttpGet("{EventoId}")], porque estou a..
                    //..utilizar o Created
                    return Ok();
                }
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Base de dados falhou");
            }
            return BadRequest();
        }
    }
}

