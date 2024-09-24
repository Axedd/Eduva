namespace SchoolSystem.Models
{
    public class StudentClass
    {
        public int StudentClassId { get; set; }
        public string? ClassName { get; set; }

        // Navigation property
        public ICollection<Student> Students { get; set; }
        public ICollection<StudentClassSubjects> StudentClassSubjects { get; set; }
    }
}
