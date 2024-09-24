using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SchoolSystem.Pages.Account
{
    [Authorize]
    public class UserDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public UserDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser ApplicationUser { get; set; }
        public StudentClass? StudentClass { get; set; }
        public Subject? Subject { get; set; }
        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }

        [BindProperty(SupportsGet = true)]
        public long StudentId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (StudentId <= 0)
            {
                return NotFound();
            }

            // Fetch the student with its class, related subjects, and teachers
            Student = await _context.Students
                .Include(s => s.StudentClass)
                    .ThenInclude(sc => sc.StudentClassSubjects)
                        .ThenInclude(scs => scs.Subject)
                .Include(s => s.StudentClass)
                    .ThenInclude(sc => sc.StudentClassSubjects)
                        .ThenInclude(scs => scs.Teacher)
                .FirstOrDefaultAsync(s => s.StudentId == StudentId);

            if (Student == null)
            {
                return NotFound();
            }

            // Set the StudentClass and its related Subjects
            StudentClass = Student.StudentClass;

            // Example logic to get a specific subject; replace with your actual logic
            Subject = StudentClass.StudentClassSubjects
                .Select(scs => scs.Subject)
                .FirstOrDefault();

            Teacher = StudentClass.StudentClassSubjects
                .Select(scs => scs.Teacher)
                .FirstOrDefault();

            return Page();
        }
    }
}