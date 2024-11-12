using System.ComponentModel.DataAnnotations;

namespace WorkManagementPortal.Backend.API.Dtos.Account
{
    public class RegisterModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string RoleName { get; set; } // Optional role name for assignment
    }
}
