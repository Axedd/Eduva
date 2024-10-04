using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;

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
        public List<DateTime> Week {  get; set; }
        public int WeekNum { get; set; }


        public async Task<IActionResult> OnGetAsync(int studentClassId, int? week)
        {
            //Agendas = await _agendaService.GetStudentClassAgendasAsync(studentClassId);
            var (weekNum, weekDays) = await _scheduleService.GetCurrentWeekAsync();

            // Retrieve agendas for the current week
            Agendas = await _scheduleService.GetAgendasForWeekAsync(studentClassId, week);


            Week = weekDays;
            WeekNum = weekNum;


            return Page();
        }

    }
}
