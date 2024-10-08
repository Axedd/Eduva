using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;

namespace SchoolSystem.Pages.Admin
{
    public class PreferencesModel : PageModel
    {
        private readonly IPreferenceService _preferenceService;

        public PreferencesModel(IPreferenceService preferenceService)
        {
            _preferenceService = preferenceService;
        }

        [BindProperty]
        public ScheduleModulePreferences ScheduleModulePreference { get; set; }

        public List<ScheduleModulePreferences> ScheduleModulePreferences { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ScheduleModulePreferences = await _preferenceService.GetScheduleModulePreferencesAsync();


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _preferenceService.AddScheduleModulePreferenceAsync(ScheduleModulePreference);


            ScheduleModulePreferences = await _preferenceService.GetScheduleModulePreferencesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteScheduleModulePreferenceAsync(int id)
        {
            await _preferenceService.DeleteScheduleModulePreferenceAsync(id);

            ScheduleModulePreferences = await _preferenceService.GetScheduleModulePreferencesAsync();

            return RedirectToPage();
        }
    }
}
