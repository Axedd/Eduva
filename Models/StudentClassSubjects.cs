namespace SchoolSystem.Models
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

        public class StudentClassSubjectsDto
        {
            public int StudentClassId { get; set; } // Foreign key for StudentClass
            public long SubjectId { get; set; } // Foreign key for Subject
            public long TeacherId { get; set; }
            public SubjectDto? Subject { get; set; }
            public StudentClassDto? StudentClassDto { get; set; }

        }

        public class StudentClassSubjectDto
        {
            public SubjectDto? SubjectDto { get; set; }
        }

    }
}
