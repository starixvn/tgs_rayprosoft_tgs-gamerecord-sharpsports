using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.SharpSports.Common;

public record SharpSportsGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://api.98ktest.com/")]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "c6a33e4e235e731b51d2f303199edeb4")]
	public string MerchantId { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "2a954fedf9a203d646e1a7f3c32528df")]
	public string ClientId { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "1ff45c6ed7494b04940dc0bd9e0dca6b")]
	public string ClientSecret { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2024-06-05T16:00:00+00:00")]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2024-06-06T16:00:00+00:00")]
	public DateTimeOffset EndTime { get; init; }
}
