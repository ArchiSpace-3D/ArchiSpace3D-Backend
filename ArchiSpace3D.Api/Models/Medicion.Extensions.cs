using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchiSpace3D.Api.Models
{
    public partial class Medicion
    {
        [NotMapped]
        public puntosReferencia PuntoInicialTyped
        {
            get => JsonSerializer.Deserialize<puntosReferencia>(Puntoinicial)!;
            set => Puntoinicial = JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public puntosReferencia PuntoFinalTyped
        {
            get => JsonSerializer.Deserialize<puntosReferencia>(Puntofinal)!;
            set => Puntofinal = JsonSerializer.Serialize(value);
        }
    }
}