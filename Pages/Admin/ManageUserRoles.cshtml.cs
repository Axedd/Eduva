using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;

namespace SchoolSystem.Pages.Admin
{
    public class ManageUserRolesModel : PageModel
    {

        private readonly ApplicationDbContext _context;
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;

        public ManageUserRolesModel(ApplicationDbContext context,
            IStudentService studentService,
            IUserService userService)
        {
            _context = context;
            _studentService = studentService;
            _userService = userService;
        }

        public List<Student> Students { get; set; }
        public List<StudentRoleViewModel> StudentRoleViewModels { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            StudentRoleViewModels = await _userService.GetStudentsWithRolesAsync();



            return Page();
        }
    }
}
