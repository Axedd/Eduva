using AuthWebApp.Data;
using AuthWebApp.Models;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;

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

    }
}
