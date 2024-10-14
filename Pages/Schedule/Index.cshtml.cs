using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using System;

namespace SchoolSystem.Pages.Schedule
{

    public class IndexModel : PageModel
    {
        private readonly IAgendaService _agendaService;
        private readonly IScheduleService _scheduleService;
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly ITeacherService _teacherService;
        private readonly IConfiguration _configuration;
        private readonly IIdValidationService _idValidationService;
        public IndexModel(IAgendaService agendaService, 
            IScheduleService scheduleService, 
            IStudentService studentService, 
            IUserService userService,
            ITeacherService teacherService,
            IConfiguration configuration,
            IIdValidationService idValidationService)
        {
            _agendaService = agendaService;
            _scheduleService = scheduleService;
            _studentService = studentService;
            _userService = userService;
            _teacherService = teacherService;
            _configuration = configuration;
            _idValidationService = idValidationService;
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

        public string TinyMceApiKey { get; private set; }


        public async Task<IActionResult> OnGetAsync(int? studentClassId, long? teacherId, int? week, long? agendaId, bool? editmode = false)
        {
            UserRole = _userService.GetUserRole();
            TinyMceApiKey = _configuration["TinyMCE:ApiKey"]!;

            if (agendaId.HasValue)
            {

                if (!await _idValidationService.IsValidAgendaId(agendaId.Value))
                {
                    return RedirectToPage("/Errors/NotFound"); // Redirect to the custom 404 page
                }

                AgendaDetails = await _agendaService.GetAgendaByAgendaIdAsync(agendaId.Value);

                var UserId = _userService.GetUserId();
                if (!teacherId.HasValue)
                {
                    teacherId = await _teacherService.GetTeacherByUserId(UserId);
                }

                EditMode = editmode.GetValueOrDefault(false);

                IsAgendaTeacher = AgendaDetails.TeacherId == teacherId;

                return Page();
            } else
            {
                if (!studentClassId.HasValue)
                {
                    studentClassId = await _studentService.GetClassIdOfStudentAsync();
                }

                // Get current week and agendas
                var (weekNum, weekDays) = await _scheduleService.GetCurrentWeekAsync(week);
                
                if (week == null)
                {
                    week = weekNum;
                }

                if (UserRole == "Teacher" || teacherId.HasValue)
                {
                    var UserId = _userService.GetUserId();
                    if (!teacherId.HasValue)
                    {
                        teacherId = await _teacherService.GetTeacherByUserId(UserId);
                    }

                    Agendas = await _scheduleService.GetTeacherAgendasAsync(teacherId.Value, week);
                } else if (UserRole == "Student" || UserRole == "Admin")
                {
                    Agendas = await _scheduleService.GetAgendasForWeekAsync(studentClassId.Value, week);
                }

                ScheduleModulePreferences = await _scheduleService.GetScheduleModulePreferencesAsync();

                if (Agendas.Count > 0)
                {
                    // Group agendas by day
                    AgendasByDay = GroupAgendasByDay(Agendas);

                    // Find the global earliest and latest times
                    GlobalEarliestStartTime = await FindGlobalEarliestStartTime(Agendas);
                    GlobalLatestEndTime = FindGlobalLatestEndTime(Agendas);

                    // Find overlapping groups for each day
                    OverlappingGroupsByDay = new Dictionary<string, List<List<Agenda>>>();
                    foreach (var day in AgendasByDay.Keys)
                    {
                        OverlappingGroupsByDay[day] = FindOverlappingGroups(AgendasByDay[day]);
                    }
                } else
                {
                    GlobalEarliestStartTime = await FindGlobalEarliestStartTime();
                    GlobalLatestEndTime = 1024;
                }

                Week = weekDays;
                WeekNum = weekNum;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAgendaAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!string.IsNullOrWhiteSpace(AgendaUpdate.Note) && !string.IsNullOrWhiteSpace(AgendaUpdate.HomeWork))
            {
                var sanitizer = new Ganss.Xss.HtmlSanitizer();
                AgendaUpdate.Note = sanitizer.Sanitize(AgendaUpdate.Note);
                AgendaUpdate.HomeWork = sanitizer.Sanitize(AgendaUpdate.HomeWork);
            }

            await _agendaService.UpdateAgendaAsync(AgendaUpdate.AgendaId, AgendaUpdate);


            return RedirectToPage(new { agendaId = AgendaUpdate.AgendaId });
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