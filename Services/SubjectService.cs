using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace SchoolSystem.Services
{
    public class SubjectService
    {
        private ApplicationDbContext _context;
        private Random _random;

        public SubjectService(ApplicationDbContext context, Random random)
        {
            _context = context;
            _random = random;
        }

        public List<Subject> Subjects { get;  set; }

        public async Task<int> GenerateSubjectIdAsync()
        {

            var existingSubjectIds = await _context.Subjects.Select(s => s.SubjectId).ToListAsync();
            int generatedSubjectId;
            do
            {
                generatedSubjectId = _random.Next(1000, 9999);

            } while (existingSubjectIds.Contains(generatedSubjectId));
            

            return generatedSubjectId;
        }


        public async Task AssignSubjectToClassAsync(long subjectId, long teacherId, int studentClassId)
        {
            var studentClass = await _context.StudentClasses.FindAsync(studentClassId);

            if (studentClass == null)
            {
                throw new Exception("Student class not found");
            }

            var subject = await _context.Subjects.FindAsync(subjectId);

            if (subject == null)
            {
                throw new Exception("Subject not found");
            }

            // Check if the subject is already assigned to the student class
            var existingAssignment = await _context.StudentClassSubjects
                .FirstOrDefaultAsync(scs => scs.StudentClassId == studentClassId && scs.SubjectId == subjectId);

            if (existingAssignment != null)
            {
                throw new Exception("This subject is already assigned to the class!");
            }

            var studentClassSubject = new StudentClassSubjects
            {
                StudentClassId = studentClassId,
                SubjectId = subjectId,
                TeacherId = teacherId,
            };

            await _context.StudentClassSubjects.AddAsync(studentClassSubject);
            await _context.SaveChangesAsync();


        }

    }
}
