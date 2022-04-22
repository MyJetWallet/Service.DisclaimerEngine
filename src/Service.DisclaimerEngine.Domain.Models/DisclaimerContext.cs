using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.DisclaimerEngine.Domain.Models
{
    [DataContract]
    public class DisclaimerContext
    {
        [DataMember(Order = 1)]public string DisclaimerId { get; set; }
        [DataMember(Order = 2)]public string ClientId { get; set; }
        [DataMember(Order = 3)]public List<Answer> Answers { get; set; }
        [DataMember(Order = 4)]public DateTime Timestamp { get; set; }
    }
}