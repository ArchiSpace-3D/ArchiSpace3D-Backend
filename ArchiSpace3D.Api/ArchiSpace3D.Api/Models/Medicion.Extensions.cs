using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArchiSpace3D.Api.Models
{
    public partial class Medicion
    {
        private static readonly JsonSerializerOptions _caseInsensitiveOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [NotMapped]
        [JsonIgnore]
        public puntosReferencia PuntoInicialTyped
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Puntoinicial))
                    return new puntosReferencia();
                try
                {
                    return JsonSerializer.Deserialize<puntosReferencia>(Puntoinicial, _caseInsensitiveOptions)
                        ?? new puntosReferencia();
                }
                catch
                {
                    return new puntosReferencia();
                }
            }
            set => Puntoinicial = JsonSerializer.Serialize(value);
        }

        [NotMapped]
        [JsonIgnore]
        public puntosReferencia PuntoFinalTyped
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Puntofinal))
                    return new puntosReferencia();
                try
                {
                    return JsonSerializer.Deserialize<puntosReferencia>(Puntofinal, _caseInsensitiveOptions)
                        ?? new puntosReferencia();
                }
                catch
                {
                    return new puntosReferencia();
                }
            }
            set => Puntofinal = JsonSerializer.Serialize(value);
        }
    }
}
