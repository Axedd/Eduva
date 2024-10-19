using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Services;
using SchoolSystem.Pages.Shared;
using Microsoft.EntityFrameworkCore;

namespace SchoolSystem.Pages.Admin
{
    public class AddStudentModel : BaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStudentService _studentService;
        private readonly IStudentClassService _studentClassService;

        public AddStudentModel(ApplicationDbContext context, IStudentService studentService, IStudentClassService studentClassService, ILogger<BaseService> logger)
            : base(logger)
        {
            _context = context;
            _studentService = studentService;
            _studentClassService = studentClassService;
        }

        [BindProperty]
        public Student NewStudent { get; set; } // Consider renaming for clarity
        public List<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();

        public async Task<IActionResult> OnGetAsync()
        {
            StudentClasses = await _studentClassService.GetStudentClassesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Request.Form["Student.Gender"] == "Select Gender")
            {
                ModelState.AddModelError("Student.Gender", "Please select a gender.");
            }

            if (!ModelState.IsValid)
            {
                StudentClasses = await _studentClassService.GetStudentClassesAsync();
                return Page();
            }

            try
            {
                NewStudent.StudentId = await _studentService.GenerateStudentId();
                NewStudent.JoinedDate = DateTime.Now;
                NewStudent.ProfilePicturePath = "/students/default.jpg";

                _context.Students.Add(NewStudent);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Student added successfully!";
                return RedirectToPage();
            }
            catch (DbUpdateException ex)
            {
                HandleError(ex, "Error occurred while adding the student to the database.");
                ModelState.AddModelError(string.Empty, "An error occurred while saving the student. Please try again.");
                _logger.LogError(ex, "Database update error while adding student."); // Logging with LogError
            }
            catch (Exception ex)
            {
                HandleError(ex, "An unexpected error occurred while adding a student.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                _logger.LogError(ex, "Unexpected error while adding student."); // Logging with LogError
            }

            StudentClasses = await _studentClassService.GetStudentClassesAsync();
            return Page();
        }
    }
}