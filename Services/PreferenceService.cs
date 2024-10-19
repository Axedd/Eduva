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

        public async Task UpdateScheduleModulePreferenceAsync(ScheduleModulePreferences preference)
        {
            // Check if the preference exists
            var existingPreference = await _context.ScheduleModulePreferences
                .FirstOrDefaultAsync(p => p.Id == preference.Id);

            if (existingPreference != null)
            {
                await _context.ScheduleModulePreferences
                    .Where(p => p.Id == preference.Id) 
                    .ExecuteUpdateAsync(setter => setter
                        .SetProperty(p => p.ModuleNumber, preference.ModuleNumber)
                        .SetProperty(p => p.StartTime, preference.StartTime) 
                        .SetProperty(p => p.EndTime, preference.EndTime)); 
            }
        }

        public async Task DeleteScheduleModulePreferenceAsync(int id)
        {
            _context.Remove(_context.ScheduleModulePreferences.Single(smp => smp.Id == id));

            await _context.SaveChangesAsync();
        }

    }
}
