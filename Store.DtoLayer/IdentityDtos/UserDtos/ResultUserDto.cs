using System;

namespace Store.DtoLayer.IdentityDtos.UserDtos
{
    public class ResultUserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TCKimlik { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; } // Since IdentityUser doesn't have IsActive by default, we'll map LockoutEnabled or similar, or just leave it for now.
    }
}
