using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;

namespace SchoolSystem.Services
{
    public class StudentClassService : IStudentClassService
    {
        private readonly ApplicationDbContext _context;
        public StudentClassService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentClass>> GetStudentClassesAsync()
        {
            return await _context.StudentClasses.OrderBy(sc => sc.ClassName).ToListAsync();
        }
    }
}
