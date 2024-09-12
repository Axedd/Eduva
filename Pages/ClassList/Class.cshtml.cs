using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Models;
using SchoolSystem.Pages.Base;
using SchoolSystem.Services;

namespace SchoolSystem.Pages.ClassList
{
    public class ClassModel : BasePageModel
	{

		public ClassModel(StudentService studenService) : base(studenService) { }


		public SchoolClass SchoolClass { get; set; }
		public List<Student> ClassStudents { get; set; }


		public async Task<IActionResult> OnGetAsync(int id)
        {
			SchoolClass = await _studentService.GetSchoolClassByIdAsync(id);
			ClassStudents = await _studentService.GetClassStudentsByClassIdAsync(id);


            return Page();
        }
    }
}
