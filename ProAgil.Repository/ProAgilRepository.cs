using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public class ProAgilRepository : IProAgilRepository
    {
        private readonly ProAgilContext _context;
        public ProAgilRepository(ProAgilContext context)
        {
            _context = context;
            //no tracking tambem pode ser feita de forma geral
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        //Gerais
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
             _context.Update(entity);
        }
        public void Delete<T>(T entity) where T : class
        {
             _context.Remove(entity);
        }
        public async Task<bool> SaveChangesAsync()
        {
             return (await _context.SaveChangesAsync()) > 0; // se for maior que zero, ou seja, gravou algum dado, entao retorna verdadeiro
        }

        //Evento
             public async Task<Evento[]> GetAllEventoAsync(bool includePalestrantes = false)
        {
            //vou criar uma query
            IQueryable<Evento> query = _context.Eventos
                .Include(c => c.Lotes)
                .Include(c => c.RedesSociais);

            if(includePalestrantes )//se for verdadeiro
            {
                query = query
                .Include(pe => pe.PalestrantesEventos)
                .ThenInclude(p => p.Palestrante);
            }
            //AsNoTracking() -> para noao travar o recurso para que ele seja retornado
            query = query.AsNoTracking()
            .OrderByDescending(c => c.DataEvento);

            return await query.ToArrayAsync();
        }
        
        public async Task<Evento[]> GetAllEventoAsyncByTema(string tema, bool includePalestrantes)
        {
           //vou criar uma query
            IQueryable<Evento> query = _context.Eventos
                .Include(c => c.Lotes)
                .Include(c => c.RedesSociais);

            if(includePalestrantes )//se for verdadeiro
            {
                query = query
                .Include(pe => pe.PalestrantesEventos)
                .ThenInclude(p => p.Palestrante);
            }
            //AsNoTracking() -> para noao travar o recurso para que ele seja retornado
            query = query.AsNoTracking()
                    .OrderByDescending(c => c.DataEvento)
                    .Where(c => c.Tema.ToLower().Contains(tema.ToLower()));
                   
            return await query.ToArrayAsync();
        }
        public async Task<Evento> GetEventoAsyncById(int EventoId, bool includePalestrantes)
        {
            //vou criar uma query
            IQueryable<Evento> query = _context.Eventos
                .Include(c => c.Lotes)
                .Include(c => c.RedesSociais);

            if(includePalestrantes )//se for verdadeiro
            {
                query = query
                .Include(pe => pe.PalestrantesEventos)
                .ThenInclude(p => p.Palestrante);
            }
            //AsNoTracking() -> para noao travar o recurso para que ele seja retornado
            query = query.AsNoTracking()
                    .OrderByDescending(c => c.DataEvento)
                    .Where(c => c.Id == EventoId);

            return await query.FirstOrDefaultAsync();
        }


        //Palestrante
        public async Task<Palestrante> GetPalestranteAsync(int PalestranteId, bool includeEventos = false)
        {
            //vou criar uma query
            IQueryable<Palestrante> query = _context.Palestrantes
                .Include(c => c.RedesSociais);

            if(includeEventos)//se for verdadeiro
            {
                query = query
                .Include(pe => pe.PalestrantesEventos)
                .ThenInclude(e => e.Evento);
            }

            // => é onde
            //AsNoTracking() -> para noao travar o recurso para que ele seja retornado
            query = query.AsNoTracking()
                        .OrderBy(p => p.Nome)
                        .Where(p => p.Id == PalestranteId);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Palestrante[]> GetAllPalestrantesAsyncByName(string name, bool includeEventos = false)
        {
            //vou criar uma query
            IQueryable<Palestrante> query = _context.Palestrantes
                .Include(c => c.RedesSociais);

            if(includeEventos)//se for verdadeiro
            {
                query = query
                .Include(pe => pe.PalestrantesEventos)
                .ThenInclude(e => e.Evento);
            }

            // => é onde
            //AsNoTracking() -> para noao travar o recurso para que ele seja retornado
            query = query.AsNoTracking()
                        .Where(p => p.Nome.ToLower().Contains(name.ToLower()));

            return await query.ToArrayAsync();
        }

    }
}