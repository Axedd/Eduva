namespace AuthWebApp.Models
{
    public class Teacher
    {
        public long TeacherId { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<StudentClassSubjects> StudentClassSubjects { get; set; }
    }
}
