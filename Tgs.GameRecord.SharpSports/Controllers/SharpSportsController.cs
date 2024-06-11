using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.SharpSports.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.SharpSports.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SharpSportsController : ControllerBase
{
	/// <summary>
	/// Games the record manual asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///				"ApiUrl": "https://api.98ktest.com/",
	///				"MerchantId": "c6a33e4e235e731b51d2f303199edeb4",
	///				"ClientId": "2a954fedf9a203d646e1a7f3c32528df",
	///				"ClientSecret": "1ff45c6ed7494b04940dc0bd9e0dca6b",
	///				"StartTime": "2024-06-05T16:00:00+00:00",
	///				"EndTime": "2024-06-06T16:00:00+00:00"
	///         }
	/// </remarks>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		SharpSportsGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: SharpSportsNatsConst.ManualPublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///				"ApiUrl": "https://api.98ktest.com/",
	///				"MerchantId": "c6a33e4e235e731b51d2f303199edeb4",
	///				"ClientId": "2a954fedf9a203d646e1a7f3c32528df",
	///				"ClientSecret": "1ff45c6ed7494b04940dc0bd9e0dca6b",
	///				"StartTime": "2024-06-05T16:00:00+00:00",
	///				"EndTime": "2024-06-06T16:00:00+00:00"
	///         }
	/// </remarks>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		SharpSportsGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: SharpSportsNatsConst.SchedulePublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);
}