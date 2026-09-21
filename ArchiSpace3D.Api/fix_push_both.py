import os
import re

file_path = r"C:\Users\angel\OneDrive\Desktop\Archie\Backend\ArchiSpace3D.Api\ArchiSpace3D.Api\Service\notificacionService.cs"
with open(file_path, "r", encoding="utf-8") as f:
    content = f.read()

old_push = """        private async Task EmpujarPushAsync(Notificacion notificacion)
        {
            Console.WriteLine($"🔍 EmpujarPushAsync: buscando proyecto {notificacion.Idproyecto}");

            var proyecto = await _proyectoDao.GetByIdAsync(notificacion.Idproyecto);
            if (proyecto is null)
            {
                Console.WriteLine("❌ Proyecto no encontrado.");
                return;
            }

            Console.WriteLine($"✅ Proyecto encontrado. Idcliente = {proyecto.Idcliente}");

            var cliente = await _usuarioDao.GetByIdAsync(proyecto.Idcliente);
            if (cliente is null)
            {
                Console.WriteLine("❌ Cliente no encontrado.");
                return;
            }

            Console.WriteLine($"✅ Cliente encontrado: {cliente.Email}. FcmToken = '{cliente.Fcmtoken}'");

            if (string.IsNullOrEmpty(cliente.Fcmtoken))
            {
                Console.WriteLine("⚠️ El cliente no tiene FcmToken guardado.");
                return;
            }

            await _pushService.EnviarNotificacionAsync(
                cliente.Fcmtoken,
                titulo: proyecto.Nombre,
                cuerpo: notificacion.Mensaje,
                data: new Dictionary<string, string>
                {
            { "idProyecto", notificacion.Idproyecto.ToString() },
            { "tipo", notificacion.Tipo ?? "" }
                }
            );
        }"""

new_push = """        private async Task EmpujarPushAsync(Notificacion notificacion)
        {
            Console.WriteLine($"🔍 EmpujarPushAsync: buscando proyecto {notificacion.Idproyecto}");

            var proyecto = await _proyectoDao.GetByIdAsync(notificacion.Idproyecto);
            if (proyecto is null) return;

            // Enviar al cliente
            var cliente = await _usuarioDao.GetByIdAsync(proyecto.Idcliente);
            if (cliente != null && !string.IsNullOrEmpty(cliente.Fcmtoken))
            {
                await _pushService.EnviarNotificacionAsync(
                    cliente.Fcmtoken,
                    titulo: proyecto.Nombre,
                    cuerpo: notificacion.Mensaje,
                    data: new Dictionary<string, string> { { "idProyecto", notificacion.Idproyecto.ToString() }, { "tipo", notificacion.Tipo ?? "" } }
                );
            }

            // Enviar al arquitecto (para que también le lleguen las alertas a su celular al probar)
            var arquitecto = await _usuarioDao.GetByIdAsync(proyecto.Idarquitecto);
            if (arquitecto != null && !string.IsNullOrEmpty(arquitecto.Fcmtoken))
            {
                await _pushService.EnviarNotificacionAsync(
                    arquitecto.Fcmtoken,
                    titulo: proyecto.Nombre,
                    cuerpo: notificacion.Mensaje,
                    data: new Dictionary<string, string> { { "idProyecto", notificacion.Idproyecto.ToString() }, { "tipo", notificacion.Tipo ?? "" } }
                );
            }
        }"""

content = content.replace(old_push, new_push)

with open(file_path, "w", encoding="utf-8") as f:
    f.write(content)
