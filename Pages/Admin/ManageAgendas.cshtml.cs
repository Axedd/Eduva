using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;

namespace SchoolSystem.Pages.Admin
{
    public class ManageAgendasModel : PageModel
    {
        private readonly IStudentClassService _studentClassService;

        public ManageAgendasModel(IStudentClassService studentClassService)
        {
            _studentClassService = studentClassService;
        }

        public List<StudentClass> StudentClasses { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            StudentClasses = await _studentClassService.GetStudentClassesAsync();

            return Page();
        }
    }
}
