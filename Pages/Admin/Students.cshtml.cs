using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Models;
using SchoolSystem.Pages.Base;
using SchoolSystem.Services;

namespace SchoolSystem.Pages.Admin
{
    public class StudentsModel : BasePageModel
	{
        public StudentsModel(StudentService studentService) : base(studentService) { }


        public List<SchoolClass> schoolClasses { get; set; }

		[BindProperty]
		public Student Student { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
			schoolClasses = await _studentService.GetSchoolClassesAsync();

            return Page();
		}


        public async Task<IActionResult> OnPostAsync()
        {
            Student.StudentId = await _studentService.GenerateStudentIdAsync();
			schoolClasses = await _studentService.GetSchoolClassesAsync();

            await _studentService.AddNewStudentAsync(Student);

			if (!AddStudentCheck())
            {
                return Page();
            }
			

			return Page();
        }

        public bool AddStudentCheck()
        {
            if (Student.FirstName == "")
            {
                return false;
            }
            return true;
        }
    }
}
