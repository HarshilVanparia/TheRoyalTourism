using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace TheRoyalTourism.Models
{
    public class PackageModel
    {
        public int Pid { get; set; }

        [Required(ErrorMessage = "Package name is required")]
        public string Pname { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Plocation { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
        public decimal Pprice { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile Pimg { get; set; }

        [Required(ErrorMessage = "Day count is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Days must be at least 1")]
        public int Pday { get; set; }

        [Required(ErrorMessage = "Destination is required")]
        public int Did { get; set; }

        [Required(ErrorMessage = "Package type is required")]
        public string Package_Type { get; set; }
    }
}
