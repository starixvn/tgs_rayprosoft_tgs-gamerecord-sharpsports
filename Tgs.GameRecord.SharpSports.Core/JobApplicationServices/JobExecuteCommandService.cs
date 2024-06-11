using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.SharpSports.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.SharpSports.Core.JobApplicationServices;

internal class JobExecuteCommandService(
	ILogger<JobExecuteCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	TimeProvider timeProvider,
	IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
	private readonly double _intervalMinutes = options.Value.IntervalMinutesMax < options.Value.IntervalMinutes
		? options.Value.IntervalMinutesMax
		: options.Value.IntervalMinutesMinimum > options.Value.IntervalMinutes
		? options.Value.IntervalMinutesMinimum
		: options.Value.IntervalMinutes;

	public async ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();

		var endTime = new DateTimeOffset(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0, currentTime.Offset);
		var startTime = endTime.AddMinutes(_intervalMinutes * -1);

		var message = new SharpSportsGameRecordMessage
		{
			ApiUrl = command.ApiUrl,
			MerchantId = command.MerchantId,
			ClientId = command.ClientId,
			ClientSecret = command.ClientSecret,
			StartTime = startTime,
			EndTime = endTime
		};

		logger.LogDebug("LogOn:{logOn} Parameters:{parameters}", DateTime.Now.ToString("u"), message);

		await messageSender.PublishAsync(
			subject: SharpSportsNatsConst.SchedulePublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}