using System.ComponentModel.DataAnnotations;

namespace AuthWebApp.Models
{
    public class Student
    {
        public long StudentId { get; set; }
        public string? Address { get; set; }
        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }
        public DateTime JoinedDate { get; set; }
        public string? ProfilePicturePath { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        // Relationship with ApplicationUser
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Relationship with StudentClass
        public int? StudentClassId { get; set; }
        public StudentClass? StudentClass { get; set; }
    }
}
