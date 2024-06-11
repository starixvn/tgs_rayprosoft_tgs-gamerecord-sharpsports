using System.Reflection;

using Tgs.GameCommand.Client;
using Tgs.GameInterface.SharpSports.Client;
using Tgs.GameRecord.SharpSports.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordCore(
		this IServiceCollection services,
		Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
		Action<GameInterfaceSharpSportsClientOptions, IServiceProvider> gameInterfaceSharpSportsClientOptions)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddGameCommandClient(gameCommandClientOptions)
		.AddGameInterfaceSharpSportsClient(gameInterfaceSharpSportsClientOptions)
		.AddSingleton(TimeProvider.System)
		.AddSingleton<IPullRecordDispatcher, PullRecordDispatcher>()
		.AddTransient<IHangfireUtensil, HangfireUtensil>();
}