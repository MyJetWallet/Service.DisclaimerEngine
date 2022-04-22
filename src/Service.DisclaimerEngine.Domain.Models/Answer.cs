using System;
using System.Runtime.Serialization;

namespace Service.DisclaimerEngine.Domain.Models
{
    [DataContract]
    public class Answer
    {
        [DataMember(Order = 1)]
        public string DisclaimerId { get; set; }
        [DataMember(Order = 2)]
        public string QuestionId { get; set; }
        [DataMember(Order = 3)]
        public string ClientId { get; set; }
        [DataMember(Order = 4)]
        public bool Result { get; set; }
    }
}