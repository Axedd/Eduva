using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;
using Microsoft.Extensions.Logging;
using SchoolSystem.Pages.Shared; // Make sure to include this for logging

namespace SchoolSystem.Pages.Admin
{
	public class EditStudentModel : BaseService
	{
		private readonly IIdValidationService _idValidationService;
		private readonly IStudentClassService _studentClassService;
		private readonly IStudentService _studentService;
		private readonly ILogger<EditStudentModel> _logger; // Logger for diagnostics

		public EditStudentModel(
			IIdValidationService idValidationService,
			IStudentClassService studentClassService,
			IStudentService studentService,
			ILogger<EditStudentModel> logger) : base(logger)
		{
			_idValidationService = idValidationService;
			_studentClassService = studentClassService;
			_studentService = studentService;
			_logger = logger; // Initialize the logger
		}

		[BindProperty(SupportsGet = true)]
		public long StudentId { get; set; }
		[BindProperty]
		public Student? Student { get; set; }
		public List<StudentClass> StudentClasses { get; set; } = new List<StudentClass>(); // Ensure initialized

		public async Task<IActionResult> OnGetAsync(long studentId)
		{
			if (!await _idValidationService.IsValidStudentIdAsync(studentId))
			{
				_logger.LogWarning("Invalid student ID: {StudentId}", studentId);
				return BadRequest("Invalid student ID.");
			}

			Student = await _studentService.GetStudentById(studentId);

			StudentClasses = await _studentClassService.GetStudentClassesAsync();

			if (Student == null)
			{
				_logger.LogWarning("Student not found: {StudentId}", studentId);
				return NotFound();
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
            if (Student == null || !await _idValidationService.IsValidStudentIdAsync(Student.StudentId))
            {
                _logger.LogWarning("Invalid or null student ID.");
                return BadRequest("Invalid student information.");
            }

            if (!ModelState.IsValid)
			{
				StudentClasses = await _studentClassService.GetStudentClassesAsync();
				Student = await _studentService.GetStudentById(StudentId);
				
				return Page();
			}

			try
			{
				await _studentService.UpdateStudentAsync(Student);
				TempData["SuccessMessage"] = "Student details updated successfully!";
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, "Concurrency issue while updating student: {StudentId}", Student.StudentId);
				ModelState.AddModelError(string.Empty, "Concurrency error: Another user may have updated the student. Please try again.");
				StudentClasses = await _studentClassService.GetStudentClassesAsync();
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error while updating student: {StudentId}", Student.StudentId);
				ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
				StudentClasses = await _studentClassService.GetStudentClassesAsync();
				return Page();
			}

			return RedirectToPage(new { studentId = Student.StudentId });
		}
	}
}