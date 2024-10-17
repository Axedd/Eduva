namespace SchoolSystem.Models
{
    public class SubNavViewModel
    {
        public string CurrentPage { get; set; }
        public string UserRole { get; set; }
        public string FullName { get; set; }
        public string SelectedStudentFullName { get; set; } // New property for selected student's name
    }
}
