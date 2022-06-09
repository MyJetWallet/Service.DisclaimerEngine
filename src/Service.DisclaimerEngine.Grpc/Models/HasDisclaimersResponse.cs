using System.Runtime.Serialization;

namespace Service.DisclaimerEngine.Grpc.Models;

[DataContract]
public class HasDisclaimersResponse
{
    [DataMember(Order = 1)]
    public bool HasDisclaimers { get; set; }

    [DataMember(Order = 2)]
    public bool HasHighYieldDisclaimers { get; set; }
}