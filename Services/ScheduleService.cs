using Microsoft.EntityFrameworkCore;
using SchoolSystem.Data;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using System.Globalization;

namespace SchoolSystem.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAgendaService _agendaService;
        private readonly IScheduleCalculatorService _scheduleCalculatorService;

        public ScheduleService(ApplicationDbContext context, IAgendaService agendaService, IScheduleCalculatorService scheduleCalculatorService)
        {
            _context = context;
            _agendaService = agendaService;
            _scheduleCalculatorService = scheduleCalculatorService;
        }

        public async Task<List<ScheduleModulePreferences>> GetScheduleModulePreferencesAsync()
        {
            return await _context.ScheduleModulePreferences.OrderBy(smp => smp.StartTime).ToListAsync();
        }

        public List<Agenda> Agendas { get; set; }

        public async Task<List<Agenda>> GetAgendasForWeekAsync(int studentClassId, int? weekNum)
        {
            var (weekStart, weekEnd) = _scheduleCalculatorService.GetWeekDays(weekNum);

            // Retrieve studentclass agendas for the specified week
            return await _agendaService.GetAgendasForClassInRangeAsync(studentClassId, weekStart, weekEnd);
        }

        public async Task<List<Agenda>> GetTeacherAgendasAsync(long teacherId, int? weekNum)
        {
            var (weekStart, weekEnd) = _scheduleCalculatorService.GetWeekDays(weekNum);

            // Retrieve teacher agendas for the specified week
            return await _agendaService.GetTeacherAgendasInRangeAsync(teacherId, weekStart, weekEnd);
        }

        public async Task<(int WeekNumber, List<DateTime> WeekDays)> GetCurrentWeekAsync(int? weekNum)
        {
            DateTime today = DateTime.Today;
            int currentWeekNumber = 0;

            if (weekNum != null)
            {
                currentWeekNumber = weekNum.Value;
            }
            else
            {
                currentWeekNumber = _scheduleCalculatorService.GetWeekNumberOfYear(today);
            }

            List<DateTime> currentWeekDays = _scheduleCalculatorService.GetCurrentWeekDays(weekNum);

            // Return both the week number and the list of weekdays
            return (currentWeekNumber, currentWeekDays);
        }

    }
}