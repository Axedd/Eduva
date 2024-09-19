using AuthWebApp.Data;
using AuthWebApp.Models;
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






    }
}
