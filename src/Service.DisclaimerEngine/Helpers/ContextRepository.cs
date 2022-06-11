using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.DisclaimerEngine.Domain.Models;
using Service.DisclaimerEngine.Domain.Models.NoSql;
using Service.DisclaimerEngine.Postgres;
using Service.DisclaimerEngine.Services;

namespace Service.DisclaimerEngine.Helpers
{
    public class ContextRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ILogger<DisclaimerService> _logger;
        private readonly IMyNoSqlServerDataWriter<DisclaimerContextNoSqlEntity> _writer;

        public ContextRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ILogger<DisclaimerService> logger, IMyNoSqlServerDataWriter<DisclaimerContextNoSqlEntity> writer)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
            _writer = writer;
        }

        public async Task UpsertContexts(List<DisclaimerContext> contexts)
        {
            await _writer.BulkInsertOrReplaceAsync(contexts.Select(DisclaimerContextNoSqlEntity.Create).ToList());
            await _writer.CleanAndKeepMaxPartitions(Program.Settings.MaxCachedEntities);
            
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await context.UpsertAsync(contexts);
        }
        
        public async Task<List<DisclaimerContext>> GetContextsByClientId(string clientId)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var contexts = await context.Contexts.Where(t => t.ClientId == clientId).Include(t => t.Answers)
                .ToListAsync();
            return contexts;
        }
        
        public async Task<List<DisclaimerContext>> GetContextsByDisclaimerId(string disclaimerId)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var contexts = await context.Contexts.Where(t => t.DisclaimerId == disclaimerId).Include(t => t.Answers)
                .ToListAsync();
            return contexts;
        }

        public async Task<List<string>> GetDisclaimerIdsWithoutClient(string clientId)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var contexts = await ctx.Contexts.Where(t => t.ClientId == clientId).Select(t=>t.DisclaimerId)
                .ToListAsync();
            var disclaimers = await ctx.Disclaimers.Where(t => !contexts.Contains(t.Id)).Select(t=>t.Id).ToListAsync();
            return disclaimers;
        }
    }
}