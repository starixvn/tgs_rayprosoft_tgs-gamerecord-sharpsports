using Tgs.GameCommand.Client;
using Tgs.GameInterface.SharpSports.Client;
using Tgs.GameRecord.SharpSports;
using Tgs.GameRecord.SharpSports.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecord(
	   this IServiceCollection services,
	   Action<RedisOptions, IServiceProvider> redisDBconfigure,
	   Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
	   Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
	   Action<GameInterfaceSharpSportsClientOptions, IServiceProvider> gameInterfaceSharpSportsClientOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisDBconfigure)
		.Services
		.AddOptions<GameRecordInfoOptions>()
		.Configure(gameRecordInfoOptions)
		.Services
		.AddSingleton<RedisConnectionManager>()
		.AddGameRecordCore(
			gameCommandClientOptions,
			gameInterfaceSharpSportsClientOptions)
		.AddGameRecordRepository(
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
