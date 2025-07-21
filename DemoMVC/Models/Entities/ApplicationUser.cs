using Microsoft.AspNetCore.Identity;

namespace DemoMVC.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        // Add additional properties for your application user here
        // For example, you can add a property for the user's full name
        [PersonalData]
        public string FullName { get; set; } = default!;

        // You can also add other properties as needed
        // public DateTime DateOfBirth { get; set; }
        // public string Address { get; set; }
    }
}