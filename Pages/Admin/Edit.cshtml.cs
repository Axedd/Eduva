using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Models;
using SchoolSystem.Pages.Base;
using SchoolSystem.Services;

namespace SchoolSystem.Pages.Admin
{
    public class EditModel : BasePageModel
    {
        public EditModel(StudentService studentService) : base(studentService) { }

        public Student Student { get; set; }


        public async Task<IActionResult> OnGetAsync(long StudentId)
        {
            Student = await _studentService.GetStudentById(StudentId);


            return Page();
        }
    }
}
