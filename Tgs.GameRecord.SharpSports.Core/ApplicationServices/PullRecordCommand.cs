namespace Tgs.GameRecord.SharpSports.Core.ApplicationServices;
public record PullRecordCommand(
	Uri ApiUrl,
	string MerchantId,
	string ClientId,
	string ClientSecret,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
