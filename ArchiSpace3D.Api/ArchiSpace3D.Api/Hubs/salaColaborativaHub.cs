using Microsoft.AspNetCore.SignalR;

namespace ArchiSpace3D.Api.Hubs
{ 
    public class SalaColaborativaHub : Hub
    {
        public async Task UnirseASala(int idProyecto)
        {
            var grupo = idProyecto.ToString();
            await Groups.AddToGroupAsync(Context.ConnectionId, grupo);
            await Clients.OthersInGroup(grupo).SendAsync("UsuarioConectado", Context.ConnectionId);
        }

        public async Task SalirDeSala(int idProyecto)
        {
            var grupo = idProyecto.ToString();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupo);
            await Clients.OthersInGroup(grupo).SendAsync("UsuarioDesconectado", Context.ConnectionId);
        }
    }
}