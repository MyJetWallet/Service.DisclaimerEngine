using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.DisclaimerEngine.Grpc;

namespace Service.DisclaimerEngine.Client
{
    [UsedImplicitly]
    public class DisclaimerEngineClientFactory: MyGrpcClientFactory
    {
        public DisclaimerEngineClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IHelloService GetHelloService() => CreateGrpcService<IHelloService>();
    }
}
