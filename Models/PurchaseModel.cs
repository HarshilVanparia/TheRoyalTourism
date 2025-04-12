namespace TheRoyalTourism.Models
{
    public class PurchaseModel
    {
        public string PackageName { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
