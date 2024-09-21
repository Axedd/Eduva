using AuthWebApp.Data;
using AuthWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace SchoolSystem.Pages.Admin
{
    public class EditTeacherModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditTeacherModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Teacher? Teacher { get; set; }
        public List<Subject> Subjects { get; set; }

        public async Task<IActionResult> OnGetAsync(long teacherId)
        {
            Teacher = await _context.Teachers.Include(t => t.SubjectTeachers).ThenInclude(st => st.Subject).FirstOrDefaultAsync(t => t.TeacherId == teacherId);
            Subjects = await _context.Subjects.ToListAsync();

            if (Teacher == null)
            {
                return NotFound();
            }

            foreach (var Subject in Teacher.SubjectTeachers)
            {
                if (Subjects.Contains(Subject.Subject))
                {
                    Subjects.Remove(Subject.Subject);
                }
            }

            return Page();
        }

    }
}
