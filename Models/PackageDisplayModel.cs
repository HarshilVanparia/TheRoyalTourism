using static ServiceStack.LicenseUtils;

namespace TheRoyalTourism.Models
{
    public class PackageDisplayModel
    {
        public int Pid { get; set; }
        public string Pname { get; set; }
        public string Plocation { get; set; }
        public decimal Pprice { get; set; }
        public string Pimg { get; set; }
        public int Pday { get; set; }
        public int Did { get; set; }
        public string Package_Type { get; set; }
    }
}
