using AuthWebApp.Data;
using AuthWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthWebApp.Pages.StudentClasses
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<StudentClass> StudentClasses { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            StudentClasses = await _context.StudentClasses.ToListAsync();
            if (StudentClasses == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
