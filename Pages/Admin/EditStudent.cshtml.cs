using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;
using Microsoft.Extensions.Logging; // Make sure to include this for logging

namespace SchoolSystem.Pages.Admin
{
    public class EditStudentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdValidationService _idValidationService;
        private readonly IStudentClassService _studentClassService;
        private readonly ILogger<EditStudentModel> _logger; // Logger for diagnostics

        public EditStudentModel(ApplicationDbContext context, IIdValidationService idValidationService, IStudentClassService studentClassService, ILogger<EditStudentModel> logger)
        {
            _context = context;
            _idValidationService = idValidationService;
            _studentClassService = studentClassService;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public long StudentId { get; set; }
        [BindProperty]
        public Student? Student { get; set; }
        public List<StudentClass> StudentClasses { get; set; }

        public async Task<IActionResult> OnGetAsync(long studentId)
        {
            if (!await _idValidationService.IsValidStudentIdAsync(studentId))
            {
                _logger.LogWarning("Invalid student ID: {StudentId}", studentId);
                return NotFound();
            }

            Student = await _context.Students
                .Include(sc => sc.StudentClass)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            StudentClasses = await _studentClassService.GetStudentClassesAsync();

            if (Student == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Student == null || !await _idValidationService.IsValidStudentIdAsync(Student.StudentId))
            {
                _logger.LogWarning("Invalid or null student ID.");
                return NotFound();
            }

            var existingStudent = await _context.Students
                .Include(s => s.StudentClass)
                .FirstOrDefaultAsync(s => s.StudentId == Student.StudentId);

            if (existingStudent == null)
            {
                _logger.LogWarning("Student not found in the database: {StudentId}", Student.StudentId);
                return NotFound();
            }

            existingStudent.FirstName = Student.FirstName;
            existingStudent.LastName = Student.LastName;

            if (Student.StudentClassId != null)
            {
                if (!await _idValidationService.IsValidStudentClassIdAsync((int)Student.StudentClassId))
                {
                    _logger.LogWarning("Invalid Student Class ID: {StudentClassId}", Student.StudentClassId);
                    return BadRequest("Invalid Student Class ID.");
                }

                existingStudent.StudentClassId = Student.StudentClassId;
            }

            _context.Students.Update(existingStudent);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("Concurrency issue while updating student: {StudentId}", Student.StudentId);
                if (!await _idValidationService.IsValidStudentIdAsync((int)Student.StudentId))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("/Admin/Settings");
        }
    }
}
