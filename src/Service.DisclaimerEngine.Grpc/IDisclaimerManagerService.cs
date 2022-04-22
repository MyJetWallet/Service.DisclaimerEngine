using System.ServiceModel;
using System.Threading.Tasks;
using Service.DisclaimerEngine.Domain.Models;
using Service.DisclaimerEngine.Grpc.Models;

namespace Service.DisclaimerEngine.Grpc;

[ServiceContract]
public interface IDisclaimerManagerService
{
    [OperationContract]
    Task<GetContextResponse> GetContexts(GetContextsRequest request);

    [OperationContract]
    Task<GetDisclaimerListResponse> GetDisclaimers(GetDisclaimerListRequest request);
    
    [OperationContract]
    Task<OperationResponse> CreateDisclaimer(Disclaimer disclaimer);

    [OperationContract]
    Task<OperationResponse> DeleteDisclaimer(DeleteDisclaimerRequest request);
}