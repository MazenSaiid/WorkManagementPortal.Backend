using System.ComponentModel.DataAnnotations;

namespace WorkManagementPortal.Backend.API.Dtos.Account
{
    public class RegisterModel
    {
        public int EmployeeSerialNumber { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public string RoleName { get; set; } // Optional role name for assignment
        public string PhoneNumber { get; set; }
        public string? SupervisorId { get; set; }
        public string? TeamLeaderId { get; set; }
        public int? WorkShiftId { get; set; }
    }
}
