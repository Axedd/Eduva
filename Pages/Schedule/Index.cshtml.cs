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
        public IndexModel(IAgendaService agendaService, IScheduleService scheduleService)
        {
            _agendaService = agendaService;
            _scheduleService = scheduleService;
        }

        public List<Agenda> Agendas { get; set; }
        public List<DateTime> Week { get; set; }
        public int WeekNum { get; set; }
        public Dictionary<string, List<Agenda>> AgendasByDay { get; set; }
        public int GlobalEarliestStartTime { get; set; }
        public int GlobalLatestEndTime { get; set; }
        public Dictionary<string, List<List<Agenda>>> OverlappingGroupsByDay { get; set; }
        public List<ScheduleModulePreferences> ScheduleModulePreferences { get; set; }

        public async Task<IActionResult> OnGetAsync(int studentClassId, int? week)
        {
            // Get current week and agendas
            var (weekNum, weekDays) = await _scheduleService.GetCurrentWeekAsync();
            Agendas = await _scheduleService.GetAgendasForWeekAsync(studentClassId, week);

            ScheduleModulePreferences = await _scheduleService.GetScheduleModulePreferencesAsync();

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

        private async Task<int> FindGlobalEarliestStartTime(List<Agenda> agendas)
        {
            ScheduleModulePreferences = await _scheduleService.GetScheduleModulePreferencesAsync();
            int earliestAgendaStartTime = agendas.Min(a => a.StartDateTime.Hour * 60 + a.StartDateTime.Minute); 
            foreach (var preference in ScheduleModulePreferences)
            {
                int preferenceMinutes = preference.StartTime.Hour * 60 + preference.StartTime.Minute;

                if (earliestAgendaStartTime > preferenceMinutes)
                {
                    earliestAgendaStartTime = preferenceMinutes;
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