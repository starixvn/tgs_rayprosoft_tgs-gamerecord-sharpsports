using Tgs.GameRecord.SharpSports.Core.JobApplicationServices;

namespace Tgs.GameRecord.SharpSports.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
