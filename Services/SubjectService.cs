﻿using SchoolSystem.Data;
using SchoolSystem.Models;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Dtos;

namespace SchoolSystem.Services
{
    public class SubjectService : ISubjectService
    {
        private ApplicationDbContext _context;
        private Random _random;

        public SubjectService(ApplicationDbContext context, Random random)
        {
            _context = context;
            _random = random;
        }

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


        public async Task AddSubjectAsync(Subject newSubject)
        {
            var generatedSubjectId = await GenerateSubjectIdAsync();
            newSubject.SubjectId = generatedSubjectId;

            _context.Subjects.Add(newSubject);
            await _context.SaveChangesAsync();

        }

        public async Task AddSubjectTeacherAsync(SubjectTeachers newSubjectTeacher)
        {
            await _context.SubjectTeachers.AddAsync(newSubjectTeacher);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SubjectDto>> GetStudentClassSubjectsAsync(int classId)
        {

            var subjects = await _context.StudentClassSubjects
               .Where(sc => sc.StudentClassId == classId)
               .Include(scs => scs.Subject)
               .Select(scs => new SubjectDto
               {
                   SubjectId = scs.SubjectId,
                   SubjectName = scs.Subject.SubjectName,
                   StudentClassId = scs.StudentClassId,
               })
               .ToListAsync();

            return subjects;
        }
 

        public async Task<StudentClassSubjects> GetStudentClassSubjectById(int studentClassId, long subjectId)
        {
            return await _context.StudentClassSubjects
                .Include(scs => scs.Teacher)
                .Include(scs => scs.Subject)
                .Where(scs => scs.StudentClassId == studentClassId && scs.SubjectId == subjectId)
                .FirstOrDefaultAsync();
        }

        public async Task<StudentClassSubjectDto> GetStudentClassSubjectInfoAsync(int classId, long subjectId)
        {
            var subjectInfo = await _context.StudentClassSubjects
                 .Where(scs => scs.StudentClassId == classId && scs.SubjectId == subjectId)
                 .Include(scs => scs.Subject)
                 .Include(scs => scs.Teacher)
                 .Select(scs => new StudentClassSubjectDto
                 {
                     SubjectId = scs.SubjectId,
                     SubjectName = scs.Subject.SubjectName,
                     StudentClassId = scs.StudentClassId,
                     TeacherName = scs.Teacher.FirstName + " " + scs.Teacher.LastName,
                     TeacherId = scs.Teacher.TeacherId
                 })
                 .FirstOrDefaultAsync();

            return subjectInfo;
        }

        public async Task<Subject> GetSubjectByIdAsync(long subjectId)
        {
            return await _context.Subjects.Where(s => s.SubjectId == subjectId).FirstOrDefaultAsync();
        }

        public async Task<List<SubjectTeacherDto>> GetSubjectTeachersAsync(long subjectId)
        {
            return await _context.SubjectTeachers
                .Include(st => st.Teacher)
                .Include(st => st.Subject)
                .Where(st => st.SubjectId == subjectId)
                .Select(st => new SubjectTeacherDto
                {
                    SubjectId = st.SubjectId,
                    Teacher = new TeacherDto
                    {
                        TeacherId = st.Teacher.TeacherId,
                        FirstName = st.Teacher.FirstName,
                        LastName = st.Teacher.LastName,
                        ProfilePicturePath = st.Teacher.ProfilePicturePath
                    }
                })
        .ToListAsync();
        }

        public async Task<List<Subject>> GetSubjectWithTeachersAsync()
        {
            return await _context.Subjects.Include(s => s.SubjectTeachers).ToListAsync();
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await _context.Subjects.ToListAsync();
        }


	}
}
