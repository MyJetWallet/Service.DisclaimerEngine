using Autofac;
using Service.DisclaimerEngine.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.DisclaimerEngine.Client
{
    public static class AutofacHelper
    {
        public static void RegisterDisclaimerEngineClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new DisclaimerEngineClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IHelloService>().SingleInstance();
        }
    }
}
