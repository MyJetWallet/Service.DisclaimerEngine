using System.ServiceModel;
using System.Threading.Tasks;
using Service.DisclaimerEngine.Grpc.Models;

namespace Service.DisclaimerEngine.Grpc;

[ServiceContract]
public interface IDisclaimerService
{
    [OperationContract]
    Task<OperationResponse> SubmitAnswers(SubmitAnswersRequest request);

    [OperationContract]
    Task<HasDisclaimersResponse> HasDisclaimers(HasDisclaimersRequest request);

    [OperationContract]
    Task<GetDisclaimersResponse> GetDisclaimers(GetDisclaimersRequest request);
}