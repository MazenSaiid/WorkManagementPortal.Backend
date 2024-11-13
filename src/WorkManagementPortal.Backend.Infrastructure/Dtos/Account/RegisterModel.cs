using System.ComponentModel.DataAnnotations;

namespace WorkManagementPortal.Backend.API.Dtos.Account
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string RoleName { get; set; } // Optional role name for assignment
        public string? SupervisorId { get; set; }
        public string? TeamLeaderId { get; set; }
    }
}
