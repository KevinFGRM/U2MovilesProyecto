using Android.App;
using Android.Content.PM;
using Android.Gms.Extensions;
using Android.OS;
using Firebase;
using Firebase.Messaging;

namespace MiniJokeRPGAPP
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override async void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Firebase.FirebaseApp.InitializeApp(this);
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
                RequestPermissions(new[] { "android.permission.POST_NOTIFICATIONS" }, 0);

            var tokenResult = await FirebaseMessaging.Instance.GetToken().AsAsync<Java.Lang.String>();
            var token = tokenResult?.ToString() ?? "";

            System.Diagnostics.Debug.WriteLine($"Token FCM: {token}");
            Preferences.Set("fcm_token", token);
        }
    }
}
