using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IAgendaService
    {

        Task AddAgendaAsync(Agenda agenda);
        Task<long> GenerateAgendaIdAsync();
        Task<List<Agenda>> GetStudentClassAgendasAsync(long studentClassId);
        Task<List<Agenda>> GetAgendasForClassInRangeAsync(int studentClassId, DateTime startDate, DateTime endDate);
        Task<Agenda> GetAgendaByAgendaIdAsync(long agendaId);

    }
}
