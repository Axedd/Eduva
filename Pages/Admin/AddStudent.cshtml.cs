using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Services;

namespace SchoolSystem.Pages.Admin
{
    public class AddStudentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IStudentService _studentService;
        private readonly IStudentClassService _studentClassService;

        public AddStudentModel(ApplicationDbContext context, IStudentService studentService, IStudentClassService studentClassService)
        {
            _context = context;
            _studentService = studentService;
            _studentClassService = studentClassService;
        }

        [BindProperty]
        public Student Student { get; set; }
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

            // If ModelState is invalid, return the page with validation errors
            if (!ModelState.IsValid)
            {
                StudentClasses = await _studentClassService.GetStudentClassesAsync();
                return Page();
            }


            Student.StudentId = await _studentService.GenerateStudentId();
            Student.JoinedDate = DateTime.Now;
            Student.ProfilePicturePath = "/students/default.jpg";


            _context.Students.Add(Student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Student added successfully!";

            StudentClasses = await _studentClassService.GetStudentClassesAsync();
            return RedirectToPage();
        }
    }
}
