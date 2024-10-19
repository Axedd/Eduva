using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IScheduleCalculatorService
    {
        (DateTime weekStart, DateTime weekEnd) GetWeekDays(int? weekNum);
        DateTime GetStartOfCurrentWeek();
        List<DateTime> GetCurrentWeekDays(int? weekNum);
        int GetWeekNumberOfYear(DateTime date);

        Dictionary<string, List<Agenda>> GroupAgendasByDay(List<Agenda> agendas);
        int FindGlobalLatestEndTime(List<Agenda> agendas);
        List<List<Agenda>> FindOverlappingGroups(List<Agenda> dayAgendas);
        Task<int> FindGlobalEarliestStartTime(List<Agenda>? agendas = null);
    }
}