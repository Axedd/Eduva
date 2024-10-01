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

        public async Task<StudentClass> GetStudentClassByIdAsync(long studentClassId)
        {
            return await _context.StudentClasses.Where(sc => sc.StudentClassId == studentClassId).FirstOrDefaultAsync();
        }

        public async Task<List<StudentClassSubjects>> GetStudentClassSubjectsAsync(long studentClassId)
        {
            return await _context.StudentClassSubjects
                .Include(scs => scs.Subject)
                .Include(scs => scs.Teacher)
                .Include(scs => scs.StudentClass)
                .Where(sc => sc.StudentClassId == studentClassId)
                .ToListAsync();
        }

        public async Task<string?> GetStudentClassName(long studentClassId)
        {
            return await _context.StudentClasses
				.Where(sc => sc.StudentClassId == studentClassId)
				.Select(sc => sc.ClassName)
				.FirstOrDefaultAsync();
		}

        
    }
}
