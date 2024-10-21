namespace SchoolSystem.Dtos
{
    public class StudentClassSubjectDto
    {
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int StudentClassId { get; set; }
        public string TeacherName { get; set; }
        public long TeacherId { get; set; }
    }
}
