using Hangfire.Dashboard;

namespace Tgs.GameRecord.SharpSports;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}