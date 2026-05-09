using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Knack.API.Models
{
    public partial class TokenResponseModel
    {
        [JsonProperty("expires-at")]
        public string ExpiresAt { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
