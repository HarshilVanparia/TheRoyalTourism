using System.ComponentModel.DataAnnotations;

namespace TheRoyalTourism.Models
{
    public class TourDisplayModel
    {
        public int Tid { get; set; }

        public string Tname { get; set; }

        public int Tday { get; set; }

        public string Tpickup { get; set; }

        public string Timg1 { get; set; }

        public string Timg2 { get; set; }

        public string Timg3 { get; set; }

        public string Timg4 { get; set; }

        public string Toverview { get; set; }

        public string Thighlights { get; set; }

        public int Pid { get; set; } // Foreign key to packages
    }
}
