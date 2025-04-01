using System.ComponentModel.DataAnnotations;

namespace TheRoyalTourism.Models
{
    public class User
    {
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(30, ErrorMessage = "Full Name cannot exceed 30 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone Number must be exactly 10 digits")]
        public string PNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Role { get; set; } = "user"; // Default role is 'user'
        public string Status { get; set; } = "Active"; // Default status is 'Active'
    }
}
