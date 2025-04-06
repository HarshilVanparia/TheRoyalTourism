using System.ComponentModel.DataAnnotations;

namespace TheRoyalTourism.Models
{
    public class PlaceModel
    {
        [Required]
        public string Pl_Detail { get; set; }

        [Required]
        public string Pl_Time { get; set; }

        [Required]
        public string Pl_Location { get; set; }

        [Required]
        public IFormFile Pl_Img { get; set; }

        [Required]
        public int Did { get; set; } // Foreign key
    }
}
