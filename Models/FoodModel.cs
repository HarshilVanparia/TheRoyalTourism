using System.ComponentModel.DataAnnotations;

namespace TheRoyalTourism.Models
{
    public class FoodModel
    {
        [Required(ErrorMessage = "Food detail is required.")]
        public string Fdetail { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Flocation { get; set; }

        [Required(ErrorMessage = "Image is required.")]
        public IFormFile Fimg { get; set; }

        [Required(ErrorMessage = "Destination is required.")]
        public int Did { get; set; }
    }
}
