using System.Linq;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using Service.DisclaimerEngine.Domain.Models.NoSql;
using Service.DisclaimerEngine.Grpc;
using Service.DisclaimerEngine.Grpc.Models;

namespace Service.DisclaimerEngine.Client;

public class DisclaimerClient : IDisclaimerService
{
    private readonly IDisclaimerService _disclaimerServiceGrpc;
    private readonly IMyNoSqlServerDataReader<DisclaimerProfileNoSqlEntity> _reader;

    public DisclaimerClient(IDisclaimerService disclaimerServiceGrpc, IMyNoSqlServerDataReader<DisclaimerProfileNoSqlEntity> reader)
    {
        _disclaimerServiceGrpc = disclaimerServiceGrpc;
        _reader = reader;
    }

    public async Task<OperationResponse> SubmitAnswers(SubmitAnswersRequest request) =>
        await _disclaimerServiceGrpc.SubmitAnswers(request);

    public async Task<HasDisclaimersResponse> HasDisclaimers(HasDisclaimersRequest request)
    {
        var entity = _reader.Get(DisclaimerProfileNoSqlEntity.GeneratePartitionKey(),
            DisclaimerProfileNoSqlEntity.GenerateRowKey(request.ClientId));

        if (entity != null)
            return new HasDisclaimersResponse()
            {
                HasDisclaimers = entity.Profile.AvailableDisclaimers.Any()
            };

        return await _disclaimerServiceGrpc.HasDisclaimers(request);
    }

    public async Task<GetDisclaimersResponse> GetDisclaimers(GetDisclaimersRequest request) =>
        await _disclaimerServiceGrpc.GetDisclaimers(request);
}