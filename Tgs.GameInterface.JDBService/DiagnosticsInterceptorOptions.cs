using System.Diagnostics;

namespace Tgs.GameInterface.JDBService;

public class DiagnosticsInterceptorOptions
{
    public ActivitySource InterceptorActivitySource { get; set; } =
        new ActivitySource($"Tgs.GameInterface.JDBService.DiagnosticsInterceptor");
}
