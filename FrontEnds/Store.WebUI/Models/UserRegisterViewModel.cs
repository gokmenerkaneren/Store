using System;

namespace Store.WebUI.Models
{
    public class UserRegisterViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TCKimlik { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public bool Terms { get; set; }
        public bool Marketing { get; set; }
    }
}
