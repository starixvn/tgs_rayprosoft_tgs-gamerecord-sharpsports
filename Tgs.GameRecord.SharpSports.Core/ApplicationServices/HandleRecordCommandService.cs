using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.SharpSports.Core.ApplicationServices;

internal class HandleRecordCommandService(
	ILogger<HandleRecordCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	IMessageSender messageSender) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
	{
		var externalCreatedOn = command.GameRecord.ExternalCreatedOn.ToDateTimeOffset();
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var externalPayoffOn = DateTimeOffset.UnixEpoch;
		var betState = Handle.Common.Protos.BetState.Running;
		var cashoutState = Handle.Common.Protos.CashoutState.None;
		var bet = command.GameRecord.Bet.ToDecimal().GetValueOrDefault(0);
		var validBet = command.GameRecord.Bet.ToDecimal().GetValueOrDefault(0);
		var settle = 0.0m;
		var winLoss = 0.0m;
		if (command.GameRecord.IsClosed)
		{
			externalClosedOn = command.GameRecord.ExternalClosedOn.ToDateTimeOffset();
			externalPayoffOn = command.GameRecord.ExternalPayoffOn.ToDateTimeOffset();
			betState = command.GameRecord.IsVoid ? Handle.Common.Protos.BetState.Void :
				   command.GameRecord.IsCancel ? Handle.Common.Protos.BetState.Cancel :
				   command.GameRecord.IsRefund ? Handle.Common.Protos.BetState.Refund :
				   command.GameRecord.IsCashout ? Handle.Common.Protos.BetState.Refund :
				   command.GameRecord.WinLoss.ToDecimal().GetValueOrDefault(0) > 0 ? Handle.Common.Protos.BetState.Win :
				   Handle.Common.Protos.BetState.Loss;
			validBet = command.GameRecord.ValidBet.ToDecimal().GetValueOrDefault(0);
			settle = command.GameRecord.Settle.ToDecimal().GetValueOrDefault(0);
			winLoss = command.GameRecord.WinLoss.ToDecimal().GetValueOrDefault(0);
			cashoutState = command.GameRecord.IsCashout ? Handle.Common.Protos.CashoutState.Cashout : Handle.Common.Protos.CashoutState.None;
		}

		var request = new Handle.Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = _gameRecordInfo.GameProviderCode,
			PlayerCode = command.GameRecord.PlayerCode,
			ExternalRoundId = command.GameRecord.ExternalBetId,
			ExternalBetId = command.GameRecord.ExternalBetId,
			ExternalGameId = _gameRecordInfo.GameExternalId,
			ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
			ExternalClosedOn = externalClosedOn.ToTimestamp(),
			ExternalPayoffOn = externalPayoffOn.ToTimestamp(),
			ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
			BetState = betState,
			BetType = Handle.Common.Protos.BetType.Normal,
			FreeGameState = Handle.Common.Protos.FreeGameState.None,
			GambleState = Handle.Common.Protos.GambleState.None,
			CashoutState = cashoutState,
			ResettleState = Handle.Common.Protos.ResettleState.None,
			JackpotState = Handle.Common.Protos.JackpotState.None,
			Bet = bet,
			ValidBet = validBet,
			Settle = settle,
			Commission = 0.0m,
			Turnover = validBet,
			WinLoss = winLoss,
			Jackpot = 0.0m,
			AdditionalData = command.GameRecord.AdditionalData
		};

		await messageSender.PublishAsync(
			subject: Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			data: request,
			cancellationToken: cancellationToken)
				.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This publication requests information.({request})",
				DateTime.Now.ToString("o"),
				Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
				new
				{
					request.GameProviderCode,
					request.PlayerCode,
					request.ExternalRoundId,
					request.ExternalBetId,
					request.ExternalGameId,
					ExternalCreatedOn = request.ExternalCreatedOn.ToDateTimeOffset(),
					ExternalClosedOn = request.ExternalClosedOn.ToDateTimeOffset(),
					request.BetState
				});
	}
}