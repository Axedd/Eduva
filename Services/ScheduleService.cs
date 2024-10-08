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

        public ScheduleService(ApplicationDbContext context, IAgendaService agendaService)
        {
            _context = context;
            _agendaService = agendaService;
        }

        public async Task<List<ScheduleModulePreferences>> GetScheduleModulePreferencesAsync()
        {
            return await _context.ScheduleModulePreferences.ToListAsync();
        }

        public async Task<(int WeekNumber, List<DateTime> WeekDays)> GetCurrentWeekAsync()
        {
            DateTime today = DateTime.Today;
            int currentWeekNumber = GetWeekNumberOfYear(today);

            List<DateTime> currentWeekDays = GetCurrentWeekDays();

            // Return both the week number and the list of weekdays
            return (40, currentWeekDays);
        }

        public List<Agenda> Agendas { get; set; }

        public async Task<List<Agenda>> GetAgendasForWeekAsync(int studentClassId, int? weekNum)
        {
            DateTime weekStart;
            DateTime weekEnd;

            if (weekNum.HasValue && weekNum.Value > 0)
            {
                // Calculate the start date for the given week number
                DateTime yearStart = new DateTime(DateTime.Today.Year, 1, 1);
                int daysOffset = (weekNum.Value - 1) * 7; // Calculate days to add for the week number
                weekStart = yearStart.AddDays(daysOffset);

                // Adjust to the nearest Monday
                int dayOfWeek = (int)weekStart.DayOfWeek;
                if (dayOfWeek != 0) // 0 = Sunday
                {
                    weekStart = weekStart.AddDays(-(dayOfWeek - 1)); // Move back to Monday
                }
            }
            else
            {
                // If weekNum is 0 or null, calculate the current week based on today's date
                weekStart = GetStartOfCurrentWeek();
            }

            // Calculate the end date for the week
            weekEnd = weekStart.AddDays(6);

            // Retrieve agendas for the specified week
            Agendas = await _agendaService.GetAgendasForClassInRangeAsync(studentClassId, weekStart, weekEnd);
            return Agendas;
        }

        private DateTime GetStartOfCurrentWeek()
        {
            DateTime today = DateTime.Today;

            // Get the difference between today and the previous Monday
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = today.AddDays(-1 * diff); // Monday of the current week

            return startOfWeek;
        }

        private static List<DateTime> GetCurrentWeekDays()
        {
            List<DateTime> weekDays = new List<DateTime>();
            DateTime today = new DateTime(2024, 10, 5);

            // Get the difference between today and the previous Monday
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = today.AddDays(-1 * diff); // Monday of the current week

            for (int i = 0; i < 7; i++)
            {
                weekDays.Add(startOfWeek.AddDays(i));
            }

            return weekDays;
        }

        private static int GetWeekNumberOfYear(DateTime date)
        {
            // Get the current culture (could be modified for a specific culture)
            CultureInfo culture = CultureInfo.CurrentCulture;
            Calendar calendar = culture.Calendar;

            // Define that the week starts on Monday, and uses the first week that contains a Thursday as the first week
            CalendarWeekRule rule = culture.DateTimeFormat.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;

            // Get the week number for the provided date
            int weekNumber = calendar.GetWeekOfYear(date, rule, firstDayOfWeek);

            return weekNumber;
        }
    }
}