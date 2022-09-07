using Microsoft.Extensions.Options;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameInterface.JDBService.Protos;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameInterfaceJDBServiceClient(
        this IServiceCollection services,
        Action<GameInterfaceJDBClientOptions, IServiceProvider> configure)
        => services
            .AddOptions<GameInterfaceJDBClientOptions>()
            .Configure(configure)
            .Services
            .AddGrpcClient<GameInterfaceJDBService.GameInterfaceJDBServiceClient>((sp, configureClient) =>
            {
                var options = sp.GetRequiredService<IOptions<GameInterfaceJDBClientOptions>>().Value;
                configureClient.Address = options.BaseUrl;
            })
            .ConfigureChannel((sp, configureChannel) =>
            {
                var handler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    EnableMultipleHttp2Connections = true
                };
                configureChannel.HttpHandler = handler;
            })
            .AddInterceptor<LoggingInterceptor>()
            .Services
            .AddSingleton<DiagnosticsInterceptorOptions>()
            .AddSingleton<LoggingInterceptor>()
            .AddTransient<IGameInterfaceJDBClient, GameInterfaceJDBClient>();
}
