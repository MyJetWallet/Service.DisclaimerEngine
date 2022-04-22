using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.DisclaimerEngine.Domain.Models;

namespace Service.DisclaimerEngine.Grpc.Models;

[DataContract]
public class GetDisclaimersResponse
{
    [DataMember(Order = 1)]
    public List<DisclaimerModel> Disclaimers { get; set; }
}

[DataContract]
public class DisclaimerModel
{
    [DataMember(Order = 1)]
    public string DisclaimerId { get; set; }
    [DataMember(Order = 2)]
    public string Title { get; set; }
    [DataMember(Order = 3)]
    public string Description { get; set; }
    [DataMember(Order = 4)]
    public List<QuestionModel> Questions { get; set; }
    [DataMember(Order = 5)]
    public string ImageUrl { get; set; }
}

[DataContract]
public class QuestionModel
{
    [DataMember(Order = 1)]
    public string QuestionId { get; set; }
    [DataMember(Order = 2)]
    public string Text { get; set; }
    [DataMember(Order = 3)]
    public bool Required { get; set; }
    [DataMember(Order = 4)]
    public bool DefaultState { get; set; }
}