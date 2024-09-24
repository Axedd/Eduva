using AuthWebApp.Data;
using AuthWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Services;

namespace SchoolSystem.Pages.Admin
{
    public class RegisterUserModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdValidationService _idValidationService;
        private readonly ITeacherService _teacherService;
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;

        public RegisterUserModel(ApplicationDbContext context, IIdValidationService idValidationService, ITeacherService teacherService, IStudentService studentService, IUserService userService)
        {
            _context = context;
            _idValidationService = idValidationService;
            _teacherService = teacherService;
            _studentService = studentService;
            _userService = userService;
        }

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }
        public Teacher Teacher { get; private set; }
        public Student Student { get; private set; }


        public async Task<IActionResult> OnGetAsync(long? teacherId, long? studentId)
        {
            Console.WriteLine(studentId);

            if (teacherId is long validTeacherId && await _idValidationService.IsValidTeacherIdAsync(validTeacherId))
            {
                Teacher = await _teacherService.GetTeacherById(validTeacherId);
            }
            else if (studentId is long validStudentId && await _idValidationService.IsValidStudentIdAsync(validStudentId))
            {
                Student = await _studentService.GetStudentById(validStudentId);
            }
            else
            {
                return NotFound();
            }


            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? teacherId, long? studentId)
        {
            if (!ModelState.IsValid)
            {
                // Reload data if ModelState is invalid
                if (teacherId is long validTeacherId && await _idValidationService.IsValidTeacherIdAsync(validTeacherId))
                {
                    Teacher = await _teacherService.GetTeacherById(validTeacherId);
                }
                else if (studentId is long validStudentId && await _idValidationService.IsValidStudentIdAsync(validStudentId))
                {
                    Student = await _studentService.GetStudentById(validStudentId);
                }

                // Return the page with the existing data and validation errors
                return Page();
            }


            string username = await _userService.GenerateUsername();

            Console.WriteLine(username);

            // Proceed with your user registration logic here...

            return Page();
        }
    }
}
