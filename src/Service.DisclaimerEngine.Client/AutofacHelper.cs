using Autofac;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Service.DisclaimerEngine.Domain.Models.NoSql;
using Service.DisclaimerEngine.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.DisclaimerEngine.Client
{
    public static class AutofacHelper
    {
        public static void RegisterDisclaimerEngineClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new DisclaimerEngineClientFactory(grpcServiceUrl, null);

            builder.RegisterInstance(factory.GetDisclaimerService()).As<IDisclaimerService>().SingleInstance();
            builder.RegisterInstance(factory.GetManagerService()).As<IDisclaimerManagerService>().SingleInstance();
        }
        public static void RegisterDisclaimerEngineClientCached(this ContainerBuilder builder, string grpcServiceUrl, IMyNoSqlSubscriber myNoSqlSubscriber)
        {
            var subs = new MyNoSqlReadRepository<DisclaimerProfileNoSqlEntity>(myNoSqlSubscriber, DisclaimerProfileNoSqlEntity.TableName);

            var factory = new DisclaimerEngineClientFactory(grpcServiceUrl, subs);

            builder
                .RegisterInstance(subs)
                .As<IMyNoSqlServerDataReader<DisclaimerProfileNoSqlEntity>>()
                .SingleInstance();
            
            builder.RegisterInstance(factory.GetDisclaimerService()).As<IDisclaimerService>().SingleInstance();
            builder.RegisterInstance(factory.GetManagerService()).As<IDisclaimerManagerService>().SingleInstance();
        }
    }
}
