using Microsoft.EntityFrameworkCore;
using SchoolSystem.Data;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;

namespace SchoolSystem.Services
{
    public class ClassService : IClassService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClassService> _logger;
        public ClassService(ApplicationDbContext context, ILogger<ClassService> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<List<StudentClass>> GetAllClassesAsync()
        {
            try
            {
                var studentClasses = await _context.StudentClasses
                    .Include(sc => sc.Students)
                    .OrderBy(s => s.ClassName)
                    .ToListAsync();

                return studentClasses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving all student classes.");
                return new List<StudentClass>();  // Returning an empty list as a fallback
            }
        }
    }
}
