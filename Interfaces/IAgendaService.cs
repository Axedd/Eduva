using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IAgendaService
    {

        Task AddAgendaAsync(Agenda agenda);
        Task<long> GenerateAgendaIdAsync();
        Task<List<Agenda>> GetStudentClassAgendasAsync(long studentClassId);
        Task<List<Agenda>> GetAgendasForClassInRangeAsync(int studentClassId, DateTime startDate, DateTime endDate);
        Task<List<Agenda>> GetTeacherAgendasInRangeAsync(long teacherId, DateTime startDate, DateTime endDate);
        Task<Agenda> GetAgendaByAgendaIdAsync(long agendaId);

        Task UpdateAgendaAsync(long agendaId, Agenda agendaDetails);
        string SanitizeAgenda(string textToSanitize);

        string GetTinyMceApiKey();

        Task<bool> HasWeekendAgendas(List<DateTime> weekendDays);
    }
}
