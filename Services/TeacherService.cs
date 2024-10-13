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

        public async Task<TeacherDto> GetTeacherWithStudentClassesAsync(long teacherId, bool includeSubjects = false)
        {
            var query = _context.Teachers
                .Where(t => t.TeacherId == teacherId)
                .Include(t => t.StudentClassSubjects)
                    .ThenInclude(scs => scs.Subject); // Always include StudentClassSubjects and related Subject

            if (includeSubjects)
            {
                query = query.Include(t => t.SubjectTeachers)
                             .ThenInclude(st => st.Subject); // Optionally include subjects if needed
            }

            var teacher = await query
                .Select(t => new TeacherDto
                {
                    TeacherId = t.TeacherId,
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    ProfilePicturePath = t.ProfilePicturePath,
                    // Map StudentClassSubjectsDto
                    StudentClassSubjectsDto = t.StudentClassSubjects.Select(scs => new StudentClassSubjectsDto
                    {
                        StudentClassId = scs.StudentClassId,
                        SubjectId = scs.SubjectId,
                        TeacherId = scs.TeacherId,
                        Subject = new SubjectDto
                        {
                            SubjectId = scs.Subject.SubjectId,
                            SubjectName = scs.Subject.SubjectName
                        },
                        StudentClassDto = new StudentClassDto
                        {
                            StudentClassId = scs.StudentClassId,
                            ClassName = scs.StudentClass.ClassName
                        }
                    }).ToList(),
                    // Optionally map Subjects
                    Subjects = includeSubjects ? t.SubjectTeachers.Select(st => new SubjectTeachersDto
                    {
                        SubjectId = st.SubjectId,
                        TeacherId = st.TeacherId,
                        Subject = new SubjectDto
                        {
                            SubjectId = st.Subject.SubjectId,
                            SubjectName = st.Subject.SubjectName
                        }
                    }).ToList() : null
                })
                .FirstOrDefaultAsync();

            // Handle case when teacher is not found
            if (teacher == null)
            {
                // You could throw an exception, return null, or handle it as per your logic
                throw new KeyNotFoundException("Teacher not found");
            }

            return teacher;
        }

        public async Task<List<TeacherDto>> GetTeachersAsync(bool includeSubjects, bool includeStudentClasses)
        {
            var query = _context.Teachers.AsQueryable();

            if (includeSubjects)
            {
                query = query.Include(t => t.SubjectTeachers)
                             .ThenInclude(st => st.Subject);
            }

            if (includeStudentClasses)
            {
                query = query.Include(t => t.StudentClassSubjects)
                             .ThenInclude(scs => scs.Subject);
            }

            return await query
                .Select(t => new TeacherDto
                {
                    TeacherId = t.TeacherId,
                    FirstName = t.FirstName,
                    LastName = t.LastName,
                    ProfilePicturePath = t.ProfilePicturePath,
                    Subjects = includeSubjects ? t.SubjectTeachers.Select(st => new SubjectTeachersDto
                    {
                        SubjectId = st.SubjectId,
                        TeacherId = st.TeacherId,
                        Subject = new SubjectDto
                        {
                            SubjectId = st.Subject.SubjectId,
                            SubjectName = st.Subject.SubjectName
                        }
                    }).ToList() : null,
                    StudentClassSubjectsDto = includeStudentClasses ? t.StudentClassSubjects.Select(scs => new StudentClassSubjectsDto
                    {
                        StudentClassId = scs.StudentClassId,
                        SubjectId = scs.SubjectId,
                        TeacherId = scs.TeacherId,
                        Subject = new SubjectDto
                        {
                            SubjectId = scs.Subject.SubjectId,
                            SubjectName = scs.Subject.SubjectName
                        }
                    }).ToList() : null
                })
                .OrderBy(t => t.FirstName)
                .ToListAsync();
        }


        public async Task UpdateTeacherAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
        }

    }
}
