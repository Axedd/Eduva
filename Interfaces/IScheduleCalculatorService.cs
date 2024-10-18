namespace SchoolSystem.Interfaces
{
    public interface IScheduleCalculatorService
    {
        (DateTime weekStart, DateTime weekEnd) GetWeekDays(int? weekNum);
        DateTime GetStartOfCurrentWeek();
        List<DateTime> GetCurrentWeekDays(int? weekNum);
        int GetWeekNumberOfYear(DateTime date);
    }
}