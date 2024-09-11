using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Services;



namespace SchoolSystem.Pages.Base
{
	public class BasePageModel : PageModel
	{
		protected readonly StudentService _studentService;

		public BasePageModel(StudentService studentService)
		{
			_studentService = studentService;
		}
	}
}
