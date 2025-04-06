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



        public FoodsModel FoodsModel { get; set; }
        public TourModel TourModel { get; set; }
        public ItineraryModel ItineraryModel { get; set; }
        public FestivalModel FestivalModel { get; set; }
    }


    public class FoodsModel
    {
        public int Fid { get; set; }

        [Required]
        public string Fdetail { get; set; }

        [Required]
        public string Flocation { get; set; }

        public IFormFile Fimg { get; set; }

        [Required]
        public int Did { get; set; }

        public List<DestinationModel> DestinationList { get; set; }
    }

    public class TourModel
    {
        public int Tid { get; set; }

        [Required]
        public string Tname { get; set; }

        [Required]
        public int Tday { get; set; }

        [Required]
        public string Tpickup { get; set; }

        [Required]
        public string Toverview { get; set; }

        [Required]
        public string Thighlights { get; set; }

        public IFormFile Timg1 { get; set; }
        public IFormFile Timg2 { get; set; }
        public IFormFile Timg3 { get; set; }
        public IFormFile Timg4 { get; set; }

        [Required]
        public int Pid { get; set; }

        public List<PackageModel> PackageList { get; set; }
    }

    public class ItineraryModel
    {
        public int Iid { get; set; }

        [Required]
        public string Iname { get; set; }

        [Required]
        public int Tid { get; set; }

        public string Iday1 { get; set; }
        public string Iday2 { get; set; }
        public string Iday3 { get; set; }
        public string Iday4 { get; set; }
        public string Iday5 { get; set; }
        public string Iday6 { get; set; }
        public string Iday7 { get; set; }

        public List<TourModel> TourList { get; set; }
    }

    public class FestivalModel
    {
        public int Fpid { get; set; }

        [Required]
        public string Fpname { get; set; }

        [Required]
        public string Fplocation { get; set; }

        [Required]
        public decimal Fpprice { get; set; }

        [Required]
        public int Fpday { get; set; }

        public IFormFile Fpimg { get; set; }

        [Required]
        public int Did { get; set; }

        public List<DestinationModel> DestinationList { get; set; }
    }
}