using StackExchange.Redis;
using Tgs.GameRecord.SharpSports.Core;
using Tgs.GameRecord.SharpSports.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordRepository(
		this IServiceCollection services,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.AddTransient<IHangfireUtensilRepository, HangfireUtensilRepository>()
		.AddTransient<IHangfireLockRepository>(sp => ActivatorUtilities.CreateInstance<HangfireLockRepository>(sp, redisDatabaseResolver(sp)));
}
