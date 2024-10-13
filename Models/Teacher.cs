using SchoolSystem.Models;
using System.ComponentModel.DataAnnotations;
using static SchoolSystem.Models.StudentClassSubjects;

namespace SchoolSystem.Models
{
    public class Teacher
    {
        public long TeacherId { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicturePath { get; set; }
        public string? UserId { get; set; }
        public DateTime JoinedDate { get; set; }
        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }

        public ICollection<SubjectTeachers> SubjectTeachers { get; set; } = new List<SubjectTeachers>();
        public ICollection<StudentClassSubjects> StudentClassSubjects { get; set; }
		public ICollection<Agenda> Agendas { get; set; }
	}

    public class TeacherDto
    {
        public long TeacherId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicturePath { get; set; }
        public ICollection<StudentClassSubjectsDto>? StudentClassSubjectsDto { get; set; }
        public ICollection<SubjectTeachersDto>? Subjects { get; set; }
    }

    public class SubjectTeacherDto
    {
        public long SubjectId { get; set; }
        public TeacherDto Teacher { get; set; }
    }

}
