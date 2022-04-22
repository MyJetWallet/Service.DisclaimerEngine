using System.Runtime.Serialization;

namespace Service.DisclaimerEngine.Grpc.Models;

[DataContract]
public class HasDisclaimersRequest
{
    [DataMember(Order = 1)]
    public string ClientId { get; set; }
}