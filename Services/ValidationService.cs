using Microsoft.EntityFrameworkCore;
using SchoolSystem.Data;
using SchoolSystem.Interfaces;

namespace SchoolSystem.Services
{
    public class ValidationService : IValidationService
    {
        private readonly ApplicationDbContext _context;

        public ValidationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsValidSubjectAsync(string subjectName)
        {
            return await _context.Subjects.AnyAsync(s => s.SubjectName == subjectName);
        }


    }
}
