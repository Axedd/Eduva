using AuthWebApp.Data;
using AuthWebApp.Models;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;

namespace SchoolSystem.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<bool> isValidUserStudent(long studentId)
        {
            return await _context.Students.Where(t => t.StudentId == studentId).AnyAsync(s => s.UserId != null);
        }

        public async Task<bool> isValidUserTeacher(long teacherId)
        {

            return await _context.Teachers.Where(t => t.TeacherId == teacherId).AnyAsync(s => s.UserId != null);
        }


    }
}
