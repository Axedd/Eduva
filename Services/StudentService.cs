using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Data;
using SchoolSystem.Models;


namespace SchoolSystem.Services
{
	public class StudentService
	{
		private readonly SchoolSystemDbContext _context;

		public StudentService(SchoolSystemDbContext context)
		{
			_context = context;
		}

		public async Task<List<Student>> GetStudentsAsync()
		{
			return await _context.Students.Include(s => s.SchoolClass).ToListAsync();
		}

		public async Task<Student> GetStudentById(long studentId)
		{
			var student = await _context.Students
				.Include(s => s.SchoolClass)
				.FirstOrDefaultAsync(s => s.StudentId == studentId);

			if (student == null)
			{
				throw new Exception($"Student with ID {studentId} not found.");
			}

			return student;
		}

		public async Task<List<SchoolClass>> GetSchoolClassesAsync()
		{
			
			return await _context.SchoolClasses.ToListAsync();
		}

		public async Task<SchoolClass> GetSchoolClassByIdAsync(int id)
		{
			var schoolClass = await _context.SchoolClasses.FirstOrDefaultAsync(s => s.ClassId == id);
			if (schoolClass == null)
			{
				throw new SchoolClassNotFoundException(id);
			}
			return schoolClass;
		}

        public async Task<List<Student>> GetClassStudentsByIdAsync(int id)
        {
            var ClassStudents = await _context.Students.Where(s => s.ClassId == id).ToListAsync();
			
			if (!ClassStudents.Any())
			{
				throw new SchoolClassNotFoundException(id);
			}
            return ClassStudents;
        }


        public class SchoolClassNotFoundException : Exception
		{
			public SchoolClassNotFoundException(int id)
				: base($"SchoolClass with ID {id} not found.")
			{
			}
		}
	}
}
