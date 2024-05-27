using Tgs.GameInterface.JDBService.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameInterfaceService(
		this IServiceCollection services,
		 Action<GameInterfaceSettingOptions, IServiceProvider> gameInterfaceSetting)
		=> services
			.AddGameInterfaceCore(gameInterfaceSetting);
}
