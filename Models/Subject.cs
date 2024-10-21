using SchoolSystem.Models;

namespace SchoolSystem.Models
{
    public class Subject
    {
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }

        public ICollection<SubjectTeachers> SubjectTeachers { get; set; } = new List<SubjectTeachers>();
        public ICollection<StudentClassSubjects> StudentClassSubjects { get; set; } = new List<StudentClassSubjects>();
        public ICollection<Agenda> Agendas { get; set; }
       
    }

    public class SubjectDto
    {
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int StudentClassId { get; set; }
    }


    public class SubjectAgendaDto
    {
        public long SubjectId { get; set; }
        public string? SubjectName { get; set; }
    }
}
