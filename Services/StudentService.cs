using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Data;
using SchoolSystem.Models;


namespace SchoolSystem.Services
{
	public class StudentService
	{
		private readonly SchoolSystemDbContext _context;
		private readonly Random _random;

		public StudentService(SchoolSystemDbContext context, Random random)
		{
			_context = context;
			_random = random;
		}

		public async Task AddNewStudentAsync(Student student)
		{
			try
			{
				// Add the student to the context
				await _context.Students.AddAsync(student);

				// Save changes to the database
				int rowsAffected = await _context.SaveChangesAsync();

				// Log success message
				Console.WriteLine($"Successfully added student. Rows affected: {rowsAffected}");
			}
			catch (Exception ex)
			{
				// Log the error details for debugging
				Console.WriteLine($"Error adding student: {ex.Message}");
			}
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

			return student;
		}

		public async Task<List<SchoolClass>> GetSchoolClassesAsync()
		{
			var SchoolClasses =  await _context.SchoolClasses.OrderBy(m => m.ClassName).ToListAsync();
			return SchoolClasses;
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

        public async Task<List<Student>> GetClassStudentsByClassIdAsync(int id)
        {
            var ClassStudents = await _context.Students.Where(s => s.ClassId == id).ToListAsync();
			
			if (!ClassStudents.Any())
			{
				throw new SchoolClassNotFoundException(id);
			}
            return ClassStudents;
        }
		
		public async Task<long> GenerateStudentIdAsync()
		{
			// Generate a random 11-digit number
			long studentId = await GenerateRandom11DigitNumber(_random);


			return studentId;
		}

		public async Task<bool> IsStudentIdExistsAsync(long studentId)
		{
			bool studentExists = await _context.Students.AnyAsync(n  => n.StudentId == studentId);
			return studentExists;
		}

		public async Task<long> GenerateRandom11DigitNumber(Random rnd)
		{
			bool studentIdExists = true;
			long studentId = 0;

			while (studentIdExists)
			{
				// The minimum value with 11 digits
				long min = 10000000000; // 10^10
										// The maximum value with 11 digits
				long max = 99999999999; // 10^11 - 1

				studentId = (long)(rnd.NextDouble() * (max - min + 1)) + min;


				studentIdExists = await IsStudentIdExistsAsync(studentId);

			}

			return studentId;
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
