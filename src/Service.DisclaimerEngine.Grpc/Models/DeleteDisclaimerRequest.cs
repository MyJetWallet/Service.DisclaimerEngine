using System.Runtime.Serialization;

namespace Service.DisclaimerEngine.Grpc.Models;

[DataContract]
public class DeleteDisclaimerRequest
{
    [DataMember(Order = 1)] public string DisclaimerId { get; set; }
}