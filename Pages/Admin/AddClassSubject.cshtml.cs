using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;
using SchoolSystem.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SchoolSystem.Pages.Admin
{
    public class AddClassSubjectModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ISubjectService _subjectService;
        private readonly IStudentClassService _studentClassService;
        private readonly IIdValidationService _idValidationService;

        public AddClassSubjectModel(
            ApplicationDbContext context,
            ISubjectService subjectService,
            IStudentClassService studentClassService,
            IIdValidationService idValidationService)
        {
            _context = context;
            _subjectService = subjectService;
            _studentClassService = studentClassService;
            _idValidationService = idValidationService;
        }

        [BindProperty]
        public long SelectedSubjectId { get; set; }
        [BindProperty]
        public long SelectedTeacherId { get; set; }
        public List<Subject> Subjects { get; set; }
        public StudentClass StudentClass { get; set; }

        public async Task<IActionResult> OnGetAsync(string studentClassId)
        {
            if (!int.TryParse(studentClassId, out int classId) || classId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid class ID.";
                return RedirectToPage("/Admin/EditClass");
            }

            if (!await _idValidationService.IsValidStudentClassIdAsync(classId))
            {
                TempData["ErrorMessage"] = "Invalid student class ID.";
                return RedirectToPage("/Admin/EditClass");
            }

            await LoadDataAsync(classId);
            return Page();
        }

        public async Task<IActionResult> OnGetTeachersFromSubjectIdAsync(long subjectId)
        {
            var subjectTeachers = await _subjectService.GetSubjectTeachersAsync(subjectId);

            return new JsonResult(subjectTeachers);
        }

        public async Task<IActionResult> OnPostAssignSubjectAsync(string studentClassId)
        {
            if (!int.TryParse(studentClassId, out int classId) || classId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid class ID.");
                return Page();
            }

            try
            {
                await _subjectService.AssignSubjectToClassAsync(SelectedSubjectId, SelectedTeacherId, classId);
            }

            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadDataAsync(classId);

                return Page();
            }


            return RedirectToPage("/Admin/ManageClasses");
        }

        // Helper method to load necessary data
        private async Task LoadDataAsync(int studentClassId)
        {
            StudentClass = await _studentClassService.GetStudentClassByIdAsync(studentClassId);
            Subjects = await _subjectService.GetSubjectWithTeachersAsync();
        }
    }
}
