using SchoolSystem.Data;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;

namespace SchoolSystem.Services
{
    public class IdValidationService : IIdValidationService
    {
        private readonly ApplicationDbContext _context;

        public IdValidationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsValidStudentIdAsync(long studentId)
        {
            return await _context.Students.AnyAsync(s => s.StudentId == studentId);
        }

        public async Task<bool> IsValidTeacherIdAsync(long teacherId)
        {
            return await _context.Teachers.AnyAsync(s => s.TeacherId == teacherId);
        }

        public async Task<bool> IsValidStudentClassIdAsync(int studentClassId)
        {
            return await _context.StudentClasses.AnyAsync(sc => sc.StudentClassId == studentClassId);
        }

        public async Task<bool> IsValidSubjectId(long subjectId)
        {
            return await _context.Subjects.AnyAsync(s => s.SubjectId == subjectId);
        }

        public async Task<bool> IsValidStudentClassSubject(long studentClassId, long subjectId)
        {
            return  true;
        }

        public async Task<bool> IsValidAgendaId(long agendaId)
        {
            return _context.Agenda.Where(a => a.AgendaId == agendaId).Any();
        }

    }
}
