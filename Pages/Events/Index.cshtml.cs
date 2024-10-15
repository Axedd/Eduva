using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using SchoolSystem.Services;

namespace SchoolSystem.Pages.Events
{
    public class IndexModel : PageModel
    {
        private readonly IAgendaService _agendaService;
        private readonly IIdValidationService _idValidationService;
        private readonly IUserService _userService;
        private readonly ITeacherService _teacherService;

        public IndexModel(IAgendaService agendaService, 
            IIdValidationService idValidationService, 
            IUserService userService, 
            ITeacherService teacherService,
            IConfiguration configuration)
        {
            _agendaService = agendaService;
            _idValidationService = idValidationService;
            _userService = userService;
            _teacherService = teacherService;
        }


        public Agenda AgendaDetails { get; set; }
        public string UserRole { get; set; }
        public bool IsAgendaTeacher { get; set; }
        public bool EditMode { get; set; }
        [BindProperty]
        public Agenda AgendaUpdate { get; set; }
        public string TinyMceApiKey { get; private set; }
        public bool ShowStudentList { get; set; }

        public async Task<IActionResult> OnGetAsync(long agendaId, bool? editmode)
        {
            UserRole = _userService.GetUserRole();
            TinyMceApiKey = _agendaService.GetTinyMceApiKey();


            if (!await _idValidationService.IsValidAgendaId(agendaId))
            {
                return RedirectToPage("/Errors/NotFound"); // Redirect to the custom 404 page
            }

            AgendaDetails = await _agendaService.GetAgendaByAgendaIdAsync(agendaId);

            var UserId = _userService.GetUserId();
            long TeacherId = await _teacherService.GetTeacherByUserId(UserId);

            EditMode = editmode.GetValueOrDefault(false);

            IsAgendaTeacher = AgendaDetails.TeacherId == TeacherId;

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
                AgendaUpdate.Note = _agendaService.SanitizeAgenda(AgendaUpdate.Note);
                AgendaUpdate.HomeWork = _agendaService.SanitizeAgenda(AgendaUpdate.HomeWork);
            }

            await _agendaService.UpdateAgendaAsync(AgendaUpdate.AgendaId, AgendaUpdate);


            return RedirectToPage(new { agendaId = AgendaUpdate.AgendaId });
        }
    }
}
