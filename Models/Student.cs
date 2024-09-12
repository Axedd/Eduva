using System.Diagnostics;

namespace SchoolSystem.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long StudentId { get; set; }
        public string ProfilePicturePath { get; set; }
        public string Gender { get; set; }

        // Foreign key property
        public int ClassId { get; set; }

        // Navigation property
        public SchoolClass SchoolClass { get; set; }
		public string ImageFileName => $"/students/{FirstName[0]}/{Gender}/{FirstName[0]}_{StudentId}.jpg"; // Adjust directory and extension as needed
	}
}
