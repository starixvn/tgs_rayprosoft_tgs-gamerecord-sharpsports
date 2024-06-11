using Tgs.GameRecord.SharpSports.Core.JobApplicationServices;

namespace Tgs.GameRecord.SharpSports.Core;

public interface IHangfireUtensil
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}