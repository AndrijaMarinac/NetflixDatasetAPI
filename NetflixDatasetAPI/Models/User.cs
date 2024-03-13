using NetflixDatasetAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace NetflixUserbaseDatasetAPI.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must have min 8 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not formated correctly")]
        public string Email { get; set; }
        
        public DateTime RegistrationDate { get; set; }
        
        public DateTime LastLoginDate { get; set; }

        public RefreshToken RefreshToken { get; set; } 
    }
}
