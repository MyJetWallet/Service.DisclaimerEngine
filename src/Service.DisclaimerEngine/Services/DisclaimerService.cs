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
    public class DisclaimerService : IDisclaimerService
    {
        
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ILogger<DisclaimerService> _logger;
        private readonly ContextRepository _repository;
        private readonly ITemplateClient _templateClient;
        
        public DisclaimerService(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ILogger<DisclaimerService> logger, ContextRepository repository, ITemplateClient templateClient)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
            _repository = repository;
            _templateClient = templateClient;
        }

        public async Task<OperationResponse> SubmitAnswers(SubmitAnswersRequest request)
        {
            //_logger.LogInformation("Creating disclaimer {disclaimer}", disclaimer);
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                
                var disclaimerContext = new DisclaimerContext
                {
                    DisclaimerId = request.DisclaimerId,
                    ClientId = request.ClientId,
                    Answers = request.Answers,
                    Timestamp = DateTime.UtcNow
                };

                await _repository.UpsertContexts(new () {disclaimerContext});
                return new OperationResponse()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                //_logger.LogError(e, "When creating disclaimer {disclaimer}", disclaimer);
                return new OperationResponse()
                {
                    IsSuccess = false, 
                    ErrorText= e.Message
                };
            }
        }

        public async Task<HasDisclaimersResponse> HasDisclaimers(HasDisclaimersRequest request)
        {
            //_logger.LogInformation("Creating disclaimer {disclaimer}", disclaimer);
            try
            {
                var disclaimerIds = await _repository.GetDisclaimerIdsWithoutClient(request.ClientId);
                return new HasDisclaimersResponse()
                {
                    HasDisclaimers = disclaimerIds.Any()
                };
            }
            catch (Exception e)
            {
                //_logger.LogError(e, "When creating disclaimer {disclaimer}", disclaimer);
                return new HasDisclaimersResponse()
                {
                    HasDisclaimers = false, 
                };
            }
        }
        
        public async Task<GetDisclaimersResponse> GetDisclaimers(GetDisclaimersRequest request)
        {
            //_logger.LogInformation("Creating disclaimer {disclaimer}", disclaimer);
            try
            {
                var disclaimerIds = await _repository.GetDisclaimerIdsWithoutClient(request.ClientId);

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var disclaimers = await context.Disclaimers.Where(t => disclaimerIds.Contains(t.Id)).Include(t => t.Questions).ToListAsync();

                var groups = disclaimers.GroupBy(t => t.Type);

                var selectedDisclaimer = groups.Select(t => t.MaxBy(t => t.CreationTs)).ToList();

                var disclaimerModels = new List<DisclaimerModel>();
                foreach (var question in selectedDisclaimer)
                {
                    disclaimerModels.Add(await GetDisclaimerModel(question, request.Lang, request.Brand));
                }
                
                return new GetDisclaimersResponse()
                {
                    Disclaimers = disclaimerModels
                };
            }
            catch (Exception e)
            {
                //_logger.LogError(e, "When creating disclaimer {disclaimer}", disclaimer);
                return new GetDisclaimersResponse()
                {
                    Disclaimers = new List<DisclaimerModel>(), 
                };
            }
            //locals
            async Task<DisclaimerModel> GetDisclaimerModel(Disclaimer disclaimer, string lang, string brand)
            {
                var questions = new List<QuestionModel>();
                foreach (var question in disclaimer.Questions)
                {
                    questions.Add(await GetQuestionModel(question, lang, brand));
                }

                return new DisclaimerModel
                {
                    DisclaimerId = disclaimer.Id,
                    Title = await _templateClient.GetTemplateBody(disclaimer.TitleTemplateId, brand, lang),
                    Description = await _templateClient.GetTemplateBody(disclaimer.DescriptionTemplateId, brand, lang),
                    Questions = questions,
                    ImageUrl = disclaimer.ImageUrl
                };
            }

            async Task<QuestionModel> GetQuestionModel(Question question, string lang, string brand)
            {
                return new QuestionModel
                {
                    QuestionId = question.Id,
                    Text = await _templateClient.GetTemplateBody(question.TextTemplateId, brand, lang),
                    Required = question.IsRequired,
                    DefaultState = question.DefaultState
                };
            }
        }
    }
}