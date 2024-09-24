using AuthWebApp.Data;
using AuthWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuthWebApp.Pages.Admin
{
    public class ManageStudentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ManageStudentsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<StudentClass> StudentClasses { get; set; }
        public List<Student> ClassStudents { get; set; }

        // Fetch all classes and students for the page load
        public async Task<IActionResult> OnGetAsync()
        {
            StudentClasses = await _context.StudentClasses
                .Include(sc => sc.Students)
                .OrderBy(s => s.ClassName)
                .ToListAsync();

            if (StudentClasses.Count == 0)
            {
                return NotFound();
            }

            return Page();
        }

        // API to get students by class ID
        public async Task<IActionResult> OnGetGetStudentsByClassAsync(int classId)
        {
            var students = await _context.Students
                .Where(s => s.StudentClassId == classId)
                .Select(s => new
                {
                    s.StudentId,
                    s.FirstName,
                    s.LastName,
                    s.ProfilePicturePath
                })
                .OrderBy(s => s.FirstName)
                .ToListAsync();
            return new JsonResult(students);
        }

        public async Task<IActionResult> OnGetGetStudentByIdAsync(long studentId)
        {
            var student = await _context.Students
        .Where(s => s.StudentId == studentId)
        .Include(s => s.StudentClass)
        .Select(s => new
        {
            s.StudentId,
            s.FirstName,
            s.LastName,
            s.ProfilePicturePath,
            s.Gender,
            s.UserId,
            StudentClass = new
            {
                s.StudentClass.StudentClassId,
                s.StudentClass.ClassName
            }
        })
        .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound();
            }

            return new JsonResult(student);
        }
    }
}