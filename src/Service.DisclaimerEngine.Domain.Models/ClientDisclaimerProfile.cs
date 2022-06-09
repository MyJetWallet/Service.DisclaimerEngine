using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.DisclaimerEngine.Domain.Models
{
	[DataContract]
	public class ClientDisclaimerProfile
	{
		[DataMember(Order = 1)] public string ClientId { get; set; }
		[DataMember(Order = 2)] public List<string> AvailableDisclaimerTypes { get; set; }
	}
}