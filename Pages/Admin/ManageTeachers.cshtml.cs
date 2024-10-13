using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;

namespace SchoolSystem.Pages.Admin
{
    public class ManageTeachersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
		private readonly ITeacherService _teacherService;

        public ManageTeachersModel(ApplicationDbContext context, ITeacherService teacherService)
        {
            _context = context;
			_teacherService = teacherService;
        }

        public TeacherDto TeacherDetails { get; set; }
		public List<TeacherDto> SubjectTeachersDtos { get; set; }
		public long? TeacherId { get; set; }

		public async Task<IActionResult> OnGetAsync(long? teacherId)
		{
			try
			{

				if (teacherId != null)
				{
					TeacherDetails = await _teacherService.GetTeacherWithStudentClassesAsync(teacherId.Value, true);
                    TeacherId = teacherId.Value;

                } else
				{
                    SubjectTeachersDtos = await _teacherService.GetTeachersAsync(true, false);
                }

				
            }
			catch (Exception ex)
			{
				// Log the exception or display an error message
				Console.WriteLine($"Error: {ex.Message}");
				// Return an error page or message
				return RedirectToPage("/Error");
			}

			return Page();
		}

	}
}