using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterUserModel(
            ApplicationDbContext context, 
            IIdValidationService idValidationService, 
            ITeacherService teacherService, 
            IStudentService studentService, 
            IUserService userService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _idValidationService = idValidationService;
            _teacherService = teacherService;
            _studentService = studentService;
            _userService = userService;
            _userManager = userManager;
        }

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }
        public Teacher Teacher { get; private set; }
        public Student Student { get; private set; }


        public async Task<IActionResult> OnGetAsync(long? teacherId, long? studentId)
        {

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

            ApplicationUser newUser = new ApplicationUser
            {
                UserName = username,  // This should not be null or empty
                FirstName = ApplicationUser.FirstName,
                LastName = ApplicationUser.LastName,
                DateOfBirth = ApplicationUser.DateOfBirth,
                Address = ApplicationUser.Address,
                JoinedDate = ApplicationUser.JoinedDate,
                Email = null
            };

            if (newUser == null)
            {
                throw new InvalidOperationException("The ApplicationUser object could not be created.");
            }

            var result = await _userManager.CreateAsync(newUser);

            if (result.Succeeded)
            {
                return Page();
            }

            // If creation fails, add errors to the ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description); // Add the error messages
            }

            // Inspect the errors
            var errors = result.Errors;  // Add a breakpoint here to see why it failed.

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    Console.WriteLine($"Error: {error.Description}");  // Log the errors (for debugging)
                }
            }


            // Reload data for the page
            if (teacherId is long isValidTeacherId && await _idValidationService.IsValidTeacherIdAsync(isValidTeacherId))
            {
                Teacher = await _teacherService.GetTeacherById(isValidTeacherId);
            }
            else if (studentId is long validStudentId && await _idValidationService.IsValidStudentIdAsync(validStudentId))
            {
                Student = await _studentService.GetStudentById(validStudentId);
            }

            return Page();
        }
    }
}
