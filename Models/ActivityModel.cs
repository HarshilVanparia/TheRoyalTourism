using System.ComponentModel.DataAnnotations;

namespace TheRoyalTourism.Models
{
    public class ActivityModel
    {
        public string Adetail { get; set; }
        public string Atime { get; set; }
        public string Alocation { get; set; }
        public string Aactivity { get; set; }

        [Required(ErrorMessage = "Please upload an image")]
        public IFormFile Aimg { get; set; }

        [Required(ErrorMessage = "Destination is required")]
        public int Did { get; set; }

    }
}
