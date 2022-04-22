using System.ServiceModel;
using System.Threading.Tasks;
using Service.DisclaimerEngine.Grpc.Models;

namespace Service.DisclaimerEngine.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}