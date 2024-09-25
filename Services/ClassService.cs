using Microsoft.EntityFrameworkCore;
using SchoolSystem.Data;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;

namespace SchoolSystem.Services
{
    public class ClassService : IClassService
    {
        private readonly ApplicationDbContext _context;
        public ClassService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<StudentClass>> GetAllClassesAsync()
        {
            var StudentClasses = await _context.StudentClasses
                .Include(sc => sc.Students)
                .OrderBy(s => s.ClassName)
                .ToListAsync();

            return StudentClasses;
        }
    }
}
