using Tgs.GameRecord.SharpSports.Core.ApplicationServices;

namespace Tgs.GameRecord.SharpSports.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}
