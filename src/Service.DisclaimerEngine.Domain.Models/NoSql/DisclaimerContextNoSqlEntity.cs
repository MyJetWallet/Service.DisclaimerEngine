using MyNoSqlServer.Abstractions;

namespace Service.DisclaimerEngine.Domain.Models.NoSql
{
    public class DisclaimerContextNoSqlEntity: MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-disclaimers-contexts";

        public static string GeneratePartitionKey(string clientId) => clientId;
        public static string GenerateRowKey(string campaignId) => campaignId;

        public DisclaimerContext Context { get; set; }

        public static DisclaimerContextNoSqlEntity Create(DisclaimerContext context)
        {
            return new DisclaimerContextNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(context.ClientId),
                RowKey = GenerateRowKey(context.DisclaimerId),
                Context = context
            };
        }
    }
}