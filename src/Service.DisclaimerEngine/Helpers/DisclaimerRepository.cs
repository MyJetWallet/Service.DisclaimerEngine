using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.DisclaimerEngine.Domain.Models;
using Service.DisclaimerEngine.Postgres;

namespace Service.DisclaimerEngine.Helpers
{
    public class DisclaimerRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ContextRepository _repository;

        public DisclaimerRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ContextRepository repository)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _repository = repository;
        }

        public async Task<List<Disclaimer>> GetDisclaimersForUser(string clientId)
        {
            var disclaimerIds = await _repository.GetDisclaimerIdsWithoutClient(clientId);
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var disclaimers = await context.Disclaimers.Where(t => disclaimerIds.Contains(t.Id)).Include(t => t.Questions).ToListAsync();
            var selectedDisclaimers =  SelectLatestDisclaimers(disclaimers);
            return selectedDisclaimers;
        }

        public async Task<List<Disclaimer>> GetDisclaimersForNewUser()
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var disclaimers = await context.Disclaimers.Include(t => t.Questions).ToListAsync();
            var selectedDisclaimers =  SelectLatestDisclaimers(disclaimers);
            return selectedDisclaimers;
        }

        private static List<Disclaimer> SelectLatestDisclaimers(List<Disclaimer> disclaimers)
        {
            return disclaimers.GroupBy(t => t.Type).Select(t => t.MaxBy(t => t.CreationTs)).ToList();
        }
    }
}