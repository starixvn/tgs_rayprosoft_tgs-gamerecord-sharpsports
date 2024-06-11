using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
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
			GameRecord: new GameInterface.SharpSports.Protos.GameRecordInfo
			{
				PlayerCode = "4ucmqplxb029",
				ExternalBetId = "QXT24060517201133513",
				ExternalCreatedOn = DateTimeOffset.Parse("2024-06-05T17:20:11+08:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2024-06-06T04:18:58+08:00").ToTimestamp(),
				ExternalPayoffOn = DateTimeOffset.Parse("2024-06-06T04:18:58+08:00").ToTimestamp(),
				State = GameInterface.SharpSports.Protos.GameRecordState.Win,
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

	[Fact]
	public async Task Loss()
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
			GameRecord: new GameInterface.SharpSports.Protos.GameRecordInfo
			{
				PlayerCode = "4um6m93suvtn",
				ExternalBetId = "AIS24061114093035050",
				ExternalCreatedOn = DateTimeOffset.Parse("2024-06-11T14:09:30+08:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2024-06-11T14:18:57+08:00").ToTimestamp(),
				ExternalPayoffOn = DateTimeOffset.Parse("2024-06-11T14:18:57+08:00").ToTimestamp(),
				State = GameInterface.SharpSports.Protos.GameRecordState.Loss,
				IsCashout = false,
				Bet = 10.0m,
				ValidBet = 10.0m,
				Settle = 0.0m,
				WinLoss = -10.0m,
				AdditionalData = new JsonObject
				{
					["gold"] = 10,
					["lang"] = "zh-cn",
					["merId"] = "c6a33e4e235e731b51d2f303199edeb4",
					["state"] = 3,
					["betNum"] = 1,
					["merName"] = "PG-CNY",
					["orderId"] = "AIS24061114093035050",
					["winGold"] = 0,
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
					["playerId"] = "2864cf4f761cde5a6e3413b49efa2d0d",
					["realGold"] = 10,
					["remoteIp"] = "125.227.69.109",
					["walletId"] = 1,
					["loginName"] = "4um6m93suvtn",
					["parlayNum"] = "1",
					["betDTOList"] = new JsonArray {
						new JsonObject
						{
							["ratio"] = "0.75",
							["score"] = "0:1",
							["gameId"] = "5882463674753146881757607540",
							["betItem"] = "利物浦 (Wboy) -0.5/1",
							["ioRatio"] = "0.71",
							["session"] = null,
							["awayTeam"] = "Man City (nikkitta)",
							["betItem1"] = "Liverpool (Wboy) -0.5/1",
							["gameDate"] = "2024-06-11",
							["gameTime"] = "14:08:00",
							["gameType"] = "FT",
							["homeTeam"] = "利物浦 (Wboy)",
							["playName"] = "滚球:让球",
							["playType"] = "RE",
							["showType"] = "RB",
							["ratioType"] = "REH",
							["leagueName"] = "E-Soccer 足球电竞之战 - 8 分钟赛事",
							["teamSuffix"] = "",
							["betItemLang"] = "{\"en-us\":\"Liverpool (Wboy) -0.5/1\",\"zh-tw\":\"Liverpool (Wboy) -0.5/1\",\"zh-cn\":\"利物浦 (Wboy) -0.5/1\",\"vi-vn\":\"Liverpool (Wboy) -0.5/1\",\"th-th\":\"Liverpool (Wboy) -0.5/1\"}",
							["gameTypeSon"] = null,
							["resultScore"] = "0 : 2",
							["awayLangTeam"] = "{\"en-us\":\"Man City (nikkitta)\",\"zh-tw\":\"Man City (nikkitta)\",\"zh-cn\":\"Man City (nikkitta)\",\"th-th\":\"แมนเชสเตอร์ซิตี (nikkitta)\"}",
							["homeLangTeam"] = "{\"en-us\":\"Liverpool (Wboy)\",\"zh-tw\":\"Liverpool (Wboy)\",\"zh-cn\":\"利物浦 (Wboy)\",\"vi-vn\":\"Liverpool (Wboy)\",\"th-th\":\"Liverpool (Wboy)\"}",
							["playNameLang"] = "{\"en-us\":\"Handicap\",\"zh-tw\":\"滾球:讓球\",\"zh-cn\":\"滚球:让球\",\"vi-vn\":\"Bóng lăn Chấp bóng\",\"th-th\":\"แฮนดิแคปสด\"}",
							["leagueLangName"] = "{\"en-us\":\"E-Soccer Battle - 8 min\",\"zh-tw\":\"E-Soccer 足球電競之戰 - 8 分鐘賽事\",\"zh-cn\":\"E-Soccer 足球电竞之战 - 8 分钟赛事\",\"th-th\":\"E-Soccer แข่งขัน - 8 นาที\"}",
							["betResultDetail"] = "L",
							["leagueShortName"] = "E-Soccer 足球电竞之战 - 8 分钟赛事",
							["leagueShortLangName"] = "{\"en-us\":\"E-Soccer Battle - 8 min\",\"zh-tw\":\"E-Soccer 足球電競之戰 - 8 分鐘賽事\",\"zh-cn\":\"E-Soccer 足球电竞之战 - 8 分钟赛事\",\"th-th\":\"E-Soccer แข่งขัน - 8 นาที\"}"
						}
					},
					["createDate"] = "2024-06-11 14:09:30",
					["playerName"] = "侯赛因·图克曼t6i",
					["resultDate"] = "2024-06-11 14:18:57",
					["cashoutType"] = "0",
					["creditState"] = 1,
					["validAmount"] = 10,
					["resultDetail"] = null,
					["terminalType"] = "17",
					["realResultDate"] = "2024-06-11 14:18:57"
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
				BetState = Handle.Common.Protos.BetState.Loss,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 10.0m,
				ValidBet = 10.0m,
				Settle = 0.0m,
				Commission = 0.0m,
				Turnover = 10.0m,
				WinLoss = -10.0m,
				Jackpot = 0.0m,
				AdditionalData = command.GameRecord.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Draw()
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
			GameRecord: new GameInterface.SharpSports.Protos.GameRecordInfo
			{
				PlayerCode = "4um6m93suvtn",
				ExternalBetId = "AIS24061113291235047",
				ExternalCreatedOn = DateTimeOffset.Parse("2024-06-11T13:29:12+08:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2024-06-11T13:34:52+08:00").ToTimestamp(),
				ExternalPayoffOn = DateTimeOffset.Parse("2024-06-11T13:34:52+08:00").ToTimestamp(),
				State = GameInterface.SharpSports.Protos.GameRecordState.Draw,
				IsCashout = false,
				Bet = 10.0m,
				ValidBet = 0.0m,
				Settle = 10.0m,
				WinLoss = 0.0m,
				AdditionalData = new JsonObject
				{
					["gold"] = 10,
					["lang"] = "zh-cn",
					["merId"] = "c6a33e4e235e731b51d2f303199edeb4",
					["state"] = 4,
					["betNum"] = 1,
					["merName"] = "PG-CNY",
					["orderId"] = "AIS24061113291235047",
					["winGold"] = 10,
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
					["playerId"] = "2864cf4f761cde5a6e3413b49efa2d0d",
					["realGold"] = 10,
					["remoteIp"] = "125.227.69.109",
					["walletId"] = 1,
					["loginName"] = "4um6m93suvtn",
					["parlayNum"] = "1",
					["betDTOList"] = new JsonArray {
						new JsonObject{
							["ratio"] = "0",
							["score"] = "0:0",
							["gameId"] = "5882327728719790081495891",
							["betItem"] = "England (Lowheels) 0",
							["ioRatio"] = "0.81",
							["session"] = null,
							["awayTeam"] = "England (Lowheels)",
							["betItem1"] = "England (Lowheels) 0",
							["gameDate"] = "2024-06-11",
							["gameTime"] = "13:24:00",
							["gameType"] = "FT",
							["homeTeam"] = "Germany (Jekunam)",
							["playName"] = "滚球:让球",
							["playType"] = "RE",
							["showType"] = "RB",
							["ratioType"] = "REC",
							["leagueName"] = "E-Soccer 足球电竞之战 - 8 分钟赛事",
							["teamSuffix"] = "",
							["betItemLang"] = "{\"en-us\":\"England (Lowheels) 0\",\"zh-tw\":\"England (Lowheels) 0\",\"zh-cn\":\"England (Lowheels) 0\"}",
							["gameTypeSon"] = null,
							["resultScore"] = "0 : 0",
							["awayLangTeam"] = "{\"en-us\":\"England (Lowheels)\",\"zh-tw\":\"England (Lowheels)\",\"zh-cn\":\"England (Lowheels)\"}",
							["homeLangTeam"] = "{\"en-us\":\"Germany (Jekunam)\",\"zh-tw\":\"Germany (Jekunam)\",\"zh-cn\":\"Germany (Jekunam)\"}",
							["playNameLang"] = "{\"en-us\":\"Handicap\",\"zh-tw\":\"滾球:讓球\",\"zh-cn\":\"滚球:让球\"}",
							["leagueLangName"] = "{\"en-us\":\"E-Soccer Battle - 8 min\",\"zh-tw\":\"E-Soccer 足球電競之戰 - 8 分鐘賽事\",\"zh-cn\":\"E-Soccer 足球电竞之战 - 8 分钟赛事\"}",
							["betResultDetail"] = "P",
							["leagueShortName"] = "E-Soccer 足球电竞之战 - 8 分钟赛事",
							["leagueShortLangName"] = "{\"en-us\":\"E-Soccer Battle - 8 min\",\"zh-tw\":\"E-Soccer 足球電競之戰 - 8 分鐘賽事\",\"zh-cn\":\"E-Soccer 足球电竞之战 - 8 分钟赛事\"}"
						}
					},
					["createDate"] = "2024-06-11 13:29:12",
					["playerName"] = "侯赛因·图克曼t6i",
					["resultDate"] = "2024-06-11 13:34:52",
					["cashoutType"] = "0",
					["creditState"] = 1,
					["validAmount"] = 0,
					["resultDetail"] = null,
					["terminalType"] = "17",
					["realResultDate"] = "2024-06-11 13:34:52"
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
				BetState = Handle.Common.Protos.BetState.Draw,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 10.0m,
				ValidBet = 0.0m,
				Settle = 10.0m,
				Commission = 0.0m,
				Turnover = 0.0m,
				WinLoss = 0.0m,
				Jackpot = 0.0m,
				AdditionalData = command.GameRecord.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Cashout()
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
			GameRecord: new GameInterface.SharpSports.Protos.GameRecordInfo
			{
				PlayerCode = "4um6m93suvtn",
				ExternalBetId = "QXT24060509054233411",
				ExternalCreatedOn = DateTimeOffset.Parse("2024-06-05T09:05:42+08:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2024-06-05T09:16:47+08:00").ToTimestamp(),
				ExternalPayoffOn = DateTimeOffset.Parse("2024-06-05T09:16:47+08:00").ToTimestamp(),
				State = GameInterface.SharpSports.Protos.GameRecordState.Loss,
				IsCashout = true,
				Bet = 10.0m,
				ValidBet = 1.0m,
				Settle = 9.0m,
				WinLoss = -1.0m,
				AdditionalData = new JsonObject
				{
					["gold"] = 10,
					["lang"] = "zh-tw",
					["merId"] = "c6a33e4e235e731b51d2f303199edeb4",
					["state"] = 3,
					["betNum"] = 1,
					["merName"] = "PG-CNY",
					["orderId"] = "QXT24060509054233411",
					["winGold"] = 9,
					["cashouts"] = new JsonObject
					{
						["stake"] = 10,
						["state"] = "Y",
						["initGold"] = 10,
						["wlAmount"] = -1,
						["cashoutId"] = "U1ee863c5-c6e3-47ab-b027-78b4df61b489",
						["returnAmount"] = 9,
						["placeDateTime"] = "2024-06-05 09:16:47"
					},
					["lineType"] = "2",
					["oddfType"] = "H",
					["playerId"] = "506763ccb6d328049ccc7db080514a06",
					["realGold"] = 10,
					["remoteIp"] = "125.227.69.109",
					["walletId"] = 1,
					["loginName"] = "4ucmqplxb029",
					["parlayNum"] = "1",
					["betDTOList"] = new JsonArray
					{
						new JsonObject{
							["ratio"] = "0",
							["score"] = "",
							["gameId"] = "5851809144314920961495891",
							["betItem"] = "拉鲁斯 0",
							["ioRatio"] = "1.09",
							["session"] = null,
							["awayTeam"] = "竞赛会",
							["betItem1"] = "CA Lanus 0",
							["gameDate"] = "2024-06-14",
							["gameTime"] = "08:15:00",
							["gameType"] = "FT",
							["homeTeam"] = "拉鲁斯",
							["playName"] = "让球",
							["playType"] = "R",
							["showType"] = "FU",
							["ratioType"] = "RH",
							["leagueName"] = "阿根廷甲级联赛",
							["teamSuffix"] = "",
							["betItemLang"] = "{\"en-us\":\"CA Lanus 0\",\"zh-tw\":\"拉魯斯 0\",\"zh-cn\":\"拉鲁斯 0\",\"vi-vn\":\"CA Lanus 0\",\"th-th\":\"CA Lanus 0\"}",
							["gameTypeSon"] = null,
							["resultScore"] = null,
							["awayLangTeam"] = "{\"en-us\":\"Racing Club Avellaneda\",\"zh-tw\":\"競賽會\",\"zh-cn\":\"竞赛会\",\"vi-vn\":\"Contest\",\"th-th\":\"Contest\"}",
							["homeLangTeam"] = "{\"en-us\":\"CA Lanus\",\"zh-tw\":\"拉魯斯\",\"zh-cn\":\"拉鲁斯\",\"vi-vn\":\"CA Lanus\",\"th-th\":\"CA Lanus\"}",
							["playNameLang"] = "{\"en-us\":\"Handicap\",\"zh-tw\":\"讓球\",\"zh-cn\":\"让球\",\"vi-vn\":\"Cược chấ\",\"th-th\":\"แฮนดิแคป \"}",
							["leagueLangName"] = "{\"en-us\":\"Argentine Primera División\",\"zh-tw\":\"阿根廷足球甲級聯賽\",\"zh-cn\":\"阿根廷甲级联赛\",\"vi-vn\":\"Argentina Superliga\",\"th-th\":\"Argentina Superliga\"}",
							["betResultDetail"] = "L",
							["leagueShortName"] = "阿甲",
							["leagueShortLangName"] = "{\"en-us\":\"ARG Primera División\",\"zh-tw\":\"阿甲\",\"zh-cn\":\"阿甲\",\"vi-vn\":\"Argentina Superliga\",\"th-th\":\"Argentina Superliga\"}"
						}
					},
					["createDate"] = "2024-06-05 09:05:42",
					["playerName"] = "AliFardydon0Ty",
					["resultDate"] = "2024-06-05 09:16:47",
					["cashoutType"] = "2",
					["creditState"] = 1,
					["validAmount"] = 1,
					["resultDetail"] = null,
					["terminalType"] = "16",
					["realResultDate"] = "2024-06-05 09:16:47"
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
				BetState = Handle.Common.Protos.BetState.Loss,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.Cashout,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 10.0m,
				ValidBet = 1.0m,
				Settle = 9.0m,
				Commission = 0.0m,
				Turnover = 1.0m,
				WinLoss = -1.0m,
				Jackpot = 0.0m,
				AdditionalData = command.GameRecord.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Running()
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
			GameRecord: new GameInterface.SharpSports.Protos.GameRecordInfo
			{
				PlayerCode = "4ucmqplxb029",
				ExternalBetId = "QXT24060517201133513",
				ExternalCreatedOn = DateTimeOffset.Parse("2024-06-05T17:20:11+08:00").ToTimestamp(),
				ExternalClosedOn = DateTime.UnixEpoch.ToTimestamp(),
				ExternalPayoffOn = DateTime.UnixEpoch.ToTimestamp(),
				State = GameInterface.SharpSports.Protos.GameRecordState.Running,
				IsCashout = false,
				Bet = 10.00m,
				ValidBet = 10.00m,
				Settle = 0.0m,
				WinLoss = 0.0m,
				AdditionalData = new JsonObject
				{
					["merId"] = "c6a33e4e235e731b51d2f303199edeb4",
					["merName"] = "PG-CNY",
					["playerId"] = "506763ccb6d328049ccc7db080514a06",
					["loginName"] = "4ucmqplxb029",
					["playerName"] = "AliFardydon0Ty",
					["remoteIp"] = "125.227.69.109",
					["orderId"] = "QXT24060509004433396",
					["parlayNum"] = "1",
					["gold"] = 10,
					["realGold"] = 10,
					["winGold"] = null,
					["state"] = 1,
					["creditState"] = 0,
					["realResultDate"] = null,
					["resultDate"] = "",
					["resultDetail"] = null,
					["createDate"] = "2024-06-05 09:00:44",
					["terminalType"] = "16",
					["oddfType"] = "H",
					["betDTOList"] = new JsonArray {
						new JsonObject{
							["showType"] = "FU",
							["gameType"] = "FT",
							["playType"] = "R",
							["ratioType"] = "RH",
							["gameDate"] = "2024-06-14",
							["gameTime"] = "06:00:00",
							["awayTeam"] = "贝尔格拉诺",
							["homeTeam"] = "堤格雷",
							["leagueName"] = "阿根廷甲级联赛",
							["leagueShortName"] = "阿甲",
							["score"] = "",
							["ioRatio"] = "1.09",
							["ratio"] = "0",
							["resultScore"] = null,
							["betItem"] = "堤格雷 0",
							["betResultDetail"] = null,
							["session"] = null,
							["teamSuffix"] = "",
							["awayLangTeam"] = "{\"en-us\":\"CA Belgrano\",\"zh-tw\":\"貝爾格拉諾\",\"zh-cn\":\"贝尔格拉诺\",\"vi-vn\":\"\",\"th-th\":\"\"}",
							["homeLangTeam"] = "{\"en-us\":\"Club Atletico Tigre\",\"zh-tw\":\"堤格雷\",\"zh-cn\":\"堤格雷\",\"vi-vn\":\"Club Atletico Tigre\",\"th-th\":\"Club Atletico Tigre\"}",
							["leagueLangName"] = "{\"en-us\":\"Argentine Primera División\",\"zh-tw\":\"阿根廷足球甲級聯賽\",\"zh-cn\":\"阿根廷甲级联赛\",\"vi-vn\":\"Argentina Superliga\",\"th-th\":\"Argentina Superliga\"}",
							["leagueShortLangName"] = "{\"en-us\":\"ARG Primera División\",\"zh-tw\":\"阿甲\",\"zh-cn\":\"阿甲\",\"vi-vn\":\"Argentina Superliga\",\"th-th\":\"Argentina Superliga\"}",
							["betItem1"] = "Club Atletico Tigre 0",
							["playNameLang"] = "{\"en-us\":\"Handicap\",\"zh-tw\":\"讓球\",\"zh-cn\":\"让球\",\"vi-vn\":\"Cược chấ\",\"th-th\":\"แฮนดิแคป \"}",
							["playName"] = "让球",
							["betItemLang"] = "{\"en-us\":\"Club Atletico Tigre 0\",\"zh-tw\":\"堤格雷 0\",\"zh-cn\":\"堤格雷 0\",\"vi-vn\":\"Club Atletico Tigre 0\",\"th-th\":\"Club Atletico Tigre 0\"}",
							["gameId"] = "5859428086331637761495891",
							["gameTypeSon"] = null
						}
					},
					["lineType"] = "2",
					["cashoutType"] = "0",
					["cashouts"] = new JsonObject
					{
						["cashoutId"] = null,
						["stake"] = null,
						["initGold"] = null,
						["returnAmount"] = null,
						["wlAmount"] = null,
						["placeDateTime"] = null,
						["state"] = null
					},
					["betNum"] = 1,
					["lang"] = "zh-tw",
					["walletId"] = 1,
					["validAmount"] = null
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
				ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				ExternalPayoffOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Running,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 10.0m,
				ValidBet = 10.0m,
				Settle = 0.0m,
				Commission = 0.0m,
				Turnover = 10.0m,
				WinLoss = 0.0m,
				Jackpot = 0.0m,
				AdditionalData = command.GameRecord.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
