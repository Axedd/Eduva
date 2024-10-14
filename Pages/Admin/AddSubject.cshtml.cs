using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Services;
using SchoolSystem.Pages.Shared;

namespace SchoolSystem.Pages.Admin
{
    public class AddSubjectModel : BaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly SubjectService _subjectService;

        public AddSubjectModel(ApplicationDbContext context, SubjectService subjectService, ILogger<BaseService> logger)
            : base(logger)
        {
            _context = context;
            _subjectService = subjectService;
        }

        [BindProperty]
        public Subject NewSubject { get; set; }
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

            if (existingSubjectNames.Contains(NewSubject.SubjectName))
            {
                ModelState.AddModelError(string.Empty, "A subject with this name already exists.");
                return Page();
            }

            generatedSubjectId = await _subjectService.GenerateSubjectIdAsync();
            NewSubject.SubjectId = generatedSubjectId;

            _context.Subjects.Add(NewSubject);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the subject to the database.");
                HandleError(ex, $"Error saving changes for subject: {NewSubject.SubjectName}"); 
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                HandleError(ex, "Unexpected error occurred while adding a subject.");
            }

            Subjects = await _context.Subjects.ToListAsync();
            return Page();
        }
    }
}
