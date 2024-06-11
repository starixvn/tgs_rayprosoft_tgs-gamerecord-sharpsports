using System.Reflection.Metadata;
using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.SharpSports.Protos;
using Tgs.GameRecord.SharpSports.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.SharpSports.Core.UnitTest.ApplicationServices;
public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Win()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "SHARP_SPORTS",
			GameProviderConfiguration = "SHARP_SPORTSConfiguration",
			GameExternalId = "sports_book",
			HangfirePrefix = "gamerecord-sharp_sports-hangfire:",
			HangfireLocking = "gamerecord-sharp_sports-locking:",
			IntervalMinutes = 10d,
			IntervalMinutesMax = 60d,
			IntervalMinutesMinimum = 1d
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new HandleRecordCommand(
			GameRecord: new GameRecordInfo
			{
				PlayerCode = "4ucmqplxb029",
				ExternalBetId = "QXT24060517201133513",
				ExternalCreatedOn = DateTimeOffset.Parse("2024-06-05T17:20:11+08:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2024-06-06T04:18:58+08:00").ToTimestamp(),
				ExternalPayoffOn = DateTimeOffset.Parse("2024-06-06T04:18:58+08:00").ToTimestamp(),
				IsClosed = true,
				IsCancel = false,
				IsRefund = false,
				IsVoid = false,
				IsCashout = false,
				Bet = 10.00m,
				ValidBet = 9.3m,
				Settle = 19.3m,
				WinLoss = 9.3m,
				AdditionalData = new JsonObject
				{
					["gold"] = 10,
					["lang"] = "zh-tw",
					["merId"] = "c6a33e4e235e731b51d2f303199edeb4",
					["state"] = 2,
					["betNum"] = 1,
					["merName"] = "PG-CNY",
					["orderId"] = "QXT24060517201133513",
					["winGold"] = 19.3,
					["cashouts"] = new JsonObject
					{
						["stake"] = null,
						["state"] = null,
						["initGold"] = null,
						["wlAmount"] = null,
						["cashoutId"] = null,
						["returnAmount"] = null,
						["placeDateTime"] = null
					},
					["lineType"] = "2",
					["oddfType"] = "H",
					["playerId"] = "506763ccb6d328049ccc7db080514a06",
					["realGold"] = 10,
					["remoteIp"] = "125.227.69.109",
					["walletId"] = 1,
					["loginName"] = "4ucmqplxb029",
					["parlayNum"] = "1",
					["betDTOList"] = new JsonArray {
						new JsonObject{
							["ratio"] = "11",
							["score"] = "",
							["gameId"] = "5857725171981148161420720904",
							["betItem"] = "UCAM穆尔西亚 +11",
							["ioRatio"] = "0.93",
							["session"] = "0",
							["awayTeam"] = "UCAM穆尔西亚",
							["betItem1"] = "CB Murcia +11",
							["gameDate"] = "2024-06-06",
							["gameTime"] = "02:30:00",
							["gameType"] = "BK",
							["homeTeam"] = "乌尼卡查马拉加",
							["playName"] = "让球",
							["playType"] = "R",
							["showType"] = "FT",
							["ratioType"] = "RC",
							["leagueName"] = "西班牙篮球甲级联赛",
							["teamSuffix"] = "",
							["betItemLang"] = "{\"en-us\":\"CB Murcia +11\",\"zh-tw\":\"UCAM穆爾西亞 +11\",\"zh-cn\":\"UCAM穆尔西亚 +11\",\"vi-vn\":\"CB Murcia +11\",\"th-th\":\"CB Murcia +11\"}",
							["gameTypeSon"] = null,
							["resultScore"] = "70 : 79",
							["awayLangTeam"] = "{\"en-us\":\"CB Murcia\",\"zh-tw\":\"UCAM穆爾西亞\",\"zh-cn\":\"UCAM穆尔西亚\",\"vi-vn\":\"CB Murcia\",\"th-th\":\"CB Murcia\"}",
							["homeLangTeam"] = "{\"en-us\":\"Malaga\",\"zh-tw\":\"烏尼卡查馬拉加\",\"zh-cn\":\"乌尼卡查马拉加\",\"vi-vn\":\"Unicaja Malaga\",\"th-th\":\"Malaga\"}",
							["playNameLang"] = "{\"en-us\":\"Handicap\",\"zh-tw\":\"讓球\",\"zh-cn\":\"让球\",\"vi-vn\":\"Cược chấp\",\"th-th\":\"แฮนดิแคป\"}",
							["leagueLangName"] = "{\"en-us\":\"Spain Liga ACB\",\"zh-tw\":\"西班牙籃球甲級聯賽\",\"zh-cn\":\"西班牙篮球甲级联赛\",\"vi-vn\":\"\",\"th-th\":\"Senegal Ligue 2\"}",
							["betResultDetail"] = "W",
							["leagueShortName"] = "西甲",
							["leagueShortLangName"] = "{\"en-us\":\"ESP Liga ACB\",\"zh-tw\":\"西甲\",\"zh-cn\":\"西甲\",\"vi-vn\":\"\",\"th-th\":\"Senegal Ligue 2\"}"
						}
					},
					["createDate"] = "2024-06-05 17:20:11",
					["playerName"] = "AliFardydon0Ty",
					["resultDate"] = "2024-06-06 04:18:58",
					["cashoutType"] = "0",
					["creditState"] = 1,
					["validAmount"] = 9.3,
					["resultDetail"] = null,
					["terminalType"] = "16",
					["realResultDate"] = "2024-06-06 04:18:58"
				}.ToJsonString()
			});

		var service = new HandleRecordCommandService(
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
						.PublishAsync(
						subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
						data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
						{
							GameProviderCode = gameRecordInfo.Value.GameProviderCode,
							PlayerCode = command.GameRecord.PlayerCode,
							ExternalRoundId = command.GameRecord.ExternalBetId,
							ExternalBetId = command.GameRecord.ExternalBetId,
							ExternalGameId = gameRecordInfo.Value.GameExternalId,
							ExternalCreatedOn = command.GameRecord.ExternalCreatedOn,
							ExternalClosedOn = command.GameRecord.ExternalClosedOn,
							ExternalPayoffOn = command.GameRecord.ExternalPayoffOn,
							ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
							BetState = Handle.Common.Protos.BetState.Win,
							BetType = Handle.Common.Protos.BetType.Normal,
							FreeGameState = Handle.Common.Protos.FreeGameState.None,
							GambleState = Handle.Common.Protos.GambleState.None,
							CashoutState = Handle.Common.Protos.CashoutState.None,
							ResettleState = Handle.Common.Protos.ResettleState.None,
							JackpotState = Handle.Common.Protos.JackpotState.None,
							Bet = 10.0m,
							ValidBet = 9.3m,
							Settle = 19.3m,
							Commission = 0.0m,
							Turnover = 9.3m,
							WinLoss = 9.3m,
							Jackpot = 0.0m,
							AdditionalData = command.GameRecord.AdditionalData
						}),
						cancellationToken: Arg.Any<CancellationToken>());
	}
}
