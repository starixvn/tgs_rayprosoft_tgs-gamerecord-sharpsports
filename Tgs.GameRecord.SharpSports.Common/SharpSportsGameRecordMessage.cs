using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.SharpSports.Common;

public record SharpSportsGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	public string MerchantId { get; init; } = default!;
	[Required]
	public string ClientId { get; init; } = default!;
	[Required]
	public string ClientSecret { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset EndTime { get; init; }
}