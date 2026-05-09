using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;

namespace Knack.API.Models
{
    public class AISqlQueryResponse
    {
        [JsonPropertyName("chatresponse")]
        public string ChatResponse { get; set; }

        [JsonPropertyName("sql")]
        public string Sql { get; set; }
    }
}
