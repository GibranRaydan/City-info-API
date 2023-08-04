using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts;
public class CityInfoContext : DbContext
{
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<PointOfInterest> PointOfInterest { get; set; } = null!;
    public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
    {

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>()
            .HasData(
                new City("Caracas")
                {
                    Id = 1,
                    Description = "Una ciudad asquerosa"
                },
                new City("Bogotá")
                {
                    Id = 2,
                    Description = "Está muy alta y hay montañas"
                },
                new City("Porlamar")
                {
                    Id = 3,
                    Description = "Hace calor"
                }
            );


        modelBuilder.Entity<PointOfInterest>()
            .HasData(
                new PointOfInterest("Petare")
                {
                    Id = 1,
                    Description = "Te van a robar",
                    CityId = 1
                },

                new PointOfInterest("El Centro ")
                {
                    Id = 2,
                    Description = "Es muy feo",
                    CityId = 1

                },
                new PointOfInterest("El Sur")
                {
                    Id = 3,
                    Description = "Te roban pero en colombiano",
                    CityId = 2
                },
                new PointOfInterest("La Caracola")
                {
                    Id = 4,
                    Description = "Playa para correr",
                    CityId = 3
                },
                new PointOfInterest("Costa Azul")
                {
                    Id = 5,
                    Description = "Residencia",
                    CityId = 3
                }
            );
        base.OnModelCreating(modelBuilder);
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSqlite("connectionstring");
    //     base.OnConfiguring(optionsBuilder);
    // }
}