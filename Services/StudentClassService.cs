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


        public async Task<int> GetStudentClassIdByUserId(string userId)
        {
            return await _context.Students
                .Where(s => s.UserId == userId)
                .Select(a => a.StudentClassId ?? 0) // Provide a default value of 0 if StudentClassId is null
                .FirstOrDefaultAsync();
        }

        public async Task<List<StudentClass>> GetStudentClassesAsync()
        {
            return await _context.StudentClasses.OrderBy(sc => sc.ClassName).ToListAsync();
        }

        public async Task<StudentClass> GetStudentClassByIdAsync(long studentClassId)
        {
            return await _context.StudentClasses.Where(sc => sc.StudentClassId == studentClassId).FirstOrDefaultAsync();
        }

        public async Task<List<StudentDto>> GetAllStudentsFromClassIdAsync(long studentClassId)
        {
            return await _context.StudentClasses
                .Where(sc => sc.StudentClassId == studentClassId)
                .Include(sc => sc.Students)
                .SelectMany(sc => sc.Students)
                .Select(st => new StudentDto
                {
                    StudentId = st.StudentId,
                    FirstName = st.FirstName,
                    LastName = st.LastName,
                    ProfilePicturePath = st.ProfilePicturePath,
                
                })
                .OrderBy(s => s.FirstName)
                .ToListAsync();
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
