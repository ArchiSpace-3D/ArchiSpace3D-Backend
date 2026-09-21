using FirebaseAdmin.Messaging;

namespace ArchiSpace3D.Api.Service
{
    public class pushNotificationService : pushNotificationServiceImpl
    {
        public async Task EnviarNotificacionAsync(string token, string titulo, string cuerpo, Dictionary<string, string>? data = null)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("⚠️ EnviarNotificacionAsync: token vacío, no se envía nada.");
                return;
            }

            var message = new Message()
            {
                Token = token,
                Notification = new Notification
                {
                    Title = titulo,
                    Body = cuerpo
                },
                Data = data
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"✅ Push enviado. MessageId: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error enviando push: {ex.GetType().Name} - {ex.Message}");
            }
        }
    }
}