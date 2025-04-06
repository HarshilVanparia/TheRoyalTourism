using System.ComponentModel.DataAnnotations;

namespace TheRoyalTourism.Models
{
    public class TourModel
    {
        public int Tid { get; set; }

        [Required]
        public string Tname { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Tday { get; set; }

        [Required]
        public string Tpickup { get; set; }

        [Required]
        public IFormFile Timg1 { get; set; }

        [Required]
        public IFormFile Timg2 { get; set; }

        [Required]
        public IFormFile Timg3 { get; set; }

        [Required]
        public IFormFile Timg4 { get; set; }

        [Required]
        public string Toverview { get; set; }

        [Required]
        public string Thighlights { get; set; }

        [Required]
        public int Pid { get; set; } // Foreign key to packages
    }
}
