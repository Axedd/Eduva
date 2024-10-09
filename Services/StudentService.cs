using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;
using System.Security.Claims;

namespace SchoolSystem.Services
{
    public class StudentService : IStudentService
    {
        private ApplicationDbContext _context;
        private readonly IStudentClassService _studentClassService;
        private Random _random;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudentService(ApplicationDbContext context, Random random, IHttpContextAccessor httpContextAccessor, IStudentClassService studentClassService)
        {
            _context = context;
            _random = random;
            _httpContextAccessor = httpContextAccessor;
            _studentClassService = studentClassService;
        }    
        
        public async Task<List<Student>> GetAllStudents()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<List<Student>> GetAllRegisteredStudents()
        {
            return await _context.Students.Where(s => s.UserId != null).ToListAsync();
        }

        public async Task<int> GetClassIdOfStudentAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int studentClassId = await _studentClassService.GetStudentClassIdByUserId(userId);

            return studentClassId;
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

        public async Task<List<Student>> GetStudentsByClassAsync(int classId)
        {
            var students = await _context.Students
                .Where(s => s.StudentClassId == classId)
                .OrderBy(s => s.FirstName)
                .ToListAsync();
            
            return students;
        }


        public async Task UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
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
