using AvisosAPI.Repositories;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MiniJokeRPGAPI.Models.Entities;

namespace U2MovilesProyecto.Services
{
    public class NotificacionesService
    {
        private readonly Repository<Fcmtokens> repository;

        public NotificacionesService(Repository<Fcmtokens> repository)
        {
            this.repository = repository;
            // codigo con ia, para evitar errores. ya que el original no dejaba.
            // Solo inicializar Firebase en entornos que lo soportan
            if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS() || OperatingSystem.IsMacOS())
            {
                try
                {
                    if (FirebaseApp.DefaultInstance == null)
                    {
                        FirebaseApp.Create(new AppOptions()
                        {
                            Credential = GoogleCredential.FromFile("wwwroot/fcmkey.json")
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Firebase no disponible: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Firebase no está disponible en este sistema operativo");
            }
        }

        public void EnviarNotificacion(int idUsuario, string titulo, string mensaje)
        {
            var tokens = repository.Query().Where(x => x.IdUsuario == idUsuario).Select(x => x.Token).ToList();

            if (!tokens.Any())
                return;

            var ms = new MulticastMessage()
            {
                Tokens = tokens,

                Notification = new Notification()
                {
                    Title = titulo,
                    Body = mensaje
                }
            };

            FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(ms);
        }
    }
}
