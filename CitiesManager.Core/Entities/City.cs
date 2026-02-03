using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.Models
{
    public class City
    {
        [Key]
        public Guid CityId { get; set; }
        public string? CityName { get; set; }
    }
}
