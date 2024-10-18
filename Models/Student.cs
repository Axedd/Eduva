using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolSystem.Models
{
    public class Student
    {
        // Backing fields for FirstName, LastName, and Gender
        private string _firstName;
        private string _lastName;
        private string _gender;
        private string _profilePicturePath;

        public long StudentId { get; set; }

        public string? Address { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName
        {
            get => _firstName;
            set => _firstName = Capitalize(value);
        }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName
        {
            get => _lastName;
            set => _lastName = Capitalize(value);
        }

        public DateTime JoinedDate { get; set; }

        public string? ProfilePicturePath
        {
            get => string.IsNullOrWhiteSpace(_profilePicturePath) ? "/students/default.jpg" : _profilePicturePath;
            set => _profilePicturePath = value;
        }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender
        {
            get => _gender;
            set
            {
                if (!IsValidGender(value))
                {
                    throw new ArgumentException("Gender must be Male, Female, or Other.");
                }
                _gender = value;
            }
        }

        // Relationship with ApplicationUser
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Relationship with StudentClass
        public int? StudentClassId { get; set; }
        public StudentClass? StudentClass { get; set; }

        // Utility method to capitalize names
        private string Capitalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return char.ToUpper(value[0]) + value.Substring(1).ToLower();
        }

        // Utility method to validate gender
        private bool IsValidGender(string value)
        {
            var allowedGenders = new[] { "Male", "Female", "Other" };
            return Array.Exists(allowedGenders, gender => gender.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        public class StudentDto
        {
            public long StudentId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string? ProfilePicturePath { get; set; }
        }

        public class StudentHeaderDto
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public StudentClassDto Class { get; set; }
        }
    }
}