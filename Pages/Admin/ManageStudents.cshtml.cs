using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;

namespace SchoolSystem.Pages.Admin
{
    public class ManageStudentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IClassService _classService;
        private readonly IStudentService _studentService;

        public ManageStudentsModel(ApplicationDbContext context,
            IClassService classService,
            IStudentService studentService)
        {
            _context = context;
            _classService = classService;
            _studentService = studentService;
        }

        public List<StudentClass> StudentClasses { get; set; }
        public List<Student> ClassStudents { get; set; }

        public List<Student> Students { get; set; }

        public long? SelectedClassId { get; set; }
        public long? SelectedStudentId { get; set; }

        // Fetch all classes and students for the page load
        public async Task<IActionResult> OnGetAsync(int? classId, long? studentId)
        {
            StudentClasses = await _classService.GetAllClassesAsync();
            
            if (StudentClasses.Count == 0)
            {
                return NotFound();
            }

            if (classId.HasValue)
            {
                Students = await _studentService.GetStudentsByClassAsync(classId.Value);
                SelectedClassId = classId;

                if (studentId.HasValue)
                {
                    SelectedStudentId = studentId.Value;
                }

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