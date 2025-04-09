namespace TheRoyalTourism.Models
{
    public class AdminDataTablesViewModel
    {
        public List<UserModel> Users { get; set; }
        public List<ItineraryModel> Itineraries { get; set; }

        // Replace original models with Display ones:
        public List<ActivityDisplayModel> Activities { get; set; }
        public List<FoodDisplayModel> Foods { get; set; }

        public List<DestinationDisplayModel> Destinations { get; set; }
    }
}
    