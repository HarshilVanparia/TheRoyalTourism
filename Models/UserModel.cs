using System.ComponentModel.DataAnnotations;

namespace TheRoyalTourism.Models
{
    public class UserModel
    {
        public int Uid { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string Pnumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string Role { get; set; } = "user";
    }
}
