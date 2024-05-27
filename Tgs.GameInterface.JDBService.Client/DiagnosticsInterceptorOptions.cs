using System.Diagnostics;

namespace Tgs.GameInterface.JDBService.Client;

public class DiagnosticsInterceptorOptions
{
	public ActivitySource InterceptorActivitySource { get; set; } =
		new ActivitySource($"Tgs.GameInterface.JDBService.Client.DiagnosticsInterceptor");
}
