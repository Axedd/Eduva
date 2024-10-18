namespace SchoolSystem.Models
{
    public class User
    {
        public class UserHeaderDto
        {
            private string _profilePicturePath;

            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? ProfilePicturePath
            {
                get => string.IsNullOrWhiteSpace(_profilePicturePath) ? "/students/default.jpg" : _profilePicturePath;
                set => _profilePicturePath = value;
            }
            public StudentClassDto? StudentClass { get; set; } // Only for Students
        }
    }
}
