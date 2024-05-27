using System.Reflection;
using Tgs.GameInterface.JDBService.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameInterfaceCore(
		this IServiceCollection services,
		Action<GameInterfaceSettingOptions, IServiceProvider> gameInterfaceSetting)
		=> services
		.AddOptions<GameInterfaceSettingOptions>()
		.Configure(gameInterfaceSetting)
		.Services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddHttpClient<IGameApiClient, GameApiClient>()
		.Services
		.AddSingleton(TimeProvider.System)
		.AddSingleton<IAesCryptography, AesCryptography>();
}
