namespace Knack.API.Models
{
    public class GetPartnerSolutionDTO
    {
        public Guid? PartnerSolutionId { get; set; }
        public Guid? IndustryId { get; set; }
        public Guid? SubIndustryId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string? OrgName { get; set; }
        public string? SolutionDescription { get; set; }
        public string? SolutionName { get; set; }
        public string? LogoFileLink { get; set; }
        public string? PartnerSolutionSlug { get; set; }
        public Boolean? show { get; set; }

        public int? isPublished { get; set; }
    }

    
}
