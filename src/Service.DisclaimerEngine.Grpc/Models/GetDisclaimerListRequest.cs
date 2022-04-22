using System.Runtime.Serialization;

namespace Service.DisclaimerEngine.Grpc.Models;

[DataContract]
public class GetDisclaimerListRequest
{
    [DataMember(Order = 1)]public int Skip { get; set; }
    [DataMember(Order = 2)]public int Take { get; set; }
}