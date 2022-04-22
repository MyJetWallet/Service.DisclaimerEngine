using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.DisclaimerEngine.Domain.Models;

namespace Service.DisclaimerEngine.Grpc.Models;

[DataContract]
public class GetDisclaimerListResponse
{
    [DataMember(Order = 1)] public List<Disclaimer> Disclaimers { get; set; }
}