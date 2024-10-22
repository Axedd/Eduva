using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;
using System.Security.Claims;
using SchoolSystem.Dtos;

namespace SchoolSystem.Services
{
    public class StudentService : IStudentService
    {
        private ApplicationDbContext _context;
        private readonly IStudentClassService _studentClassService;
        private Random _random;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdValidationService _idValidationService;
        private readonly ILogger<StudentService> _logger;

        public StudentService(ApplicationDbContext context,
            Random random,
            IHttpContextAccessor httpContextAccessor,
            IStudentClassService studentClassService,
            IIdValidationService idValidationService,
            ILogger<StudentService> logger)
        {
            _context = context;
            _random = random;
            _httpContextAccessor = httpContextAccessor;
            _studentClassService = studentClassService;
            _idValidationService = idValidationService;
            _logger = logger;
        }

        #region Student Retrieval Methods

        public async Task<List<Student>> GetAllStudents()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<List<Student>> GetAllRegisteredStudents()
        {
            return await _context.Students.Where(s => s.UserId != null).ToListAsync();
        }

        public async Task<Student> GetStudentById(long studentId)
        {
            var student = await _context.Students
                .Include(s => s.StudentClass)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {studentId} not found.");
            }

            return student;
        }

        public async Task<StudentGeneralInfoDto> GetStudentGeneralInfoAsync(long studentId)
        {
            var student = await _context.Students
                .Where(s => s.StudentId == studentId)
                .Include(s => s.StudentClass)
                .Select(s => new StudentGeneralInfoDto
                {
                    StudentId = s.StudentId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    ProfilePicturePath = s.ProfilePicturePath,
                    Gender = s.Gender,
                    UserId = s.UserId ?? "",
                    StudentClass = new StudentClassDto() {
                        StudentClassId = s.StudentClass.StudentClassId,
                        ClassName = s.StudentClass.ClassName
                    }
                })
                .FirstOrDefaultAsync();

            return student;
        }

        public async Task<List<StudentGeneralInfoDto>> GetStudentsByClassAsync(int classId)
        {
            var students = await _context.Students
                .Where(s => s.StudentClassId == classId)
                .Include(s => s.StudentClass)
                .Select(s => new StudentGeneralInfoDto
                {
                    StudentId = s.StudentId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    ProfilePicturePath = s.ProfilePicturePath,
                    Gender = s.Gender,
                    UserId = s.UserId ?? "",
                    StudentClass = new StudentClassDto()
                    {
                        StudentClassId = s.StudentClass.StudentClassId,
                        ClassName = s.StudentClass.ClassName
                    }
                })
                .ToListAsync();

            return students;
        }

        #endregion

        #region Class and Student ID Methods

        public async Task<int> GetClassIdOfStudentAsync(string? userId = null, long? studentId = null)
        {
            // Check for valid input
            if (studentId.HasValue && studentId.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(studentId), studentId.Value, "Invalid student ID.");
            }

            // If studentId is provided, try to retrieve the StudentClassId using studentId
            if (studentId.HasValue && await _idValidationService.IsValidStudentIdAsync(studentId.Value))
            {
                return await _studentClassService.GetStudentClassIdAsync(studentId: studentId.Value);
            }
            // If userId is provided and studentId is not valid or not provided, try to retrieve the StudentClassId using userId
            else if (!string.IsNullOrWhiteSpace(userId))
            {
                return await _studentClassService.GetStudentClassIdAsync(userId: userId);
            }

            return 0; // Return 0 if neither studentId nor userId is valid
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

        #endregion

        #region Student Management Methods

        public async Task RegisterStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudentAsync(Student student)
        {
            var existingStudent = await GetStudentById(student.StudentId);

            if (existingStudent == null)
            {
                _logger.LogWarning("Student not found in the database: {StudentId}", student.StudentId);
            }

            existingStudent.FirstName = student.FirstName;
            existingStudent.LastName = student.LastName;

            if (student.StudentClassId != null)
            {
                if (!await _idValidationService.IsValidStudentClassIdAsync((int)student.StudentClassId))
                {
                    _logger.LogWarning("Invalid Student Class ID: {StudentClassId}", student.StudentClassId);
                }

                existingStudent.StudentClassId = student.StudentClassId;
            }

            _context.Students.Update(existingStudent);
            await _context.SaveChangesAsync();
        }

        public async Task AddStudentAsync(Student newStudent)
        {
            newStudent.StudentId = await GenerateStudentId();
            newStudent.JoinedDate = DateTime.Now;
            newStudent.ProfilePicturePath = "/students/default.jpg"; // Assign default profile picture for development

            _context.Add(newStudent);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}