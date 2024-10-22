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
        private readonly IUserRegistrationService _userRegistrationService;

        public RegisterUserModel(
            ApplicationDbContext context, 
            IIdValidationService idValidationService, 
            ITeacherService teacherService, 
            IStudentService studentService, 
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            IUserRegistrationService userRegistrationService)
        {
            _context = context;
            _idValidationService = idValidationService;
            _teacherService = teacherService;
            _studentService = studentService;
            _userService = userService;
            _userManager = userManager;
            _userRegistrationService = userRegistrationService;
        }

        [BindProperty]
        public ApplicationUser NewApplicationUser { get; set; }
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

            IdentityResult result;

            if (studentId != null)
            {
                result = await _userRegistrationService.RegisterStudentAsync(NewApplicationUser, studentId.Value);
            }
            else if (teacherId != null)
            {
                result = await _userRegistrationService.RegisterTeacherAsync(NewApplicationUser, teacherId.Value);
            }
            else
            {
                // Handle the case where neither ID is provided
                ModelState.AddModelError(string.Empty, "Either Teacher ID or Student ID must be provided.");
                return Page();
            }

            // Check if the result indicates success or failure
            if (result.Succeeded)
            {
                // Handle success case
                if (studentId.HasValue)
                {
                    TempData["SuccessMessage"] = "Student has been successfully registered.";
                    return RedirectToPage(new { studentId = studentId });
                }
                else if (teacherId.HasValue)
                {
                    TempData["SuccessMessage"] = "User has been successfully registered.";
                    return RedirectToPage(new { teacherId = teacherId });
                }

                return Page();
            }

            // If creation fails, add errors to the ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
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
