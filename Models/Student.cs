namespace AuthWebApp.Models
{
    public class Student
    {
        public long StudentId { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime JoinedDate { get; set; }
        public string? ProfilePicturePath { get; set; }
        public string? Gender { get; set; }

        // Relationship with ApplicationUser
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Relationship with StudentClass
        public int? StudentClassId { get; set; }
        public StudentClass StudentClass { get; set; }
    }
}
