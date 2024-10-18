using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using SchoolSystem.Pages.Shared;
using System;

namespace SchoolSystem.Pages.Schedule
{

    public class IndexModel : BaseService
    {
        private readonly IAgendaService _agendaService;
        private readonly IScheduleService _scheduleService;
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly ITeacherService _teacherService;
        private readonly IIdValidationService _idValidationService;
        private readonly IStudentClassService _studentClassService;
        public IndexModel(IAgendaService agendaService, 
            IScheduleService scheduleService, 
            IStudentService studentService, 
            IUserService userService,
            ITeacherService teacherService,
            IConfiguration configuration,
            IIdValidationService idValidationService,
            IStudentClassService studentClassService,
            ILogger<BaseService> logger) : base(logger)
        {
            _agendaService = agendaService;
            _scheduleService = scheduleService;
            _studentService = studentService;
            _userService = userService;
            _teacherService = teacherService;
            _idValidationService = idValidationService;
            _studentClassService = studentClassService;
        }

        public List<Agenda> Agendas { get; set; } = new List<Agenda>();
        public List<DateTime> Week { get; set; }
        public int WeekNum { get; set; }
        public Dictionary<string, List<Agenda>> AgendasByDay { get; set; }
        public int GlobalEarliestStartTime { get; set; }
        public int GlobalLatestEndTime { get; set; }
        public Dictionary<string, List<List<Agenda>>> OverlappingGroupsByDay { get; set; }
        public List<ScheduleModulePreferences> ScheduleModulePreferences { get; set; }
        public Agenda AgendaDetails { get; set; }
        public string UserRole { get; set; }
        public bool IsAgendaTeacher { get; set; }
       
        public bool EditMode { get; set; }
        [BindProperty]
        public Agenda AgendaUpdate { get; set; }

        public bool ShowStudentList {  get; set; }



        public async Task<IActionResult> OnGetAsync(int? studentClassId, long? teacherId, int? week, long? studentId = null)
        {
            UserRole = _userService.GetUserRole();
            var userId = _userService.GetUserId();

            ViewData["StudentId"] = studentId == null ? null : studentId;
            ViewData["TeacherId"] = teacherId == null ? null : teacherId;

            // Attempt to retrieve studentClassId based on provided parameters
            if (!studentClassId.HasValue)
            {
                studentClassId = studentId.HasValue
                    ? await _studentService.GetClassIdOfStudentAsync(studentId: studentId.Value)
                    : (UserRole != "Teacher" ? await _studentService.GetClassIdOfStudentAsync(userId: userId) : (int?)null);
            }

            // Get current week and agendas
            var (weekNum, weekDays) = await _scheduleService.GetCurrentWeekAsync(week);
            week ??= weekNum; // If week is null, use the current week number

            if (UserRole == "Teacher" || teacherId.HasValue)
            {
                teacherId ??= await _teacherService.GetTeacherByUserId(userId);

                if (teacherId.HasValue)
                {
                    Agendas = await _scheduleService.GetTeacherAgendasAsync(teacherId.Value, week);
                }
            }
            else if (UserRole == "Student" || UserRole == "Admin") // Admin = just for development enviorment
            {
                if (studentClassId.HasValue)
                {
                    Agendas = await _scheduleService.GetAgendasForWeekAsync(studentClassId.Value, week);
                }
            }

            ScheduleModulePreferences = await _scheduleService.GetScheduleModulePreferencesAsync();

            if (Agendas.Count > 0)
            {
                // Group agendas by day and find times
                AgendasByDay = GroupAgendasByDay(Agendas);
                GlobalEarliestStartTime = await FindGlobalEarliestStartTime(Agendas);
                GlobalLatestEndTime = FindGlobalLatestEndTime(Agendas);
                OverlappingGroupsByDay = new Dictionary<string, List<List<Agenda>>>();

                foreach (var day in AgendasByDay.Keys)
                {
                    OverlappingGroupsByDay[day] = FindOverlappingGroups(AgendasByDay[day]);
                }
            }
            else
            {
                GlobalEarliestStartTime = await FindGlobalEarliestStartTime();
                GlobalLatestEndTime = 1024; // Default value if no agendas
            }

            Week = weekDays;
            WeekNum = weekNum;

            return Page();
        }



        private Dictionary<string, List<Agenda>> GroupAgendasByDay(List<Agenda> agendas)
        {
            var agendasByDay = new Dictionary<string, List<Agenda>>();

            foreach (var agenda in agendas)
            {
                var date = agenda.StartDateTime.Date.ToString("yyyy-MM-dd");
                if (!agendasByDay.ContainsKey(date))
                {
                    agendasByDay[date] = new List<Agenda>();
                }
                agendasByDay[date].Add(agenda);
            }

            return agendasByDay;
        }

        private async Task<int> FindGlobalEarliestStartTime(List<Agenda>? agendas = null)
        {
            // Get schedule preferences from the service
            ScheduleModulePreferences = await _scheduleService.GetScheduleModulePreferencesAsync();

            int earliestAgendaStartTime;

            // If 'agendas' is null or empty, use default value from ScheduleModulePreferences
            if (agendas == null || !agendas.Any())
            {
                // Use default start time from ScheduleModulePreferences
                earliestAgendaStartTime = ScheduleModulePreferences[0].StartTime.Hour * 60 + ScheduleModulePreferences[0].StartTime.Minute;
                Console.WriteLine(earliestAgendaStartTime);
            }
            else
            {
                // Calculate the earliest agenda start time from the provided agendas
                earliestAgendaStartTime = agendas.Min(a => a.StartDateTime.Hour * 60 + a.StartDateTime.Minute);

                // Compare with preferences and find the earliest start time
                foreach (var preference in ScheduleModulePreferences)
                {
                    int preferenceMinutes = preference.StartTime.Hour * 60 + preference.StartTime.Minute;

                    if (earliestAgendaStartTime > preferenceMinutes)
                    {
                        earliestAgendaStartTime = preferenceMinutes;
                    }
                }
            }

            return earliestAgendaStartTime;
        }

        private int FindGlobalLatestEndTime(List<Agenda> agendas)
        {
            return agendas.Max(a => a.EndDateTime.Hour * 60 + a.EndDateTime.Minute);
        }

        private List<List<Agenda>> FindOverlappingGroups(List<Agenda> dayAgendas)
        {
            var overlappingGroups = new List<List<Agenda>>();
            dayAgendas = dayAgendas.OrderBy(a => a.StartDateTime).ToList();

            var currentGroup = new List<Agenda> { dayAgendas[0] };
            for (int i = 1; i < dayAgendas.Count; i++)
            {
                var prevEndTime = dayAgendas[i - 1].EndDateTime;
                var currentStartTime = dayAgendas[i].StartDateTime;

                if (currentStartTime < prevEndTime)
                {
                    currentGroup.Add(dayAgendas[i]);
                }
                else
                {
                    overlappingGroups.Add(currentGroup);
                    currentGroup = new List<Agenda> { dayAgendas[i] };
                }
            }

            overlappingGroups.Add(currentGroup);
            return overlappingGroups;
        }
    }
}