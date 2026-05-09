using Knack.DBEntities;

namespace Knack.API.Models
{
    public class PartnerSolutionStatusDTO
    {
        public Guid? PartnerSolutionId { get; set; }
        public Guid? SolutionStatusId { get; set; }
    }
}
