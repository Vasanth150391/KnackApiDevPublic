using Knack.DBEntities;

namespace Knack.API.Models
{
    public class ResponseDTO
    {
        public string? Message { get; set; }
        public Boolean? Response { get; set; }
    }
    public class ResponseErrortDTO
    {
        public string? Error { get; set; }
        public Boolean? Response { get; set; }
    }
}
