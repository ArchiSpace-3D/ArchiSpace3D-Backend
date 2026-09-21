import os

file_path = r"C:\Users\angel\OneDrive\Desktop\Archie\Backend\ArchiSpace3D.Api\ArchiSpace3D.Api\Service\proyectoService.cs"
with open(file_path, "r", encoding="utf-8") as f:
    content = f.read()

old_ctor = """        private readonly proyectoDAOImpl _proyectoDao;

        public proyectoService(proyectoDAOImpl proyectoDao)
        {
            _proyectoDao = proyectoDao;
        }"""
new_ctor = """        private readonly proyectoDAOImpl _proyectoDao;
        private readonly notificacionServiceImpl _notificacionService;

        public proyectoService(proyectoDAOImpl proyectoDao, notificacionServiceImpl notificacionService)
        {
            _proyectoDao = proyectoDao;
            _notificacionService = notificacionService;
        }"""

old_update = """        public async Task<bool> ActualizarAsync(Proyecto proyecto) => await _proyectoDao.UpdateAsync(proyecto);"""
new_update = """        public async Task<bool> ActualizarAsync(Proyecto proyecto)
        {
            var result = await _proyectoDao.UpdateAsync(proyecto);
            if (result)
            {
                await _notificacionService.NotificarCambioAsync(
                    proyecto.Idproyecto, 
                    "Edicion", 
                    $"El proyecto ha sido modificado.");
            }
            return result;
        }"""

content = content.replace(old_ctor, new_ctor)
content = content.replace(old_update, new_update)

with open(file_path, "w", encoding="utf-8") as f:
    f.write(content)
