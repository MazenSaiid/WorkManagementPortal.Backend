namespace WorkManagementPortal.Backend.API.Dtos.User
{
    public class RegisterModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; } // Optional role name for assignment
    }
}
