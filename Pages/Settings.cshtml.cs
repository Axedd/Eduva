using AuthWebApp.Data;
using AuthWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthWebApp.Pages
{
    [Authorize(Roles = "Admin")]
    public class SettingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SettingsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<StudentClass> StudentClasses { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
			StudentClasses = await _context.StudentClasses.ToListAsync();
            return Page();
        }
    }
}
