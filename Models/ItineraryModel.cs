using System.ComponentModel.DataAnnotations;

namespace TheRoyalTourism.Models
{
    public class ItineraryModel
    {
        public int Iid { get; set; }

        public string Iday1 { get; set; }
        public string Iday2 { get; set; }
        public string Iday3 { get; set; }
        public string Iday4 { get; set; }
        public string Iday5 { get; set; }
        public string Iday6 { get; set; }
        public string Iday7 { get; set; }

        [Required]
        public int Tid { get; set; } // Foreign key to tourdetails
    }
}
