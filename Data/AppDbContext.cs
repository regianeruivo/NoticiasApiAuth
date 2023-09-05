using Microsoft.EntityFrameworkCore;
using NoticiasApiAuth.Models;

namespace NoticiasApiAuth.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<Noticia> Noticias { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("DataSource=app.db;Cache=Shared");
    }
}
