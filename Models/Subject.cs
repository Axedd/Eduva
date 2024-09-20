using SchoolSystem.Models;

namespace AuthWebApp.Models
{
    public class Subject
    {
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }

		public ICollection<SubjectTeachers> SubjectTeachers { get; set; }
		public ICollection<StudentClassSubjects> StudentClassSubjects { get; set; }
    }
}
