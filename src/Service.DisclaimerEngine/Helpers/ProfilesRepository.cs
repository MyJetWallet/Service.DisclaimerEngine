using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.DisclaimerEngine.Domain.Models;
using Service.DisclaimerEngine.Domain.Models.NoSql;
using Service.DisclaimerEngine.Postgres;

namespace Service.DisclaimerEngine.Helpers
{
    public class ProfilesRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ILogger<ProfilesRepository> _logger;
        private readonly IMyNoSqlServerDataWriter<DisclaimerProfileNoSqlEntity> _writer;
        private readonly DisclaimerRepository _disclaimerRepository;
        
        public ProfilesRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ILogger<ProfilesRepository> logger, IMyNoSqlServerDataWriter<DisclaimerProfileNoSqlEntity> writer, DisclaimerRepository disclaimerRepository)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
            _writer = writer;
            _disclaimerRepository = disclaimerRepository;
        }

        public async Task<ClientDisclaimerProfile> GetOrCreateProfile(string clientId)
        {
            var entity = await _writer.GetAsync(DisclaimerProfileNoSqlEntity.GeneratePartitionKey(),
                DisclaimerProfileNoSqlEntity.GenerateRowKey(clientId));
            if (entity != null)
                return entity.Profile;
            
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var disclaimers = await _disclaimerRepository.GetDisclaimersForUser(clientId);
            var profile = new ClientDisclaimerProfile
            {
                ClientId = clientId,
                AvailableDisclaimerTypes = disclaimers.Select(t => t.Type).ToList()
            };

            await _writer.InsertOrReplaceAsync(DisclaimerProfileNoSqlEntity.Create(profile));
            await _writer.CleanAndKeepMaxRecords(DisclaimerProfileNoSqlEntity.GeneratePartitionKey(),
                Program.Settings.MaxCachedEntities);
            return profile;
        }
        
        public async Task ClearCache(string clientId)
        {
            await _writer.DeleteAsync(DisclaimerProfileNoSqlEntity.GeneratePartitionKey(),
                DisclaimerProfileNoSqlEntity.GenerateRowKey(clientId));
        }

        public async Task ClearCache()
        {
            await _writer.CleanAndKeepMaxPartitions(0);
        }
    }
}