using AuthWebApp.Data;
using AuthWebApp.Models;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;

namespace SchoolSystem.Services
{
    public class StudentService : IStudentService
    {
        private ApplicationDbContext _context;
        private Random _random;

        public StudentService(ApplicationDbContext context, Random random)
        {
            _context = context;
            _random = random;
        }                                                   

        public async Task<Student> GetStudentById(long studentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {studentId} not found.");
            }

            return student;
        }

        public async Task<long> GenerateStudentId()
        {
            var existingStudentIds = await _context.Students.Select(s => s.StudentId).ToListAsync();
            long generatedStudenttId;
            do
            {
                generatedStudenttId = _random.NextInt64(10000000000, 99999999999);

            } while (existingStudentIds.Contains(generatedStudenttId));

            return generatedStudenttId;
        }
    }
}
