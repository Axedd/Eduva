using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IScheduleService
    {
        Task<(int WeekNumber, List<DateTime> WeekDays)> GetCurrentWeekAsync();
        Task<List<Agenda>> GetAgendasForWeekAsync(int studentClassId, int? weekNum);
    }
}
