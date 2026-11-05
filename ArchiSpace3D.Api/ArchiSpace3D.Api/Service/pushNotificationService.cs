using FirebaseAdmin;
using FirebaseAdmin.Messaging;

namespace ArchiSpace3D.Api.Service
{
    public class pushNotificationService : pushNotificationServiceImpl
    {
        public async Task EnviarNotificacionAsync(string token, string titulo, string cuerpo, Dictionary<string, string>? data = null)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            if (FirebaseApp.DefaultInstance == null || FirebaseMessaging.DefaultInstance == null)
            {
                return;
            }

#pragma warning disable CS0618
            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = titulo,
                    Body = cuerpo
                },
                Data = data
            };
#pragma warning restore CS0618

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"Push enviado. MessageId: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando push: {ex.GetType().Name} - {ex.Message}");
            }
        }
    }
}
