using Java.Nio.FileNio;
using Microsoft.Extensions.Logging;
using MiniJokeRPGAPP.Services;
using MiniJokeRPGAPP.ViewModels;

namespace MiniJokeRPGAPP
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<AmigosService>();
            builder.Services.AddSingleton<MensajesService>();
            builder.Services.AddSingleton<PersonajesService>();
            builder.Services.AddSingleton<PartidasService>();


            builder.Services.AddTransient<AuthViewModel>();
            builder.Services.AddTransient<AmigosViewModel>();
            builder.Services.AddTransient<MensajesViewModel>();
            builder.Services.AddTransient<PartidasViewModel>();
#endif

            return builder.Build();
        }
    }
}
