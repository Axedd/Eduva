using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IScheduleService
    {
        Task<List<ScheduleModulePreferences>> GetScheduleModulePreferencesAsync();
        Task<(int WeekNumber, List<DateTime> WeekDays)> GetCurrentWeekAsync(int? week);
        Task<List<Agenda>> GetAgendasForWeekAsync(int studentClassId, int? weekNum);
    }
}
