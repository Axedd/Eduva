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


        public async Task<IActionResult> OnGetAsync(long studentClassId)
        {
            //Agendas = await _agendaService.GetStudentClassAgendasAsync(studentClassId);
            var (weekNum, week) = await _scheduleService.GetCurrentWeekAsync();

            Week = week;
            WeekNum = weekNum;


            return Page();
        }
    }
}
