using Knack.DBEntities;

namespace Knack.API.Models
{
    public class PartnerSolutionPublishDTO
    {
        public Guid? PartnerSolutionId { get; set; }
        public int? IsPublished { get; set; }
    }
}
