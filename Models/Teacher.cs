using SchoolSystem.Models;

namespace AuthWebApp.Models
{
    public class Teacher
    {
        public long TeacherId { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicturePath { get; set; }

        public ICollection<SubjectTeachers> SubjectTeachers { get; set; } = new List<SubjectTeachers>();
        public ICollection<StudentClassSubjects> StudentClassSubjects { get; set; }
    }

    public class TeacherDto
    {
        public long TeacherId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicturePath { get; set; }
    }

    public class SubjectTeacherDto
    {
        public long SubjectId { get; set; }
        public TeacherDto Teacher { get; set; }
    }
}
