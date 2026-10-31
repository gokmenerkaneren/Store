namespace Store.IdentityServer.Dtos
{
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TCKimlik { get; set; }
        public string PhoneNumber { get; set; }       
    }
}