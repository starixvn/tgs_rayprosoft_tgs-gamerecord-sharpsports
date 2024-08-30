using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.SharpSports.Client;
using Tgs.GameRecord.SharpSports.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SharpSports.Core.UnitTest.ApplicationServices;

public class PullRecordUnsettlementCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<PullRecordUnsettlementCommandService>.Instance;
		var fakeClient = Substitute.For<IGameInterfaceSharpSportsClient>();
		var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

		var command = new PullRecordUnsettlementCommand(
			ApiUrl: new Uri("http://localhost/"),
			MerchantId: "merchantId",
			ClientId: "clientId",
			ClientSecret: "clientSecret",
			StartTime: DateTimeOffset.UtcNow.AddMinutes(-10),
			EndTime: DateTimeOffset.UtcNow);

		var service = new PullRecordUnsettlementCommandService(
			faceLogger,
			fakeClient,
			fakeService);

		await service.ExecuteAsync(command);

		_ = fakeClient
			.Received(0)
			.GameRecordSettlementAsync(
			apiUrl: Arg.Is(command.ApiUrl),
			merchantId: Arg.Is(command.MerchantId),
			clientId: Arg.Is(command.ClientId),
			clientSecret: Arg.Is(command.ClientSecret),
			startTime: Arg.Is(command.StartTime),
			endTime: Arg.Is(command.EndTime),
			page: Arg.Is(1),
			pageSize: Arg.Is(500),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeClient
			.Received(1)
			.GameRecordUnsettlementAsync(
			apiUrl: Arg.Is(command.ApiUrl),
			merchantId: Arg.Is(command.MerchantId),
			clientId: Arg.Is(command.ClientId),
			clientSecret: Arg.Is(command.ClientSecret),
			startTime: Arg.Is(command.StartTime),
			endTime: Arg.Is(command.EndTime),
			page: Arg.Is(1),
			pageSize: Arg.Is(500),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
