using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using Service.DisclaimerEngine.Domain.Models.NoSql;
using Service.DisclaimerEngine.Grpc;
using Service.DisclaimerEngine.Helpers;
using Service.DisclaimerEngine.Services;
using Service.MessageTemplates.Client;

namespace Service.DisclaimerEngine.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var myNoSqlClient = builder.CreateNoSqlClient((() => Program.Settings.MyNoSqlReaderHostPort));

            builder.RegisterMessageTemplatesCachedClient(Program.Settings.MessageTemplatesGrpcServiceUrl,
                myNoSqlClient);

            builder.RegisterMyNoSqlWriter<DisclaimerContextNoSqlEntity>(()=>Program.Settings.MyNoSqlWriterUrl,
                DisclaimerContextNoSqlEntity.TableName);
            builder.RegisterMyNoSqlWriter<DisclaimerProfileNoSqlEntity>(()=>Program.Settings.MyNoSqlWriterUrl,
                DisclaimerProfileNoSqlEntity.TableName);
            builder.RegisterType<ContextRepository>().AsSelf().SingleInstance();
            builder.RegisterType<DisclaimerRepository>().AsSelf().SingleInstance();
            builder.RegisterType<ProfilesRepository>().AsSelf().SingleInstance();
            
            builder.RegisterType<DisclaimerService>().As<IDisclaimerService>().SingleInstance();
            builder.RegisterType<DisclaimerManagerService>().As<IDisclaimerManagerService>().SingleInstance();
        }
    }
}