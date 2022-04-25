using MyNoSqlServer.Abstractions;

namespace Service.DisclaimerEngine.Domain.Models.NoSql
{
    public class DisclaimerSettingsNoSqlEntity: MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-disclaimers-settings";

        public static string GeneratePartitionKey() => "Settings";
        public static string GenerateRowKey() => "Settings";

        public string MarketingEmailTextTemplate { get; set; }

        public static DisclaimerSettingsNoSqlEntity Create(string template)
        {
            return new DisclaimerSettingsNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(),
                MarketingEmailTextTemplate = template
            };
        }
    }
}