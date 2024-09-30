﻿using Microsoft.EntityFrameworkCore;
using SchoolSystem.Data;
using SchoolSystem.Interfaces;
using SchoolSystem.Models;
using System;

namespace SchoolSystem.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly ApplicationDbContext _context;
        private Random _random;
        public AgendaService(ApplicationDbContext context, Random random)
        {
            _context = context;
            _random = random;
        }


        public async Task AddAgendaAsync(Agenda agenda)
        {
            await _context.Agenda.AddAsync(agenda);
            await _context.SaveChangesAsync();
        }

        public async Task<long> GenerateAgendaIdAsync()
        {
            var existingAgendaIds = await _context.Agenda.Select(s => s.AgendaId).ToListAsync();
            long generatedAgendaId;
            do
            {
                generatedAgendaId = _random.NextInt64(1000000000, 9999999999);

            } while (existingAgendaIds.Contains(generatedAgendaId));

            return generatedAgendaId;
        }


    }
}
