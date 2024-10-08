using Microsoft.EntityFrameworkCore;
using SchoolSystem.Data;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;

namespace SchoolSystem.Services
{
    public class PreferenceService : IPreferenceService
    {
        private readonly ApplicationDbContext _context;
        public PreferenceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScheduleModulePreferences>> GetScheduleModulePreferencesAsync()
        {
            return await _context.ScheduleModulePreferences.OrderBy(smp => smp.StartTime).ToListAsync();
        }

        public async Task AddScheduleModulePreferenceAsync(ScheduleModulePreferences preference)
        {
            _context.ScheduleModulePreferences.Add(preference);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteScheduleModulePreferenceAsync(int id)
        {
            _context.Remove(_context.ScheduleModulePreferences.Single(smp => smp.Id == id));

            await _context.SaveChangesAsync();
        }
    }
}
