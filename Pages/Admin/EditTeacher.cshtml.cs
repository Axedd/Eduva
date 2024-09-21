using AuthWebApp.Data;
using AuthWebApp.Migrations;
using AuthWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Models;
using System;

namespace SchoolSystem.Pages.Admin
{
    public class EditTeacherModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditTeacherModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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
