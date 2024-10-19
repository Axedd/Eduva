using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Services;
using SchoolSystem.Pages.Shared;
using SchoolSystem.Interfaces;

namespace SchoolSystem.Pages.Admin
{
    public class AddSubjectModel : BaseService
    {
        private readonly ISubjectService _subjectService;
        private readonly IValidationService _validationService;

        public AddSubjectModel(ISubjectService subjectService, IValidationService validationService, ILogger<BaseService> logger)
            : base(logger)
        {
            _subjectService = subjectService;
            _validationService = validationService;
        }

        [BindProperty]
        public Subject NewSubject { get; set; }
        public List<Subject> Subjects { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Subjects = await _subjectService.GetAllSubjectsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (await _validationService.IsValidSubjectAsync(NewSubject.SubjectName))
            {
                ModelState.AddModelError(string.Empty, "A subject with this name already exists.");
                Subjects = await _subjectService.GetAllSubjectsAsync();
                return Page();
            }


            try
            {
                await _subjectService.AddSubjectAsync(NewSubject);
                TempData["SuccessMessage"] = "Subject added successfully!";
                return RedirectToPage();
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

            Subjects = await _subjectService.GetAllSubjectsAsync();
            return Page();
        }
    }
}
