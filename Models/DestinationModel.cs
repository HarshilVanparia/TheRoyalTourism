using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace TheRoyalTourism.Models
{
    public class DestinationModel
    {
        public int Did { get; set; }

        [Required(ErrorMessage = "Destination name is required")]
        public string Dname { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile Dimg { get; set; }

        [Required(ErrorMessage = "Destination type is required")]
        public string Dtype { get; set; }
    }
}
