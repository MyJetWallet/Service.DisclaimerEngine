using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.DisclaimerEngine.Settings
{
    public class SettingsModel
    {
        [YamlProperty("DisclaimerEngine.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("DisclaimerEngine.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("DisclaimerEngine.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
        
        [YamlProperty("DisclaimerEngine.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }
        
        [YamlProperty("DisclaimerEngine.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }
        
        [YamlProperty("DisclaimerEngine.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }
        
        [YamlProperty("DisclaimerEngine.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
        
        [YamlProperty("DisclaimerEngine.MaxCachedEntities")]
        public int MaxCachedEntities { get; set; }
        
        [YamlProperty("DisclaimerEngine.MessageTemplatesGrpcServiceUrl")]
        public string MessageTemplatesGrpcServiceUrl { get; set; }
        
        [YamlProperty("DisclaimerEngine.ClientProfileGrpcServiceUrl")]
        public string ClientProfileGrpcServiceUrl { get; set; }

    }
}
