using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchoolSystem.Configurations;
using SchoolSystem.Data;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using System;
using System.Linq;

namespace SchoolSystem.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly ApplicationDbContext _context;
        private Random _random;
        private readonly ILogger<AgendaService> _logger;

        private readonly string _tinyMceApiKey;
        public AgendaService(ApplicationDbContext context, Random random, ILogger<AgendaService> logger, IOptions<TinyMceSettings> tinyMceSettings)
        {
            _context = context;
            _random = random;
            _logger = logger;
            _tinyMceApiKey = tinyMceSettings.Value.ApiKey;
        }

        public string GetTinyMceApiKey()
        {
            return _tinyMceApiKey;  // Only exposed within the service
        }


        public async Task AddAgendaAsync(Agenda agenda)
        {
            try
            {
                await _context.Agenda.AddAsync(agenda);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error adding agenda with ID {AgendaId}", agenda.AgendaId);
                throw new Exception("Failed to add agenda. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding agenda with ID {AgendaId}", agenda.AgendaId);
                throw new Exception("An unexpected error occurred. Please try again.");
            }
        }

        public async Task<long> GenerateAgendaIdAsync()
        {
            try
            {
                var existingAgendaIds = await _context.Agenda.Select(s => s.AgendaId).ToListAsync();
                long generatedAgendaId;
                do
                {
                    generatedAgendaId = _random.NextInt64(1000000000, 9999999999);
                } while (existingAgendaIds.Contains(generatedAgendaId));

                return generatedAgendaId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error generating agenda ID.");
                throw new Exception("Failed to generate agenda ID. Please try again.");
            }
        }

        public async Task<List<Agenda>> GetStudentClassAgendasAsync(long studentClassId)
        {
            try
            {
                return await _context.Agenda
                    .Include(s => s.Subject)
                    .Include(s => s.Teacher)
                    .Include(s => s.StudentClass)
                    .Where(a => a.StudentClassId == studentClassId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching agendas for StudentClassId {StudentClassId}.", studentClassId);
                throw new Exception("Failed to retrieve agendas. Please try again.");
            }
        }

        public async Task<Agenda> GetAgendaByAgendaIdAsync(long agendaId)
        {
            try
            {
                return await _context.Agenda
                    .Where(a => a.AgendaId == agendaId)
                    .Include(a => a.Subject)
                    .Include(a => a.Teacher)
                    .Select(a => new Agenda
                    {
                        AgendaId = a.AgendaId,
                        StartDateTime = a.StartDateTime,
                        EndDateTime = a.EndDateTime,
                        StudentClassId = a.StudentClassId,
                        TeacherId = a.TeacherId,
                        Note = a.Note,
                        HomeWork = a.HomeWork,
                        SubjectDto = new SubjectAgendaDto
                        {
                            SubjectId = a.Subject.SubjectId,
                            SubjectName = a.Subject.SubjectName
                        },
                        StudentClassDto = new StudentClassDto
                        {
                            StudentClassId = a.StudentClassId,
                            ClassName = a.StudentClass.ClassName
                        }
                    })
                    .FirstOrDefaultAsync() ?? new Agenda();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching agenda with ID {AgendaId}.", agendaId);
                throw new Exception("Failed to retrieve agenda. Please try again.");
            }
        }

        public async Task<List<Agenda>> GetAgendasForClassInRangeAsync(int studentClassId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.Agenda
                    .Where(a => a.StudentClassId == studentClassId &&
                                a.StartDateTime >= startDate &&
                                a.EndDateTime <= endDate)
                    .Include(a => a.Subject)
                    .Select(a => new Agenda
                    {
                        AgendaId = a.AgendaId,
                        StartDateTime = a.StartDateTime,
                        EndDateTime = a.EndDateTime,
                        StudentClassId = a.StudentClassId,
                        SubjectDto = new SubjectAgendaDto
                        {
                            SubjectId = a.Subject.SubjectId,
                            SubjectName = a.Subject.SubjectName
                        }
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching agendas for StudentClassId {StudentClassId} in range {StartDate} - {EndDate}.", studentClassId, startDate, endDate);
                throw new Exception("Failed to retrieve agendas. Please try again.");
            }
        }

        public async Task<List<Agenda>> GetTeacherAgendasInRangeAsync(long teacherId, DateTime startDate, DateTime endDate)
        {
            try
            {
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
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error fetching teacher agendas for TeacherId {TeacherId} between {StartDate} and {EndDate}.", teacherId, startDate, endDate);
                throw new Exception("There was a problem retrieving the teacher's agendas. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching teacher agendas for TeacherId {TeacherId} between {StartDate} and {EndDate}.", teacherId, startDate, endDate);
                throw new Exception("An unexpected error occurred. Please try again.");
            }
        }

        public async Task UpdateAgendaAsync(long agendaId, Agenda agendaDetails)
        {
            try
            {
                var agenda = await _context.Agenda.FindAsync(agendaId);
                if (agenda == null)
                {
                    throw new Exception($"Agenda with ID {agendaId} not found.");
                }

                await _context.Agenda
                       .Where(a => a.AgendaId == agendaId)
                       .ExecuteUpdateAsync(setter => setter
                           .SetProperty(p => p.Note, agendaDetails.Note)
                           .SetProperty(p => p.HomeWork, agendaDetails.HomeWork));
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update-related errors (e.g., constraint violations)
                _logger.LogError(dbEx, "An error occurred while updating the agenda with ID {AgendaId}.", agendaId);
                throw new Exception("There was a problem updating the agenda. Please try again later.");
            }
            catch (Exception ex)
            {
                // Handle general errors
                _logger.LogError(ex, "An unexpected error occurred while updating the agenda with ID {AgendaId}.", agendaId);
                throw new Exception("An unexpected error occurred. Please try again.");
            }
        }

        public async Task<bool> HasWeekendAgendas(List<DateTime> weekendDays)
        {
            foreach (var day in weekendDays)
            {
                if (await _context.Agenda.AnyAsync(a => a.StartDateTime.Date == day.Date))
                {
                    return true;
                }
            }

            return false;
        }

        public string SanitizeAgenda(string textToSanitize)
        {
            var sanitizer = new Ganss.Xss.HtmlSanitizer();
            return sanitizer.Sanitize(textToSanitize);
        }
    }
}
