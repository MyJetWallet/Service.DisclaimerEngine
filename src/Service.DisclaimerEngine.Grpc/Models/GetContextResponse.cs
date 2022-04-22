using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.DisclaimerEngine.Domain.Models;

namespace Service.DisclaimerEngine.Grpc.Models;

[DataContract]
public class GetContextResponse
{
    [DataMember(Order = 1)] public List<DisclaimerContext> Contexts { get; set; }
}