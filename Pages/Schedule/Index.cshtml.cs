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
        private readonly IScheduleService _scheduleService;
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly ITeacherService _teacherService;
        public IndexModel(
            IScheduleService scheduleService, 
            IStudentService studentService, 
            IUserService userService,
            ITeacherService teacherService,
            ILogger<BaseService> logger) : base(logger)
        {
            _scheduleService = scheduleService;
            _studentService = studentService;
            _userService = userService;
            _teacherService = teacherService;
        }

        public List<Agenda> Agendas { get; set; } = new List<Agenda>();
        public List<DateTime> Week { get; set; }
        public int WeekNum { get; set; }
        public Dictionary<string, List<Agenda>> AgendasByDay { get; set; }
        public int GlobalEarliestStartTime { get; set; }
        public int GlobalLatestEndTime { get; set; }
        public Dictionary<string, List<List<Agenda>>> OverlappingGroupsByDay { get; set; }
        public List<ScheduleModulePreferences> ScheduleModulePreferences { get; set; }
        public string UserRole { get; set; }

        public long? StudentId { get; set; }
        public long? TeacherId { get; set; }


        public async Task<IActionResult> OnGetAsync(int? studentClassId, long? teacherId, int? week, long? studentId = null)
        {
            UserRole = _userService.GetUserRole();
            var userId = _userService.GetUserId();

            ViewData["StudentId"] = studentId == null ? null : studentId;
            ViewData["TeacherId"] = teacherId == null ? null : teacherId;

            StudentId = studentId;
            TeacherId = teacherId;


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
                AgendasByDay = _scheduleService.GroupAgendasByDay(Agendas);
                GlobalEarliestStartTime = await _scheduleService.GetGlobalEarliestStartTime(Agendas);
                GlobalLatestEndTime = _scheduleService.GetGlobalLatestEndTime(Agendas);
                OverlappingGroupsByDay = _scheduleService.FindOverlappingEvents(Agendas);
            }
            else
            {
                GlobalEarliestStartTime = await _scheduleService.GetGlobalEarliestStartTime(Agendas);
                GlobalLatestEndTime = 1024; // Default value if no agendas
            }

            Week = weekDays;
            WeekNum = weekNum;

            return Page();
        }    
    }
}