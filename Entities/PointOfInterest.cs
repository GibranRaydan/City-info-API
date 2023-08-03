using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities;

public class PointOfInterest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "The name field is required")]
    [MaxLength(50)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string? Description {get; set;}
    [ForeignKey("CityId")]
    public City? city { get; set; }
    public int CityId { get; set; }
    public PointOfInterest(String name)
    {
        Name = name;
    }
}