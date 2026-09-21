namespace ArchiSpace3D.Api.Service
{
    public interface pushNotificationServiceImpl
    {
        Task EnviarNotificacionAsync(string token, string titulo, string cuerpo, Dictionary<string, string>? data = null);
    }
}