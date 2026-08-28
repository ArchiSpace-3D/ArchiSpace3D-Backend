using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchiSpace3D.Api.Models
{
    public partial class Espaciofisico
    {
        [NotMapped]
        public List<puntosReferencia> PuntosReferenciaTyped
        {
            get => string.IsNullOrEmpty(Puntosreferencia)
                ? new List<puntosReferencia>()
                : JsonSerializer.Deserialize<List<puntosReferencia>>(Puntosreferencia)!;
            set => Puntosreferencia = JsonSerializer.Serialize(value);
        }
    }
}