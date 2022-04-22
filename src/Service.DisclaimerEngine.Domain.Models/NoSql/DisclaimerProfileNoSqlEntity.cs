using MyNoSqlServer.Abstractions;

namespace Service.DisclaimerEngine.Domain.Models.NoSql
{
    public class DisclaimerProfileNoSqlEntity: MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-disclaimers-profiles";

        public static string GeneratePartitionKey() => "Profiles";
        public static string GenerateRowKey(string clientId) => clientId;

        public ClientDisclaimerProfile Profile { get; set; }

        public static DisclaimerProfileNoSqlEntity Create(ClientDisclaimerProfile profile)
        {
            return new DisclaimerProfileNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(profile.ClientId),
                Profile = profile
            };
        }
    }
}