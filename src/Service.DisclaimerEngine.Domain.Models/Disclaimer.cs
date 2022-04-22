using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.DisclaimerEngine.Domain.Models
{
    [DataContract]
    public class Disclaimer
    {
        [DataMember(Order = 1)]public string Id { get; set; }
        [DataMember(Order = 2)]public string Name { get; set; }
        [DataMember(Order = 3)]public string Type { get; set; }
        [DataMember(Order = 4)]public DateTime CreationTs { get; set; }
        [DataMember(Order = 5)]public string TitleTemplateId { get; set; }
        [DataMember(Order = 6)]public string DescriptionTemplateId { get; set; }
        [DataMember(Order = 7)] public List<Question> Questions { get; set; }
        [DataMember(Order = 8)]public string ImageUrl { get; set; }
        [DataMember(Order = 9)]public bool ShowMarketingEmailQuestion { get; set; }
    }
}