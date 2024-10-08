using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace SchoolSystem.Pages.StudentClasses
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ClassId { get; set; }
        public List<Student> Students { get; set; }
        public StudentClass? StudentClass { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            StudentClass = await _context.StudentClasses
                .Include(sc => sc.Students) // Ensure students are loaded
                .SingleOrDefaultAsync(sc => sc.StudentClassId == ClassId);

            if (StudentClass == null)
            {
                return NotFound();
            }

            Students = StudentClass.Students.OrderBy(s => s.FirstName).ToList();

            return Page();
        }
    }
}
