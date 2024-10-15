using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;

namespace SchoolSystem.Pages.SubNavs
{


    public class StudentClassListModel : PageModel
    {
        private readonly IStudentClassService _studentClassService;
        public StudentClassListModel(IStudentClassService studentClassService)
        {
            _studentClassService = studentClassService;
        }

        public List<StudentDto> Students { get; set; }

        public async Task<IActionResult> OnGet(int studentClassId)
        {
            Students = await _studentClassService.GetAllStudentsFromClassIdAsync(studentClassId);

            return Page();
        }
    }
}
