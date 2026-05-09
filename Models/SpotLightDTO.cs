using Knack.DBEntities;

namespace Knack.API.Models
{
    public class SpotLightDTO
    {
        public Guid SpotlightId { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid? UsecaseId { get; set; }

        public Guid? PartnerSolutionId { get; set; }

        public string? SpotlightOverview { get; set; }
    }
}
