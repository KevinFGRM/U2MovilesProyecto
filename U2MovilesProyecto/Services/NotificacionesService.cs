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
        private readonly IConfiguration configuration;
        private readonly ILogger<NotificacionesService> logger;

        private bool firebaseInicializado;

        public NotificacionesService(
            Repository<Fcmtokens> repository,
            IConfiguration configuration,
            ILogger<NotificacionesService> logger)
        {
            this.repository = repository;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task EnviarNotificacionAsync(int idUsuario, string titulo, string mensaje)
        {
            if (!InicializarFirebase())
                return;

            var tokens = repository.Query()
                .Where(x => x.IdUsuario == idUsuario)
                .Select(x => x.Token)
                .ToList();

            if (!tokens.Any())
            {
                logger.LogInformation(
                    "El usuario {IdUsuario} no tiene tokens FCM registrados.",
                    idUsuario);

                return;
            }

            var ms = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new Notification
                {
                    Title = titulo,
                    Body = mensaje
                }
            };

            try
            {
                var resultado = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(ms);

                logger.LogInformation(
                    "Notificación enviada. Exitosas: {Success}, Fallidas: {Failed}",
                    resultado.SuccessCount,
                    resultado.FailureCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error enviando notificación Firebase.");
            }
        }

        private bool InicializarFirebase()
        {
            if (firebaseInicializado ||
                FirebaseApp.DefaultInstance != null)
            {
                firebaseInicializado = true;
                return true;
            }

            try
            {
                var rutaArchivo = configuration["Firebase:ServiceAccountPath"];

                if (string.IsNullOrWhiteSpace(rutaArchivo) ||
                    !File.Exists(rutaArchivo))
                {
                    logger.LogWarning("No se encontró el archivo Firebase en {Ruta}", rutaArchivo);

                    return false;
                }

                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(rutaArchivo)
                });

                firebaseInicializado = true;

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "No se pudo inicializar Firebase.");

                return false;
            }
        }
    }
}