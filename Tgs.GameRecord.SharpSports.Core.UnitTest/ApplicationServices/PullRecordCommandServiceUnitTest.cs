using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.SharpSports.Client;
using Tgs.GameRecord.SharpSports.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.SharpSports.Core.UnitTest.ApplicationServices;
public class PullRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<PullRecordCommandService>.Instance;
		var fakeUnsettlementService = Substitute.For<ICommandService<PullRecordUnsettlementCommand>>();
		var faceSettlementService = Substitute.For<ICommandService<PullRecordSettlementCommand>>();

		var command = new PullRecordCommand(
			ApiUrl: new Uri("http://localhost/"),
			MerchantId: "merchantId",
			ClientId: "clientId",
			ClientSecret: "clientSecret",
			StartTime: DateTimeOffset.UtcNow.AddMinutes(-10),
			EndTime: DateTimeOffset.UtcNow);

		var service = new PullRecordCommandService(
			faceLogger,
			fakeUnsettlementService,
			faceSettlementService);

		await service.ExecuteAsync(command);

		_ = fakeUnsettlementService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(
				new PullRecordUnsettlementCommand(
					ApiUrl: command.ApiUrl,
					MerchantId: command.MerchantId,
					ClientId: command.ClientId,
					ClientSecret: command.ClientSecret,
					StartTime: command.StartTime,
					EndTime: command.EndTime)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = faceSettlementService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(
				new PullRecordSettlementCommand(
					ApiUrl: command.ApiUrl,
					MerchantId: command.MerchantId,
					ClientId: command.ClientId,
					ClientSecret: command.ClientSecret,
					StartTime: command.StartTime,
					EndTime: command.EndTime)),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
