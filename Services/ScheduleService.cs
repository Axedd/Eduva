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
            int currentWeekNumber = weekNum ?? _scheduleCalculatorService.GetWeekNumberOfYear(today);

            List<DateTime> currentWeekDays = _scheduleCalculatorService.GetCurrentWeekDays(currentWeekNumber);

            // Check for weekend agendas and remove weekends if none found
            currentWeekDays = await RemoveWeekendsIfNoAgendas(currentWeekDays);

            return (currentWeekNumber, currentWeekDays);
        }

        public async Task<List<DateTime>> RemoveWeekendsIfNoAgendas(List<DateTime> weekDays)
        {
            var weekendDays = GetWeekendDays(weekDays);

            // If there are no agendas on weekend days, remove them from the list
            if (!await _agendaService.HasWeekendAgendas(weekendDays))
            {
                return weekDays.Except(weekendDays).ToList();
            }

            return weekDays;
        }

        public Dictionary<string, List<Agenda>> GroupAgendasByDay(List<Agenda> agendas)
        {
            return _scheduleCalculatorService.GroupAgendasByDay(agendas);
        }

        public async Task<int> GetGlobalEarliestStartTime(List<Agenda> agendas)
        {
            return await _scheduleCalculatorService.FindGlobalEarliestStartTime(agendas);
        }

        public int GetGlobalLatestEndTime(List<Agenda> agendas)
        {
            return _scheduleCalculatorService.FindGlobalLatestEndTime(agendas);
        }


        public Dictionary<string, List<List<Agenda>>> FindOverlappingEvents(List<Agenda> agendas)
        {
            var AgendasByDay = GroupAgendasByDay(agendas);

            var OverlappingGroupsByDay = new Dictionary<string, List<List<Agenda>>>();

            foreach (var day in AgendasByDay.Keys)
            {
                OverlappingGroupsByDay[day] = _scheduleCalculatorService.FindOverlappingGroups(AgendasByDay[day]);
            }
            return OverlappingGroupsByDay;
        }

        private List<DateTime> GetWeekendDays(List<DateTime> dates)
        {
            return dates.Where(date => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday).ToList();
        }

    }
}