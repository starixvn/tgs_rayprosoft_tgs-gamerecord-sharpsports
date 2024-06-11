using Microsoft.Extensions.Logging;
using Tgs.GameInterface.SharpSports.Client;
using Tgs.GameInterface.SharpSports.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SharpSports.Core.ApplicationServices;

internal class PullRecordUnsettlementCommandService(
	ILogger<PullRecordUnsettlementCommandService> logger,
	IGameInterfaceSharpSportsClient client,
	ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordUnsettlementCommand>
{
	public async ValueTask ExecuteAsync(PullRecordUnsettlementCommand command, CancellationToken cancellationToken = default)
	{
		var page = 1;

		IEnumerable<GameRecordInfo> gameRecords;

		do
		{
			gameRecords = await client.GameRecordUnsettlementAsync(
				apiUrl: command.ApiUrl,
				merchantId: command.MerchantId,
				clientId: command.ClientId,
				clientSecret: command.ClientSecret,
				startTime: command.StartTime,
				endTime: command.EndTime,
				page: page,
				cancellationToken: cancellationToken)
				.ToArrayAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? Enumerable.Empty<GameRecordInfo>();
			foreach (var gameRecord in gameRecords)
			{
				logger.LogInformation("LogOn:{logOn} GameWager:{gameWager}", DateTime.Now.ToString("o"), gameRecord);

				try
				{
					await service.ExecuteAsync(
						command: new(gameRecord),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game wager.({gameWager})", DateTime.Now.ToString("o"), gameRecord);
				}
			}

			page++;
		} while (gameRecords.Any());
	}
}