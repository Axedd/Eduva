using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;

namespace SchoolSystem.Pages.Admin
{
    public class ManageUsersModel : PageModel
    {

        private readonly ApplicationDbContext _context;
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;

        public ManageUsersModel(ApplicationDbContext context,
            IStudentService studentService,
            IUserService userService)
        {
            _context = context;
            _studentService = studentService;
            _userService = userService;
        }

        public List<Student> Students { get; set; }
        public List<StudentRoleViewModel> StudentRoleViewModels { get; set; } = new List<StudentRoleViewModel>();

        public string TempPassword { get; private set; }


        public async Task<IActionResult> OnGetAsync()
        {
            StudentRoleViewModels = await _userService.GetStudentsWithRolesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostGenerateTempPasswordAsync(string userId)
        {
            var tempPassword = await _userService.GenerateTempPassword(userId);
            if (tempPassword != null)
            {
                TempPassword = tempPassword; // Store the generated password
            }

            // Always refresh the student roles after generating a password
            StudentRoleViewModels = await _userService.GetStudentsWithRolesAsync();
            return Page();
        }
    }
}
