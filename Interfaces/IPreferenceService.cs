using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IPreferenceService
    {
        Task<List<ScheduleModulePreferences>> GetScheduleModulePreferencesAsync();
        Task AddScheduleModulePreferenceAsync(ScheduleModulePreferences preference);
        Task DeleteScheduleModulePreferenceAsync(int id);
    }
}
