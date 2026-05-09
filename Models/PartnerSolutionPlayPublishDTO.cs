using Knack.DBEntities;

namespace Knack.API.Models
{
    public class PartnerSolutionPlayPublishDTO
    {
        public Guid? PartnerSolutionPlayId { get; set; }
        public int? IsPublished { get; set; }
    }
}
