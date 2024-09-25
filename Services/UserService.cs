using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace SchoolSystem.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> GenerateUsername()
        {
            int currentYear = DateTime.Now.Year;
            int currentCount = await GetCurrentYearCountAsync(currentYear);
            int increment = currentCount + 1; // Start from the next number

            string username = $"{currentYear}G{increment}";

            // Check for uniqueness
            while (await UsernameExists(username))
            {
                increment++;
                username = $"{currentYear}G{increment}";
            }

            // Update the count for the current year
            await UpdateYearCountAsync(currentYear, increment);

            return username;
        }

        private async Task<bool> UsernameExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username);
        }

        private async Task<int> GetCurrentYearCountAsync(int year)
        {
            // Fetch the current count or return 0 if not found
            var yearCount = await _context.UsernameCounts.FindAsync(year);
            return yearCount?.Count ?? 0;
        }

        private async Task UpdateYearCountAsync(int year, int newCount)
        {
            var yearCount = await _context.UsernameCounts.FindAsync(year);
            if (yearCount == null)
            {
                // If the year doesn't exist, add a new entry
                yearCount = new UsernameCount { Year = year, Count = newCount };
                _context.UsernameCounts.Add(yearCount);
            }
            else
            {
                // Update the existing count
                yearCount.Count = newCount;
            }

            await _context.SaveChangesAsync(); // Persist changes to the database
        }

        public async Task<string> GenerateTempPassword(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            string tempPassword = GenerateRandomPassword();

            var passwordValidators = _userManager.PasswordValidators;
            foreach(var validator in passwordValidators)
            {
                var validationResult = await validator.ValidateAsync(_userManager, user, tempPassword);
                if (!validationResult.Succeeded)
                {
                    throw new Exception($"Password validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.Description))}");
                }
            }

            if (!await _userManager.HasPasswordAsync(user))
            {
                await _userManager.AddPasswordAsync(user, tempPassword);
            } else
            {
                throw new ArgumentException("A password is already active!");
            }
            


            return tempPassword;
        }

        public async Task<IdentityResult> AssignRoleToUserAsync(string userId, string role)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("User ID and role name must be provided.");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                throw new InvalidOperationException("Role not found.");
            }

            // Block assignment of the Admin role
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The Admin role cannot be assigned to users.");
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            
            return result;
        }

        public async Task<List<StudentRoleViewModel>> GetStudentsWithRolesAsync()
        {
            try
            {
                var students = await _context.Students
                    .Where(s => s.UserId != null)
                    .ToListAsync();

                var studentRoleViewModels = new List<StudentRoleViewModel>();

                foreach (var student in students)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == student.UserId);
                    var roleNames = await _userManager.GetRolesAsync(user);

                    var roles = roleNames.Select(roleName => new RoleViewModel
                    {
                        RoleId = roleName, // If you have a way to get the RoleId, update this accordingly
                        RoleName = roleName
                    }).ToList();

                    studentRoleViewModels.Add(new StudentRoleViewModel
                    {
                        StudentId = student.StudentId,
                        UserId = student.UserId,
                        UserName = user?.UserName,
                        HasPassword = user.PasswordHash != null,
                        Roles = roles
                    });
                }

                return studentRoleViewModels;
            }
            catch (Exception ex)
            {
                // Log the exception (or rethrow for debugging)
                Console.WriteLine(ex.Message);
                throw; // Optional: rethrow to see the stack trace
            }
        }

        public async Task<bool> isValidUserStudent(long studentId)
        {
            return await _context.Students.Where(t => t.StudentId == studentId).AnyAsync(s => s.UserId != null);
        }

        public async Task<bool> isValidUserTeacher(long teacherId)
        {

            return await _context.Teachers.Where(t => t.TeacherId == teacherId).AnyAsync(s => s.UserId != null);
        }

        private string GenerateRandomPassword()
        {
            // Generate a temporary password with specific criteria
            const int passwordLength = 8; // Set desired password length
            var random = new Random();

            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specialChars = "!@<>?";

            // Ensure at least one of each required character type
            var password = new StringBuilder();
            password.Append(lowercase[random.Next(lowercase.Length)]);
            password.Append(uppercase[random.Next(uppercase.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);

            // Fill the rest of the password length with random characters
            var allChars = lowercase + uppercase + digits + specialChars;
            for (int i = 4; i < passwordLength; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the password to ensure randomness
            var result = password.ToString().ToCharArray();
            return new string(result.OrderBy(x => random.Next()).ToArray());
        }


    }
}
