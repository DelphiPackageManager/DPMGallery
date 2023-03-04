using FluentMigrator.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace DPMGallery.DTO
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
