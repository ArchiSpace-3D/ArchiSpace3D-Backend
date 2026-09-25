# ArchiSpace3D Backend

Este es el repositorio del Backend (API REST) para ArchiSpace3D, una plataforma innovadora que permite a arquitectos y clientes colaborar y visualizar espacios arquitectónicos en 3D y Realidad Aumentada (AR).

## 🚀 Tecnologías Principales

*   **Framework:** ASP.NET Core (.NET 7/8)
*   **Base de Datos:** PostgreSQL (alojada en Supabase)
*   **ORM:** Entity Framework Core
*   **Autenticación:** JSON Web Tokens (JWT) & Google OAuth
*   **Notificaciones:** Firebase Cloud Messaging (FCM)
*   **Hosting:** Railway

## 📂 Estructura del Proyecto

*   **Controllers:** Controladores que exponen los endpoints de la API (Usuarios, Proyectos, Invitaciones, Sugerencias, Mediciones, etc.).
*   **Models:** Entidades que mapean directamente con la base de datos de PostgreSQL.
*   **Dao:** Data Access Object (Patrón DAO) para abstraer las consultas a Entity Framework.
*   **Service:** Lógica de negocio de la aplicación.
*   **Config:** Archivos de configuración (Ej. credenciales de Firebase).

## 🛠 Configuración Local

1.  **Clonar el repositorio:**
    ```bash
    git clone https://github.com/ArchiSpace-3D/ArchiSpace3D-Backend.git
    cd ArchiSpace3D-Backend
    ```

2.  **Configurar Variables de Entorno / AppSettings:**
    *   Asegúrate de contar con la cadena de conexión a Supabase en `appsettings.json` o a través de Secrets de usuario.
    *   (Opcional para pruebas locales) Configura la variable de entorno `FIREBASE_CREDENTIALS_JSON` con las credenciales de Firebase si deseas probar notificaciones Push.

3.  **Ejecutar el proyecto:**
    ```bash
    dotnet run --project ArchiSpace3D.Api/ArchiSpace3D.Api.csproj
    ```

4.  **Explorar la API:**
    *   Una vez en ejecución, puedes acceder a la interfaz de Swagger en `https://localhost:<puerto>/swagger`.

## 🤝 Ramas del Repositorio

*   `main`: Rama principal de producción.
*   `Karen` / `Angel`: Ramas de desarrollo y nuevas funcionalidades.
