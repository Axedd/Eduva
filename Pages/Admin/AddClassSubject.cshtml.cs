using AuthWebApp.Data;
using AuthWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace SchoolSystem.Pages.Admin
{
    public class AddClassSubjectModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        
        public AddClassSubjectModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<StudentClassSubjects> StudentClassSubjects { get; set; }
        public List<Subject> Subjects { get; set; }



        public async Task<IActionResult> OnGetAsync(int studentClassId)
        {
            StudentClassSubjects = await _context.StudentClassSubjects
                .Include(scs => scs.Subject)
                .Include(scs => scs.Teacher)
                .Where(scs => scs.StudentClassId == studentClassId)
                .ToListAsync();

			Subjects = await _context.Subjects
                .Include(s => s.SubjectTeachers)
                .ToListAsync();





			return Page();
        }
    }
}
