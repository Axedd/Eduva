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







        public async Task<(int WeekNumber, List<DateTime> WeekDays)> GetCurrentWeekAsync()
        {
            DateTime today = DateTime.Today;
            int currentWeekNumber = GetWeekNumberOfYear(today);

            List<DateTime> currentWeekDays = GetCurrentWeekDays();

            // Return both the week number and the list of weekdays
            return (currentWeekNumber, currentWeekDays);
        }

        public List<Agenda> Agendas { get; set; }

        /*
        public async Task GetCurrentWeekAgendasAsync(int studentClassId)
        {
            Agendas = await _agendaService.GetStudentClassAgendasAsync(studentClassId);



        }
        */


        private static List<DateTime> GetCurrentWeekDays()
        {
            List<DateTime> weekDays = new List<DateTime>();
            DateTime today = DateTime.Today;

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
