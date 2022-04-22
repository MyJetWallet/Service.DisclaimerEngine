using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.DisclaimerEngine.Domain.Models;
using Service.DisclaimerEngine.Grpc;
using Service.DisclaimerEngine.Grpc.Models;
using Service.DisclaimerEngine.Helpers;
using Service.DisclaimerEngine.Postgres;
using Service.MessageTemplates.Client;

namespace Service.DisclaimerEngine.Services
{
    public class DisclaimerManagerService : IDisclaimerManagerService
    {
        
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ILogger<DisclaimerManagerService> _logger;

        
        public DisclaimerManagerService(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ILogger<DisclaimerManagerService> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
        }

        public async Task<GetContextResponse> GetContexts(GetContextsRequest request)
        {
            var take = 20;
            var skip = 0;
            if (request.Take != 0)
                take = request.Take;
            
            if (request.Skip != 0)
                skip = request.Skip;
            
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var query = ctx.Contexts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.ClientId))
                query = query.Where(t => t.ClientId == request.ClientId);
    
            if (!string.IsNullOrWhiteSpace(request.DisclaimerId))
                query = query.Where(t => t.DisclaimerId == request.DisclaimerId);

            var contexts = await query
                .Skip(skip).Take(take)
                .Include(t => t.Answers)
                .ToListAsync();
            
            return new GetContextResponse()
            {
                Contexts = contexts
            };
        }

        public async Task<GetDisclaimerListResponse> GetDisclaimers(GetDisclaimerListRequest request)
        {
            var take = 20;
            var skip = 0;
            if (request.Take != 0)
                take = request.Take;
            
            if (request.Skip != 0)
                skip = request.Skip;
            
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var disclaimers = await ctx.Disclaimers
                .Skip(skip).Take(take)
                .Include(t => t.Questions)
                .ToListAsync();
            
            return new GetDisclaimerListResponse()
            {
                Disclaimers = disclaimers
            };
        }

        public async Task<OperationResponse> CreateDisclaimer(Disclaimer disclaimer)
        {
            _logger.LogInformation("Creating disclaimer {disclaimer}", disclaimer);
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                foreach (var question in disclaimer.Questions)
                {
                    question.DisclaimerId = disclaimer.Id;
                }
                await context.UpsertAsync(new[] {disclaimer});
                return new OperationResponse()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When creating disclaimer {disclaimer}", disclaimer);
                return new OperationResponse()
                {
                    IsSuccess = false, 
                    ErrorText= e.Message
                };
            }
        }

        public async Task<OperationResponse> DeleteDisclaimer(DeleteDisclaimerRequest request)
        {
            _logger.LogInformation("Deleting disclaimer {request}", request);
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var disclaimer = await context.Disclaimers.Include(t=>t.Questions).FirstOrDefaultAsync(t => t.Id == request.DisclaimerId);
                if(disclaimer == null)
                    return new OperationResponse()
                {
                    IsSuccess = true
                };

                context.Questions.RemoveRange(disclaimer.Questions);
                context.Disclaimers.Remove(disclaimer);
                await context.SaveChangesAsync();
                
                return new OperationResponse()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When deleting disclaimer {request}", request);
                return new OperationResponse()
                {
                    IsSuccess = false, 
                    ErrorText= e.Message
                };
            }
        }
        
    }
}