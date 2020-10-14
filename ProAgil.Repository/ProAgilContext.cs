using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public class ProAgilContext : DbContext
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
                modelBuilder.Entity<PalestranteEvento>()
                .HasKey(PE => new {PE.EventoId, PE.PalestranteId});
            }
    }
}