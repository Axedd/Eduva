namespace AuthWebApp.Models
{
    public class StudentClassSubjects
    {
        public int StudentClassId { get; set; } // Foreign key for StudentClass
        public long SubjectId { get; set; } // Foreign key for Subject
        public long TeacherId { get; set; }

        // Navigation properties
        public StudentClass StudentClass { get; set; } // Navigation property for StudentClass
        public Subject Subject { get; set; } // Navigation property for Subject
        public Teacher Teacher { get; set; }

    }
}
