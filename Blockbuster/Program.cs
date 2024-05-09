using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace BlockbusterAPI
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SakilaDbContext>(options =>
                options.UseMySQL("Server=localhost;Database=sakila;Uid=username;Pwd=password;"));

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting(); 

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class SakilaDbContext : DbContext
    {
        public DbSet<Film> Films { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("your_connection_string_here");
        }
    }

    public class Film
    {
        public int FilmId { get; set; }
        public string Title { get; set; }
        // Add more properties as needed
    }

    public class FilmsController
    {
        private readonly SakilaDbContext _context;

        public FilmsController(SakilaDbContext context)
        {
            _context = context;
        }

        [HttpGet("/films")]
        public IEnumerable<Film> GetFilms()
        {
            return _context.Films.ToList();
        }

        [HttpGet("/films/{filmId}")]
        public ActionResult<Film> GetFilm(int filmId)
        {
            var film = _context.Films.Find(filmId);
            if (film == null)
            {
                return NotFound();
            }
            return film;
        }

        [HttpGet("/films/{filmId}/availability")]
        public IEnumerable<Store> GetFilmAvailability(int filmId)
        {
            var stores = _context.Stores
                .Where(s => s.Inventory.Any(i => i.FilmId == filmId))
                .ToList();
            return stores;
        }
    }

    public class Store
    {
        public int StoreId { get; set; }
        public string Address { get; set; }
        // Add more properties as needed
        public ICollection<Inventory> Inventory { get; set; }
    }

    public class Inventory
    {
        public int InventoryId { get; set; }
        public int FilmId { get; set; }
        public int StoreId { get; set; }
        // Add more properties as needed
    }
}
