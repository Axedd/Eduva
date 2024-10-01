using SchoolSystem.Models;

namespace SchoolSystem.Interfaces
{
    public interface IAgendaService
    {

        Task AddAgendaAsync(Agenda agenda);
        Task<long> GenerateAgendaIdAsync();
        Task<List<Agenda>> GetStudentClassAgendasAsync(long studentClassId);

    }
}
