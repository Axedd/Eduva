using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Data;
using SchoolSystem.Models;
using SchoolSystem.Pages.Base;
using SchoolSystem.Services;

namespace SchoolSystem.Pages.Students
{
    public class indexModel : BasePageModel
    {

        public indexModel(StudentService studentService) : base(studentService) { }
        public List<Student> Students { get; set; }

		public async Task<IActionResult> OnGetAsync()
        {
			Students = await _studentService.GetStudentsAsync();

			return Page();
        }
    }
}
