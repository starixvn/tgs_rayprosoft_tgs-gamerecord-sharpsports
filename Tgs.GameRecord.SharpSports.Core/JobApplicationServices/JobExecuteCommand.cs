namespace Tgs.GameRecord.SharpSports.Core.JobApplicationServices;

public record JobExecuteCommand(
	Uri ApiUrl,
	string MerchantId,
	string ClientId,
	string ClientSecret);
