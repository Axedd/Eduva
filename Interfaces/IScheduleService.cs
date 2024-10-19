using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IScheduleService
    {
        Task<List<ScheduleModulePreferences>> GetScheduleModulePreferencesAsync();
        Task<(int WeekNumber, List<DateTime> WeekDays)> GetCurrentWeekAsync(int? week);
        Task<List<Agenda>> GetAgendasForWeekAsync(int studentClassId, int? weekNum);
        Task<List<Agenda>> GetTeacherAgendasAsync(long teacherId, int? weekNum);

        Dictionary<string, List<Agenda>> GroupAgendasByDay(List<Agenda> agendas);
        Task<int> GetGlobalEarliestStartTime(List<Agenda> agendas);
        int GetGlobalLatestEndTime(List<Agenda> agendas);
        Dictionary<string, List<List<Agenda>>> FindOverlappingEvents(List<Agenda> agendas);
    }
}
