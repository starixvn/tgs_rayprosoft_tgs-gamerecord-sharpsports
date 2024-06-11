namespace Tgs.GameRecord.SharpSports.Core.ApplicationServices;
public record PullRecordSettlementCommand(
	Uri ApiUrl,
	string MerchantId,
	string ClientId,
	string ClientSecret,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);