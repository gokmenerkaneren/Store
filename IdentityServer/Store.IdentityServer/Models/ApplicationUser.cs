using Microsoft.AspNetCore.Identity;
using System;
namespace Store.IdentityServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string TCKimlik { get; set; }
    }
}
