using System.Runtime.Serialization;

namespace Service.DisclaimerEngine.Grpc.Models;

[DataContract]
public class GetContextsRequest
{
    [DataMember(Order = 1)]public string ClientId { get; set; }
    [DataMember(Order = 2)]public string DisclaimerId { get; set; }
    [DataMember(Order = 3)]public int Skip { get; set; }
    [DataMember(Order = 4)]public int Take { get; set; }
}