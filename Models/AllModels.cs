using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TheRoyalTourism.Models
{
    public class AllModels
    {
        public UserModel UserModel { get; set; } = new UserModel();
        public DestinationModel DestinationModel { get; set; } = new DestinationModel();
        public PackageModel PackageModel { get; set; } = new PackageModel();
        public List<DestinationModel> DestinationList { get; set; } = new List<DestinationModel>();
        public ActivityModel ActivityModel { get; set; }
        public PlaceModel PlaceModel { get; set; }
        public FoodModel FoodModel { get; set; }


        public TourModel TourModel { get; set; } = new TourModel();
        public List<PackageModel> PackageList { get; set; } = new List<PackageModel>();


        public ItineraryModel ItineraryModel { get; set; }
        public List<TourModel> TourList { get; set; } = new List<TourModel>();


       
    }
}