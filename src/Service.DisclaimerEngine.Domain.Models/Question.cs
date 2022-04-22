using System.Runtime.Serialization;

namespace Service.DisclaimerEngine.Domain.Models
{
    [DataContract]
    public class Question
    {
        [DataMember(Order = 1)]public string Id { get; set; }
        [DataMember(Order = 2)]public string Name { get; set; }
        [DataMember(Order = 3)]public string TextTemplateId { get; set; }
        [DataMember(Order = 4)]public bool IsRequired { get; set; }
        [DataMember(Order = 5)] public bool DefaultState { get; set; }
        [DataMember(Order = 6)]public string DisclaimerId { get; set; }

    }
}