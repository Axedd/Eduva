using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace SchoolSystem.Pages.Admin
{
    public class ManageTeachersModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ManageTeachersModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Teacher> Teachers { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				Teachers = await _context.Teachers.OrderBy(t => t.FirstName).ToListAsync();
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

		public async Task<IActionResult> OnGetGetTeacherInfoAsync(long teacherId)
		{
			var teacherInfo = await _context.Teachers
				.Where(t => t.TeacherId == teacherId)
				.FirstOrDefaultAsync();

			return new JsonResult(teacherInfo);
		}
	}
}