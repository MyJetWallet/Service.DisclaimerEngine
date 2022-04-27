using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.Abstractions;
using Service.ClientProfile.Grpc;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.DisclaimerEngine.Domain;
using Service.DisclaimerEngine.Domain.Models;
using Service.DisclaimerEngine.Domain.Models.NoSql;
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
        private readonly DisclaimerRepository _disclaimerRepository;
        private readonly ProfilesRepository _profilesRepository;
        private readonly IMyNoSqlServerDataReader<DisclaimerSettingsNoSqlEntity> _settingsReader;
        private readonly ITemplateClient _templateClient;
        private readonly IClientProfileService _clientProfileService;
        
        public DisclaimerService(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ILogger<DisclaimerService> logger, ContextRepository repository, ITemplateClient templateClient, DisclaimerRepository disclaimerRepository, ProfilesRepository profilesRepository, IMyNoSqlServerDataReader<DisclaimerSettingsNoSqlEntity> settingsReader, IClientProfileService clientProfileService)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
            _repository = repository;
            _templateClient = templateClient;
            _disclaimerRepository = disclaimerRepository;
            _profilesRepository = profilesRepository;
            _settingsReader = settingsReader;
            _clientProfileService = clientProfileService;
        }

        public async Task<OperationResponse> SubmitAnswers(SubmitAnswersRequest request)
        {
            _logger.LogInformation("Submitting answers {request}", request.ToJson());
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var disclaimer = await context.Disclaimers.Include(t => t.Questions).FirstOrDefaultAsync(t => t.Id == request.DisclaimerId);
                if (disclaimer == null)
                    return new OperationResponse()
                    {
                        IsSuccess = false,
                        ErrorText = "DisclaimerNotFound"
                    };
                
                foreach (var answer in request.Answers)
                {
                    var question = disclaimer.Questions.FirstOrDefault(t => t.Id == answer.QuestionId);
                    if (question == null)
                        continue;
                
                    if (question.IsRequired && !answer.Result)
                        return new OperationResponse()
                        {
                            IsSuccess = false,
                            ErrorText = "AnswerRequired"
                        };
                }

                var contexts = new List<DisclaimerContext>();
                var disclaimerContext = new DisclaimerContext
                {
                    DisclaimerId = request.DisclaimerId,
                    ClientId = request.ClientId,
                    Answers = request.Answers,
                    Timestamp = DateTime.UtcNow
                };

                var marketingAnswer =
                    request.Answers.FirstOrDefault(t => t.QuestionId == DisclaimerConsts.MarketingQuestionId);
                
                if (marketingAnswer != null)
                {
                    disclaimerContext.Answers.Remove(marketingAnswer);
                    if (marketingAnswer.Result)
                        await _clientProfileService.SetMarketingEmailSettings(new SetMarketingEmailSettingsRequest
                        {
                            ClientId = request.ClientId,
                            IsAllowed = true
                        });
                }
               
                contexts.Add(disclaimerContext);
                
                var oldDisclaimers = await context.Disclaimers.Where(t => t.Id != request.DisclaimerId && t.Type == disclaimer.Type).ToListAsync();
                foreach (var oldDisclaimer in oldDisclaimers)
                {
                    contexts.Add(new DisclaimerContext
                    {
                        DisclaimerId = oldDisclaimer.Id,
                        ClientId = request.ClientId,
                        Answers = new List<Answer>(),
                        ReplacedWithNewerDisclaimer = true,
                        Timestamp = DateTime.UtcNow
                    });
                }

                await _repository.UpsertContexts(contexts);
                await _profilesRepository.ClearCache(request.ClientId);
                return new OperationResponse()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When submitting answers {request}", request.ToJson());
                return new OperationResponse()
                {
                    IsSuccess = false, 
                    ErrorText= e.Message
                };
            }
        }

        public async Task<HasDisclaimersResponse> HasDisclaimers(HasDisclaimersRequest request)
        {
            _logger.LogInformation("Checking disclaimers {request}", request.ToJson());
            try
            {
                var profile = await _profilesRepository.GetOrCreateProfile(request.ClientId);
                return new HasDisclaimersResponse()
                {
                    HasDisclaimers = profile.AvailableDisclaimers.Any()
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When checking disclaimers {request}", request.ToJson());
                return new HasDisclaimersResponse()
                {
                    HasDisclaimers = false, 
                };
            }
        }
        
        public async Task<GetDisclaimersResponse> GetDisclaimers(GetDisclaimersRequest request)
        {
            _logger.LogInformation("Getting disclaimers {request}", request.ToJson());
            try
            {
                var disclaimers = await _disclaimerRepository.GetDisclaimersForUser(request.ClientId);

                var disclaimerModels = new List<DisclaimerModel>();
                
                var profile = await _clientProfileService.GetOrCreateProfile(new GetClientProfileRequest()
                {
                    ClientId = request.ClientId
                });

                foreach (var question in disclaimers)
                {
                    disclaimerModels.Add(await GetDisclaimerModel(question, request.Lang, request.Brand, (!profile.MarketingEmailAllowed)));
                }
                
                return new GetDisclaimersResponse()
                {
                    Disclaimers = disclaimerModels
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When getting disclaimers {request}", request.ToJson());
                return new GetDisclaimersResponse()
                {
                    Disclaimers = new List<DisclaimerModel>(), 
                };
            }
            
            //locals
            async Task<DisclaimerModel> GetDisclaimerModel(Disclaimer disclaimer, string lang, string brand, bool addMarketing)
            {
                var questions = new List<QuestionModel>();
                foreach (var question in disclaimer.Questions)
                {
                    questions.Add(await GetQuestionModel(question, lang, brand));
                }

                if (disclaimer.ShowMarketingEmailQuestion && addMarketing)
                {
                    var settings = _settingsReader.Get(DisclaimerSettingsNoSqlEntity.GeneratePartitionKey(),
                        DisclaimerSettingsNoSqlEntity.GenerateRowKey());
                    if (settings != null)
                    {
                        questions.Add(new QuestionModel
                        {
                            QuestionId = DisclaimerConsts.MarketingQuestionId,
                            Text = await _templateClient.GetTemplateBody(settings.MarketingEmailTextTemplate, lang, brand),
                            Required = false,
                            DefaultState = false
                        });
                    }
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