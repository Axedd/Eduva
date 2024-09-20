using AuthWebApp.Data;
using AuthWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace SchoolSystem.Pages.Admin
{
    public class EditClassModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditClassModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<StudentClass> StudentClasses { get; set; }
        public List<StudentClassSubjects> StudentClassSubjects { get; set; }
        public Subject? Subject { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                StudentClasses = await _context.StudentClasses
                    .OrderBy(sc => sc.ClassName)
                    .ToListAsync();

                
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
                var subjects = await _context.StudentClassSubjects
                    .Where(sc => sc.StudentClassId == classId)
                    .Include(scs => scs.Subject)
                    .Select(scs => new
                    {
                        scs.Subject.SubjectId,
                        scs.Subject.SubjectName,
                        scs.StudentClassId
                    })
                    .ToListAsync();

                return new JsonResult(subjects);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public async Task<IActionResult> OnGetSubjectInfoByStudentClassIdAsync(int classId, int subjectId)
        {
            try
            {
                var subjectInfo = await _context.StudentClassSubjects
                    .Where(scs => scs.StudentClassId == classId && scs.SubjectId == subjectId)
                    .Include(scs => scs.Subject)
                    .Include(scs => scs.Teacher)
                    .Select(scs => new
                    {
                        scs.Teacher.TeacherId,
                        scs.Teacher.FirstName,
                        scs.Teacher.LastName,
                        scs.Subject.SubjectName,
                        scs.Subject.SubjectId
                    })
                    .FirstOrDefaultAsync();

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