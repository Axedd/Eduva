using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;
using static SchoolSystem.Models.StudentClassSubjects;

namespace SchoolSystem.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;
        public TeacherService(ApplicationDbContext context)
        {
            _context = context;
        }

        private Teacher Teacher { get; set; }

        public async Task<Teacher> GetTeacherById(long teacherId)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.TeacherId == teacherId);

            if (teacher == null)
            {
                throw new KeyNotFoundException($"Teacher with ID {teacherId} not found.");
            }

            return teacher;
        }

        public async Task<long> GetTeacherByUserId(string userId)
        {
            return await _context.Teachers.Where(t => t.UserId == userId).Select(t => t.TeacherId).FirstOrDefaultAsync();
        }

        public async Task<TeacherDto> GetTeachersWithStudentClassesAsync(long teacherId)
        {
            return await _context.Teachers
                .Where(t => t.TeacherId == teacherId)
                .Include(t => t.StudentClassSubjects)
                    .ThenInclude(scs => scs.Subject) // Include the Subject through StudentClassSubjects
                .Select(t => new TeacherDto
                {
                    TeacherId = t.TeacherId,
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    ProfilePicturePath = t.ProfilePicturePath,
                    StudentClassSubjectsDto = t.StudentClassSubjects.Select(scs => new StudentClassSubjectsDto
                    {
                        StudentClassId = scs.StudentClassId,
                        SubjectId = scs.SubjectId,
                        TeacherId = scs.TeacherId,
                        Subject = new SubjectDto // Ensure this is a SubjectDto, not Subject
                        {
                            SubjectId = scs.Subject.SubjectId,
                            SubjectName = scs.Subject.SubjectName // Correctly map to SubjectDto
                        },
                        StudentClassDto = new StudentClassDto
                        {
                            StudentClassId = scs.StudentClassId,
                            ClassName = scs.StudentClass.ClassName
                        }
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<List<TeacherDto>> GetAllTeachersWithSubjectsAsync()
        {
            try
            {
                return await _context.Teachers
                    .Include(t => t.SubjectTeachers) // Include the SubjectTeachers relationship
                        .ThenInclude(st => st.Subject) // Include the Subject details
                    .Select(t => new TeacherDto
                    {
                        TeacherId = t.TeacherId,
                        FirstName = t.FirstName,
                        LastName = t.LastName,
                        ProfilePicturePath = t.ProfilePicturePath,
                        Subjects = t.SubjectTeachers.Select(st => new SubjectTeachersDto
                        {
                            SubjectId = st.SubjectId,
                            TeacherId = st.TeacherId,
                            Subject = new SubjectDto // Map the Subject to SubjectDto
                            {
                                SubjectId = st.Subject.SubjectId,
                                SubjectName = st.Subject.SubjectName
                            }
                        }).ToList() // List of SubjectTeachersDto with Subject info
                    })
                    .OrderBy(t => t.FirstName) // Order by First Name
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception 
                throw; // or handle the error as appropriate for your application
            }
        }


        public async Task UpdateTeacherAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
        }

    }
}
