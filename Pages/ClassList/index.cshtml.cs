using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Data;
using SchoolSystem.Models;
using SchoolSystem.Pages.Base;
using SchoolSystem.Services;

namespace SchoolSystem.Pages.Class
{
    public class indexModel : BasePageModel
	{
        public indexModel (StudentService StudentService) : base (StudentService) { }

        public List<SchoolClass> SchoolClasses { get; set; }


        public async Task<IActionResult> OnGetAsync() 
        {
            SchoolClasses = await _studentService.GetSchoolClassesAsync();
			SchoolClasses = SchoolClasses.OrderBy(m => m.ClassName).ToList();



			return Page();
        }
    }
}
