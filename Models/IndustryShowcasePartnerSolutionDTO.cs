namespace Knack.API.Models
{
    public class IndustryShowcasePartnerSolutionDTO
    {
        public Guid? IndustryShowcasePartnerSolutionId { get; set; } = null!;

        public Guid? IndustryThemeId { get; set; } = null!;

        public Guid? PartnerId { get; set; } = null!;

        public string PartnerName { get; set; } = null!;

        public string MarketPlaceLink { get; set; } = null!;
        public string? logoFileLink { get; set; } = null!;
        public string? websiteLink { get; set; } = null!;
        public string? overviewDescription { get; set; } = null!;
        public Guid? OrgId { get; set; } = null!;
        public string? IndustrySlug { get; set; }
        public string? SubIndustrySlug { get; set; }
        public string? PartnerNameSlug { get; set; }
    }
    public class GetIndustryShowcasePartnerSolutionDTO
    {
        public Guid? IndustryShowcasePartnerSolutionId { get; set; } = null!;

        public Guid? IndustryThemeId { get; set; } = null!;

        public Guid? IndustryId { get; set; } = null!;
        public Guid? SubIndustryId { get; set; } = null!;

        public Guid? PartnerId { get; set; } = null!;

        public string PartnerName { get; set; } = null!;

        public string MarketPlaceLink { get; set; } = null!;
        public string? logoFileLink { get; set; } = null!;
        public string? websiteLink { get; set; } = null!;
        public string? overviewDescription { get; set; } = null!;
        public Guid? OrgId { get; set; } = null!;
        public Boolean? hasMultipleTheme { get; set; }
        public string? IndustrySlug { get; set; }
        public string? SubIndustrySlug { get; set; }
        public string? PartnerNameSlug { get; set; }
        public string? IndustryThemeSlug { get; set; }
    }
}
