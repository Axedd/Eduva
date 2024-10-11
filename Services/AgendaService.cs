using Microsoft.EntityFrameworkCore;
using SchoolSystem.Data;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using System;

namespace SchoolSystem.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly ApplicationDbContext _context;
        private Random _random;
        public AgendaService(ApplicationDbContext context, Random random)
        {
            _context = context;
            _random = random;
        }


        public async Task AddAgendaAsync(Agenda agenda)
        {
            await _context.Agenda.AddAsync(agenda);
            await _context.SaveChangesAsync();
        }

        public async Task<long> GenerateAgendaIdAsync()
        {
            var existingAgendaIds = await _context.Agenda.Select(s => s.AgendaId).ToListAsync();
            long generatedAgendaId;
            do
            {
                generatedAgendaId = _random.NextInt64(1000000000, 9999999999);

            } while (existingAgendaIds.Contains(generatedAgendaId));

            return generatedAgendaId;
        }

        public async Task<List<Agenda>> GetStudentClassAgendasAsync(long studentClassId)
        {
            var agendas = await _context.Agenda
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .Include(s => s.StudentClass)
                .Where(a => a.StudentClassId == studentClassId)
                .ToListAsync();

            return agendas;
        }

        public async Task<Agenda> GetAgendaByAgendaIdAsync(long agendaId)
        {
            return await _context.Agenda
                .Where(a => a.AgendaId == agendaId)
                .Include(a => a.Subject) // Include the related Subject entity
                .Include(a => a.Teacher)
                .Select(a => new Agenda
                {
                    AgendaId = a.AgendaId,
                    StartDateTime = a.StartDateTime,
                    EndDateTime = a.EndDateTime,
                    StudentClassId = a.StudentClassId,
                    SubjectDto = new SubjectAgendaDto // Projecting the Subject entity to SubjectAgendaDto
                    {
                        SubjectId = a.Subject.SubjectId,
                        SubjectName = a.Subject.SubjectName
                    },
                })
                .FirstOrDefaultAsync() ?? new Agenda();
        }

        public async Task<List<Agenda>> GetAgendasForClassInRangeAsync(int studentClassId, DateTime startDate, DateTime endDate)
        {
            return await _context.Agenda
                .Where(a => a.StudentClassId == studentClassId &&
                            a.StartDateTime >= startDate &&
                            a.EndDateTime <= endDate)
                .Include(a => a.Subject) // Include the related Subject entity
                .Select(a => new Agenda
                {
                    AgendaId = a.AgendaId,
                    StartDateTime = a.StartDateTime,
                    EndDateTime = a.EndDateTime,
                    StudentClassId = a.StudentClassId,
                    SubjectDto = new SubjectAgendaDto // Projecting the Subject entity to SubjectAgendaDto
                    {
                        SubjectId = a.Subject.SubjectId,
                        SubjectName = a.Subject.SubjectName
                    },
                })
                .ToListAsync();
        }

        public async Task<List<Agenda>> GetTeacherAgendasInRangeAsync(long teacherId, DateTime startDate, DateTime endDate)
        {
            Console.WriteLine($"TeacherID: {teacherId}");
            Console.WriteLine($"StartDate: {startDate}");
            Console.WriteLine($"EndDate: {endDate}");
            return await _context.Agenda
               .Where(a => a.TeacherId == teacherId &&
                           a.StartDateTime >= startDate &&
                           a.EndDateTime <= endDate)
               .Include(a => a.Subject) // Include the related Subject entity
               .Select(a => new Agenda
               {
                   AgendaId = a.AgendaId,
                   StartDateTime = a.StartDateTime,
                   EndDateTime = a.EndDateTime,
                   StudentClassId = a.StudentClassId,
                   SubjectDto = new SubjectAgendaDto // Projecting the Subject entity to SubjectAgendaDto
                   {
                       SubjectId = a.Subject.SubjectId,
                       SubjectName = a.Subject.SubjectName
                   },
               })
               .ToListAsync();
        }


    }
}
