using AuthWebApp.Data;
using AuthWebApp.Migrations;
using AuthWebApp.Models;
using AuthWebApp.Pages.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using System;

namespace SchoolSystem.Pages.Admin
{
    public class EditTeacherModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdValidationService _idValidationService;
        private readonly ILogger<EditTeacherModel> _logger; 

        public EditTeacherModel(ApplicationDbContext context, IIdValidationService idValidationService, ILogger<EditTeacherModel> logger)
        {
            _context = context;
            _idValidationService = idValidationService;
            _logger = logger;
        }

        [BindProperty]
        public Teacher? Teacher { get; set; }
        public List<Subject> Subjects { get; set; }

        public async Task<IActionResult> OnGetAsync(long teacherId)
        {

            if (!await _idValidationService.IsValidTeacherIdAsync(teacherId))
            {
                _logger.LogWarning("Invalid Teacher ID: {}", teacherId);
                return NotFound();
            }

            Teacher = await _context.Teachers
                .Include(t => t.SubjectTeachers)
                .ThenInclude(st => st.Subject)
                .FirstOrDefaultAsync(t => t.TeacherId == teacherId);
            
            Subjects = await _context.Subjects.ToListAsync();

            if (Teacher == null)
            {
                _logger.LogWarning("Could Not Find Teacher");
                return NotFound();
            }

            foreach (var subject in Teacher.SubjectTeachers)
            {
                if (subject.Subject != null && Subjects.Contains(subject.Subject))
                {
                    Subjects.Remove(subject.Subject);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAssignTeacherSubjectAsync()
        {
            var selectedSubjectId = Request.Form["SelectedSubjectId"];

            if (Teacher == null)
            {
                return NotFound();
            }

            if (!long.TryParse(selectedSubjectId, out var subjectId))
            {
                ModelState.AddModelError(string.Empty, "Invalid subject ID.");
                return Page();
            }

            var subjectTeacher = new Models.SubjectTeachers
            {
                SubjectId = subjectId,
                TeacherId = Teacher.TeacherId
            };

            // Add the new subject-teacher relationship to the database
            await _context.SubjectTeachers.AddAsync(subjectTeacher);

            // Save changes in the database
            await _context.SaveChangesAsync();


            // Redirect to the teachers page after successful assignment
            return RedirectToPage("/Admin/Teachers");
        }

    }
}
