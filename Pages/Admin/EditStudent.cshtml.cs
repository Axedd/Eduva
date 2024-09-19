using AuthWebApp.Data;
using AuthWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthWebApp.Pages.Admin
{
    public class EditStudentModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditStudentModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public long StudentId { get; set; }
        [BindProperty]
        public Student? Student { get; set; }
        public List<StudentClass> StudentClasses { get; set; }

        public async Task<IActionResult> OnGetAsync(long studentId)
        {
            Student = await _context.Students
                                    .Where(s => s.StudentId == studentId)
                                    .Include(sc => sc.StudentClass)
                                    .Select(s => new Student
                                    {
                                        StudentId = s.StudentId,
                                        FirstName = s.FirstName ?? string.Empty, 
                                        LastName = s.LastName ?? string.Empty,
                                        StudentClass = s.StudentClass,
                                        ProfilePicturePath = s.ProfilePicturePath ?? "/students/default.jpg", 
                                        Gender = s.Gender ?? "Unknown",  
                                    })
                                    .FirstOrDefaultAsync();

			StudentClasses = await _context.StudentClasses.OrderBy(sc => sc.ClassName).ToListAsync();


			if (Student == null)
            {
                return NotFound();
            }

            return Page();
        }

		public async Task<IActionResult> OnPostAsync()
		{


			long studentId = Student!.StudentId;

			// Retrieve the existing student from the database by ID.
			var existingStudent = await _context.Students
				.Include(s => s.StudentClass) // Include related entities if needed
				.FirstOrDefaultAsync(s => s.StudentId == Student.StudentId);

			if (existingStudent == null)
			{
				Console.WriteLine("Student not found in the database.");
				return NotFound();
			}

			existingStudent.FirstName = Student!.FirstName;
			existingStudent.LastName = Student!.LastName;
            if (Student.StudentClassId != null)
			{
                existingStudent.StudentClassId = Student.StudentClassId;
            }


            _context.Students.Update(existingStudent);

			// Try saving changes and handle potential exceptions
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!StudentExists(Student.StudentId))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return RedirectToPage("/Admin/Settings");
		}

		private bool StudentExists(long studentId)
		{
			return _context.Students.Any(e => e.StudentId == studentId);
		}
	}
}
