﻿using AuthWebApp.Data;
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

        public async Task<bool> IsValidStudentIdAsync(int studentId)
        {
            return await _context.Students.AnyAsync(s => s.StudentId == studentId);
        }

        public async Task<bool> IsValidStudentClassIdAsync(int studentClassId)
        {
            return await _context.StudentClasses.AnyAsync(sc => sc.StudentClassId == studentClassId);
        }
    }
}