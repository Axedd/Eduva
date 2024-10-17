namespace SchoolSystem.Models
{
    public class User
    {
        public class UserHeaderDto
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? ProfilePicturePath { get; set; }
            public StudentClassDto? StudentClass { get; set; } // Only for Students
        }
    }
}
