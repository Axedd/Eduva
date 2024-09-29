using SchoolSystem.Data;
using SchoolSystem.Models;

namespace SchoolSystem.Services
{
    public class AgendaService
    {
        private readonly ApplicationDbContext _context;
        public AgendaService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task AddAgenda(Agenda agenda)
        {


        }

    }
}
