namespace CityInfo.API;
using CityInfo.API.Models;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }

    public static CitiesDataStore Current { get; } = new CitiesDataStore();

    public CitiesDataStore()
    {
        Cities = new List<CityDto>()
        {
            new CityDto()
            {
                Id =1,
                Name = "Caracas",
                Description = "Una mierda",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 5,
                        Name = "Petare",
                        Description = "Te van a robar y matar"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 5,
                        Name = "La guaira",
                        Description = "Una playa asquerosa"
                    }
                }
            },

            new CityDto()
            {
                Id =2,
                Name = "Bogata",
                Description = "Menos mierda",

                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 7,
                        Name = "Casa de Choco",
                        Description = "Pa partiles el culo en smash"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 6,
                        Name = "El Sur",
                        Description = "Donde te violan"
                    }
                }
            },
        };
    }
}