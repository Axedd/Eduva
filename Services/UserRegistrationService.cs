using Microsoft.AspNetCore.Identity;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;

namespace SchoolSystem.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly ITeacherService _teacherService;

        public UserRegistrationService(
            UserManager<ApplicationUser> userManager, 
            IUserService userService, 
            IStudentService studentService, 
            ITeacherService teacherService)
        {
            _userManager = userManager;
            _userService = userService;
            _studentService = studentService;
            _teacherService = teacherService;
        }



        public async Task<IdentityResult> RegisterStudentAsync(ApplicationUser user, long studentId)
        {
            // Check if the student exists first
            var student = await _studentService.GetStudentById(studentId);
            if (student == null)
            {
                // Log or handle case where the student was not found
                throw new InvalidOperationException($"Student with ID {studentId} not found.");
            }
            else if (student.UserId != null)
            {
                // Student already has a UserId, return failure result
                var errors = new[] { new IdentityError { Description = $"Student with ID {studentId} is already registered." } };
                return IdentityResult.Failed(errors); 
            }

            // Generate a unique username
            string username = await _userService.GenerateUsername();

            ApplicationUser newUser = new ApplicationUser
            {
                UserName = username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                JoinedDate = DateTime.Now,
                Email = null 
            };

            // Create the user in the UserManager
            var result = await _userManager.CreateAsync(newUser);

            if (result.Succeeded)
            {
                // Link the new UserId to the Student
                student.UserId = newUser.Id;
                await _studentService.UpdateStudentAsync(student); // Call to update student details
                await _userManager.AddToRoleAsync(newUser, "Student");
            }

            return result; // Return the result of the user creation
        }

        public async Task<IdentityResult> RegisterTeacherAsync(ApplicationUser user, long teacherId)
        {
            // Check if the teacher exists first
            var teacher = await _teacherService.GetTeacherById(teacherId);
            if (teacher == null)
            {
                // Log or handle case where the teacher was not found
                throw new InvalidOperationException($"Teacher with ID {teacherId} not found.");
            }
            else if (teacher.UserId != null)
            {
                // Teacher already has a UserId, return failure result
                var errors = new[] { new IdentityError { Description = $"Teacher with ID {teacherId} is already registered." } };
                return IdentityResult.Failed(errors);
            }

            // Generate a unique username
            string username = await _userService.GenerateUsername();

            ApplicationUser newUser = new ApplicationUser
            {
                UserName = username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                JoinedDate = DateTime.Now,
                Email = null // or assign as needed
            };

            // Create the user in the UserManager
            var result = await _userManager.CreateAsync(newUser);

            if (result.Succeeded)
            {
                // Link the new UserId to the Teacher
                teacher.UserId = newUser.Id;
                await _teacherService.UpdateTeacherAsync(teacher); // Call to update teacher details

                // Add the new user to the Teacher role
                await _userManager.AddToRoleAsync(newUser, "Teacher");
            }

            return result; // Return the result of the user creation
        }


    }
}
