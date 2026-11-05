using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArchiSpace3D.Api.Models
{
    public partial class Espaciofisico
    {
        private static readonly JsonSerializerOptions _caseInsensitiveOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [NotMapped]
        [JsonIgnore]
        public List<puntosReferencia> PuntosReferenciaTyped
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Puntosreferencia))
                    return new List<puntosReferencia>();
                try
                {
                    return JsonSerializer.Deserialize<List<puntosReferencia>>(Puntosreferencia, _caseInsensitiveOptions)
                        ?? new List<puntosReferencia>();
                }
                catch
                {
                    return new List<puntosReferencia>();
                }
            }
            set => Puntosreferencia = JsonSerializer.Serialize(value);
        }
    }
}
