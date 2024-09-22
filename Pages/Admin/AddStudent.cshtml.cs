using AuthWebApp.Data;
using AuthWebApp.Models;
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
        [BindProperty]
        public StudentClass StudentClass { get; set; }

        public List<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();

        public async Task<IActionResult> OnGetAsync()
        {
            StudentClasses = await _studentClassService.GetStudentClassesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Student.StudentId = await _studentService.GenerateStudentId();
            Student.JoinedDate = DateTime.Now;


            Console.WriteLine(Student.StudentId);
            Console.WriteLine(Student.JoinedDate);
            Console.WriteLine(Student.StudentClassId);

            StudentClasses = await _studentClassService.GetStudentClassesAsync();

            return Page();
        }
    }
}
