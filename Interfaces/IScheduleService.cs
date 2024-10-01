namespace SchoolSystem.Interfaces
{
    public interface IScheduleService
    {
        Task<(int WeekNumber, List<DateTime> WeekDays)> GetCurrentWeekAsync();
    }
}
