namespace Tgs.GameRecord.SharpSports.Core.ApplicationServices;
public record PullRecordUnsettlementCommand(
	Uri ApiUrl,
	string MerchantId,
	string ClientId,
	string ClientSecret,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
