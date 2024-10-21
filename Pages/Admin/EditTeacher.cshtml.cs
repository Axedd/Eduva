using SchoolSystem.Data;
using SchoolSystem.Migrations;
using SchoolSystem.Models;
using SchoolSystem.Pages.Admin;
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
        private readonly IIdValidationService _idValidationService;
        private readonly ITeacherService _teacherService;
        private readonly ISubjectService _subjectService;
        private readonly ILogger<EditTeacherModel> _logger;

        public EditTeacherModel(IIdValidationService idValidationService, ITeacherService teacherService,  ILogger<EditTeacherModel> logger, ISubjectService subjectService)
        {
            _idValidationService = idValidationService;
            _teacherService = teacherService;
            _logger = logger;
            _subjectService = subjectService;
        }

        [BindProperty]
        public Teacher? Teacher { get; set; }
        public List<Subject> Subjects { get; set; }
        [BindProperty]
        public long SelectedSubjectId { get; set; } // Add this property

        public async Task<IActionResult> OnGetAsync(long teacherId)
        {

            if (!await _idValidationService.IsValidTeacherIdAsync(teacherId))
            {
                _logger.LogWarning("Invalid Teacher ID: {}", teacherId);
                return NotFound();
            }

            Teacher = await _teacherService.GetTeacherById(teacherId);

            Subjects = await _subjectService.GetAllSubjectsAsync();

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

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadTeacherData();

            return Page();
        }


        public async Task<IActionResult> OnPostAssignTeacherSubjectAsync()
        {
            if (Teacher == null)
            {
                return NotFound();
            }

            if (!long.TryParse(Request.Form["SelectedSubjectId"], out var subjectId))
            {
                ModelState.AddModelError("SelectedSubjectId", "Invalid subject ID."); // Specify the field name
                return await LoadTeacherData(); // Create a method to load data again
            }

            var subjectTeacher = new Models.SubjectTeachers
            {
                SubjectId = subjectId,
                TeacherId = Teacher.TeacherId
            };

            // Add the new subject-teacher relationship to the database
            await _subjectService.AddSubjectTeacherAsync(subjectTeacher);

            return RedirectToPage(new { teacherId = Teacher.TeacherId });
        }

        private async Task<IActionResult> LoadTeacherData()
        {
            // Load necessary data again for the view
            if (!await _idValidationService.IsValidTeacherIdAsync(Teacher.TeacherId))
            {
                _logger.LogWarning("Invalid Teacher ID: {}", Teacher.TeacherId);
                return NotFound();
            }

            Teacher = await _teacherService.GetTeacherById(Teacher.TeacherId);

            Subjects = await _subjectService.GetAllSubjectsAsync();

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

    }
}
