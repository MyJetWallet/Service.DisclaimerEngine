using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using MyNoSqlServer.DataReader;
using Service.DisclaimerEngine.Domain.Models.NoSql;
using Service.DisclaimerEngine.Grpc;

namespace Service.DisclaimerEngine.Client
{
    [UsedImplicitly]
    public class DisclaimerEngineClientFactory: MyGrpcClientFactory
    {
        private readonly MyNoSqlReadRepository<DisclaimerProfileNoSqlEntity> _reader;

        public DisclaimerEngineClientFactory(string grpcServiceUrl, MyNoSqlReadRepository<DisclaimerProfileNoSqlEntity> reader) : base(grpcServiceUrl)
        {
            _reader = reader;
        }

        public IDisclaimerManagerService GetManagerService() => CreateGrpcService<IDisclaimerManagerService>();
        
        public IDisclaimerService GetDisclaimerService() => _reader != null 
            ? new DisclaimerClient(CreateGrpcService<IDisclaimerService>(), _reader)
            :  CreateGrpcService<IDisclaimerService>();
    }
}
