using System.Runtime.Serialization;

namespace Service.DisclaimerEngine.Grpc.Models;

[DataContract]
public class GetDisclaimersRequest
{
    [DataMember(Order = 1)] public string ClientId { get; set; }
    [DataMember(Order = 2)] public string Lang { get; set; }
    [DataMember(Order = 3)] public string Brand { get; set; }

}