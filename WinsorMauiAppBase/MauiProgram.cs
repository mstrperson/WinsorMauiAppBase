using Microsoft.Extensions.Logging;
using WinsorMauiAppBase.Services;

namespace WinsorMauiAppBase;

public static class MauiProgram
{
    private static CancellationToken closeProgramToken;
    public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
            {
                fonts.AddFont("CrimsonText-Semibold.ttf", "Serif");
                fonts.AddFont("NotoSans-Regular.ttf", "SansSerif");
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});


        closeProgramToken = new CancellationToken();

        ApiService.MaintainAuthorization(closeProgramToken);

        var app = builder.Build();

        return builder.Build();
	}
}
