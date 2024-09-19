using AuthWebApp.Data;
using AuthWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Services;

namespace SchoolSystem.Pages.Admin
{
    public class AddSubjectModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly SubjectService _subjectService;

        public AddSubjectModel(ApplicationDbContext context, SubjectService subjectService)
        {
            _context = context;
            _subjectService = subjectService;
        }

        [BindProperty]
        public Subject Subject { get; set; }
        public List<Subject> Subjects { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Subjects = await _context.Subjects.ToListAsync();



            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int generatedSubjectId;
            var existingSubjectNames = await _context.Subjects.Select(s => s.SubjectName).ToListAsync();

            if (existingSubjectNames.Contains(Subject.SubjectName))
            {
                ModelState.AddModelError(string.Empty, "A subject with this name already exists.");
                return Page();
            }

            generatedSubjectId = await _subjectService.GenerateSubjectIdAsync();
            Subject.SubjectId = generatedSubjectId;

            _context.Subjects.Add(Subject);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the subject to the database.");
                Console.WriteLine($"Error saving changes: {ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            Subjects = await _context.Subjects.ToListAsync();
            return Page();
        }
    }
}
