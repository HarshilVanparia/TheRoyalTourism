using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TheRoyalTourism.Models
{
    public class Destination
    {

        [Required(ErrorMessage = "Destination name is required.")]
        public string DName { get; set; }

        [Required(ErrorMessage = "Please upload a thumbnail image.")]
        public IFormFile DImg { get; set; }

        [Required(ErrorMessage = "Please select a destination type.")]
        public string DType { get; set; }
    }
}