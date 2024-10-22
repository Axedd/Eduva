using SchoolSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace SchoolSystem.Dtos
{
    public class StudentGeneralInfoDto
    {
        private string _firstName;
        private string _lastName;
        private string _profilePicturePath;


        public long StudentId { get; set; }
        public string FirstName
        {
            get => _firstName;
            set => _firstName = Capitalize(value);
        }

        public string LastName
        {
            get => _lastName;
            set => _lastName = Capitalize(value);
        }
        public string? ProfilePicturePath
        {
            get => string.IsNullOrWhiteSpace(_profilePicturePath) ? "/students/default.jpg" : _profilePicturePath;
            set => _profilePicturePath = value;
        }
        public string Gender { get; set; }
        public string UserId { get; set; }
        public StudentClassDto StudentClass {  get; set; }


        // Utility method to capitalize names
        private string Capitalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return char.ToUpper(value[0]) + value.Substring(1).ToLower();
        }
    }
}
