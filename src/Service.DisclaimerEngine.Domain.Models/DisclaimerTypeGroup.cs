namespace Service.DisclaimerEngine.Domain.Models
{
	public class DisclaimerTypeGroup
	{
		public static string[] BaseDisclaimerTypes =
		{
			DisclaimerTypes.TermsOfServiceType
		};

		public static string[] HighYieldDisclaimerTypes =
		{
			DisclaimerTypes.TermsOfHighYieldType
		};
	}
}