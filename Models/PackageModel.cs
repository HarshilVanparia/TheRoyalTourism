namespace TheRoyalTourism.Models
{
    public class PackageModel
    {
        public int Pid { get; set; }
        public string Pname { get; set; }
        public string Plocation { get; set; }
        public decimal Pprice { get; set; }
        public string Pimg { get; set; }
        public int Pday { get; set; }
        public string Dname { get; set; }
        public List<string> DestinationList { get; set; } = new List<string>();
    }
}
