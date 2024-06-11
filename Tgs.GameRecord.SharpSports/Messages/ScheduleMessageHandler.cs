using System.Text;
using System.Text.Json;
using Tgs.GameRecord.SharpSports.Common;
using Tgs.GameRecord.SharpSports.Core;
using Tgs.GameRecord.SharpSports.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.SharpSports.Messages;

public class ScheduleMessageHandler(
	ILogger<ScheduleMessageHandler> logger,
	IPullRecordDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<SharpSportsGameRecordMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					message.ApiUrl,
					message.MerchantId,
					message.ClientId,
					message.ClientSecret,
					StartTime = message.StartTime.ToString("u"),
					EndTime = message.EndTime.ToString("u"),
				},
				Message = "This receive message handler."
			}));

		return dispatcher.PushCommandAsync(
			command: new PullRecordCommand(
			ApiUrl: message.ApiUrl,
			MerchantId: message.MerchantId,
			ClientId: message.ClientId,
			ClientSecret: message.ClientSecret,
			StartTime: message.StartTime,
			EndTime: message.EndTime),
			cancellationToken: cancellationToken);
	}
}
