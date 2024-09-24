using Microsoft.AspNetCore.Identity;

namespace SchoolSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public DateTime JoinedDate { get; set; }
        public string? Email { get; set; } // Nullable Email

        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }

    }
}
