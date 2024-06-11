namespace Tgs.GameRecord.SharpSports.Core;

public record GameRecordInfoOptions()
{
	public string GameProviderCode { get; set; } = default!;
	public string GameProviderConfiguration { get; init; } = default!;
	public string GameExternalId { get; set; } = default!;
	public string HangfirePrefix { get; set; } = "gamerecord-sharp_sports-hangfire:";
	public string HangfireLocking { get; set; } = "gamerecord-sharp_sports-locking:";
	public double IntervalMinutes { get; init; } = 10;

	public double IntervalMinutesMax { get; init; } = 1440;

	public double IntervalMinutesMinimum { get; init; } = 1;
}