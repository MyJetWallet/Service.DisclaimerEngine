using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.DisclaimerEngine.Domain.Models;

namespace Service.DisclaimerEngine.Grpc.Models;

[DataContract]
public class SubmitAnswersRequest
{
    [DataMember(Order = 1)]
    public string ClientId { get; set; }
    [DataMember(Order = 2)]
    public string DisclaimerId { get; set; }
    [DataMember(Order = 3)]
    public List<Answer> Answers { get; set; }
}