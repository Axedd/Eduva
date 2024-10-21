using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using SchoolSystem.Interfaces;

namespace SchoolSystem.Pages.Admin
{
    public class ManageClassesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IStudentClassService _studentClassService;
        private readonly ISubjectService _subjectService;

        public ManageClassesModel(ApplicationDbContext context, IStudentClassService studentClassService, ISubjectService subjectService)
        {
            _context = context;
            _studentClassService = studentClassService;
            _subjectService = subjectService;
        }

        public List<StudentClass> StudentClasses { get; set; }
        public List<StudentClassSubjects> StudentClassSubjects { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                StudentClasses = await _studentClassService.GetStudentClassesAsync();

                return Page();
            }
            catch (Exception ex)
            {
                // Log the error
                // Handle the error (e.g., return an error view or message)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public async Task<IActionResult> OnGetSubjectByStudentClassIdAsync(int classId)
        {
            try
            {
                var subjects = await _subjectService.GetStudentClassSubjectsAsync(classId);

                // JSON for javascript
                return new JsonResult(subjects);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        // When user selects a studentclass subject this handler retrieves the relevant subjectinfo
        public async Task<IActionResult> OnGetSubjectInfoByStudentClassIdAsync(int classId, int subjectId)
        {
            try
            {
                var subjectInfo = await _subjectService.GetStudentClassSubjectInfoAsync(classId, subjectId);

                // JSON for javascript
                return new JsonResult(subjectInfo);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}