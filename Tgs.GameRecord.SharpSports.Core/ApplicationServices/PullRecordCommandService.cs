using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SharpSports.Core.ApplicationServices;

internal class PullRecordCommandService(
	ILogger<PullRecordCommandService> logger,
	ICommandService<PullRecordUnsettlementCommand> unsettlementService,
	ICommandService<PullRecordSettlementCommand> settlementService) : ICommandService<PullRecordCommand>
{
	public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
	{
		try
		{
			await unsettlementService.ExecuteAsync(
				command: new(
					ApiUrl: command.ApiUrl,
					MerchantId: command.MerchantId,
					ClientId: command.ClientId,
					ClientSecret: command.ClientSecret,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Command = new
					{
						command.ApiUrl,
						command.MerchantId,
						command.ClientId,
						command.ClientSecret,
						StartTime = command.StartTime.ToString("u"),
						EndTime = command.EndTime.ToString("u"),
					},
					Message = "An exception occurred while processing unsettled processes."
				}));
		}

		try
		{
			await settlementService.ExecuteAsync(
				command: new(
					ApiUrl: command.ApiUrl,
					MerchantId: command.MerchantId,
					ClientId: command.ClientId,
					ClientSecret: command.ClientSecret,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Command = new
					{
						command.ApiUrl,
						command.MerchantId,
						command.ClientId,
						command.ClientSecret,
						StartTime = command.StartTime.ToString("u"),
						EndTime = command.EndTime.ToString("u"),
					},
					Message = "An exception occurred while processing the billing process."
				}));
		}
	}
}
