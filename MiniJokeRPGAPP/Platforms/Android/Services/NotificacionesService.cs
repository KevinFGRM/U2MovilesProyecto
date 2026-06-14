using Android.App;
using Android.Content;
using Firebase.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using static Android.Resource;

namespace MiniJokeRPGAPP.Platforms.Android.Services
{

    [Service(Exported = false)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class NotificacionesService : FirebaseMessagingService
    {
        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
            Preferences.Set("fcm_token", token);
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);

            var title = message.GetNotification()?.Title ?? "Sin título";
            var body = message.GetNotification()?.Body ?? "Sin cuerpo";


            ShowNotification(title, body);
        }

        private void ShowNotification(string title, string body)
        {
            if (ApplicationContext != null)
            {
                var channelId = "default_channel";

                var intent = new Intent(ApplicationContext, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);

                var pendingIntent = PendingIntent.GetActivity(ApplicationContext, 0, intent,
                    PendingIntentFlags.OneShot | PendingIntentFlags.Immutable);

                var builder = new Notification.Builder(ApplicationContext, channelId)
                    .SetSmallIcon(Drawable.IcDialogInfo)
                    .SetContentTitle(title)
                    .SetContentText(body)
                    .SetAutoCancel(true)
                    .SetContentIntent(pendingIntent);

                var manager = (NotificationManager)ApplicationContext
                    .GetSystemService(Context.NotificationService)!;

                var channel = new NotificationChannel(channelId, "General",
                    NotificationImportance.Default);
                manager.CreateNotificationChannel(channel);

                manager.Notify(0, builder.Build());
            }
        }
    }
}