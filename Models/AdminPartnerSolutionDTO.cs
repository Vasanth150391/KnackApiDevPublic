using Knack.DBEntities;

namespace Knack.API.Models
{
    public class AdminPartnerSolutionDTO
    {
        public Guid? IndustryId { get; set; }
        public Guid? SubIndustryId { get; set; }
        public string IndustryName { get; set; }
        public string? SubIndustryName { get; set; }
        public int? NoofSolutions { get; set; }
        public int? NoofApproved { get; set; }
        public int? NoofPublish { get; set; }
    }
}