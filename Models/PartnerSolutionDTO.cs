using Knack.DBEntities;

namespace Knack.API.Models
{
    public class PartnerSolutionDTO
    {
        public Guid? PartnerSolutionId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? IndustryId { get; set; }
        public Guid? SubIndustryId { get; set; }
        public Guid? OrganizationId { get; set; }
        public List<string>? SelectedGeos { get; set; }
        public string? SolutionDescription { get; set; }
        public string SolutionOrgWebsite { get; set; }
        public string? SolutionName { get; set; }
        public string MarketplaceLink { get; set; }
        public string? SpecialOfferLink { get; set; }
        public string? LogoFileLink { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public int? IsPublished { get; set; }
        public Guid? RowChangedBy { get; set; }
        public string? PartnerSolutionSlug { get; set; }
        public int? IndustryDesignation { get; set; }

        public string? ParentSolutionId { get; set; }
        public List<PartnerSolutionAreaDTO>? PartnerSolutionAreas { get; set; }

        public List<PartnerSolutionAvailableGeoDTO>? PartnerSolutionAvailableGeo { get; set; }

        public SpotLightDTO? SpotLight { get; set; }
    }

    public class PartnerSolutionAreaDTO
    {
        public Guid? partnersolutionByAreaId { get; set; }
        public Guid? solutionAreaId { get; set; }
        public string? areaSolutionDescription { get; set; }
        public Guid? ParentSolutionId { get; set; }
        public List<PartnerSolutionResourceLinkDTO> PartnerSolutionResourceLinks { get; set; }
    }

    public class PartnerSolutionResourceLinkDTO
    {
        public Guid? resourceLinkId { get; set; }
        public string resourceLinkTitle { get; set; }
        public string resourceLinkUrl { get; set; }
        public string? resourceLinkOverview { get; set; }
        public Guid? PartnerSolutionByAreaId { get; set; }
        public DateTime? eventDateTime { get; set; }
    }

    public class PartnerSolutionSimplifiedDTO
    {
        public Guid? PartnerSolutionId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? IndustryId { get; set; }
        public Guid? SubIndustryId { get; set; }
        public string? SolutionDescription { get; set; }
        public string SolutionOrgWebsite { get; set; }
        public string? SolutionName { get; set; }
        public string MarketplaceLink { get; set; }
        public string? SpecialOfferLink { get; set; }
        public string? LogoFileLink { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public int? IsPublished { get; set; }
        public Guid? RowChangedBy { get; set; }
        public int? IndustryDesignation { get; set; }
        public List<PartnerSolutionAreaDTO>? PartnerSolutionAreas { get; set; }

        public List<PartnerSolutionAvailableGeo>? PartnerSolutionAvailableGeo { get; set; }

        public SpotLightDTO? SpotLight { get; set; }
    }
    public class PartnerSolutionViewDTO
    {
        public Guid? PartnerSolutionId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? IndustryId { get; set; }
        public Guid? SubIndustryId { get; set; }
        public string? SolutionDescription { get; set; }
        public string SolutionOrgWebsite { get; set; }
        public string? SolutionName { get; set; }
        public string MarketplaceLink { get; set; }
        public string? SpecialOfferLink { get; set; }
        public string? LogoFileLink { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public int? IsPublished { get; set; }
        public Guid? RowChangedBy { get; set; }
        public string? OrgName { get; set; }
        public List<PartnerSolutionAreaViewDTO>? PartnerSolutionAreas { get; set; }
        public List<PartnerSolutionAvailableGeoDTO>? Geos { get; set; }
        public SpotLightDTO? SpotLight { get; set; }
        public Boolean? hasMultipleTheme { get; set; }
        public Guid? IndustryThemeId { get; set; }
        public string? IndustrySlug { get; set; }
        public string? SubIndustrySlug { get; set; }
        public string? IndustryThemeSlug { get; set; }
    }
    public class PartnerSolutionAreaViewDTO
    {
        public Guid? partnersolutionByAreaId { get; set; }
        public Guid? solutionAreaId { get; set; }
        public string? areaSolutionDescription { get; set; }
        public string? solutionAreaName { get; set; }
        public List<PartnerSolutionResourceLinkViewDTO> PartnerSolutionResourceLinks { get; set; }
    }

    public class PartnerSolutionResourceLinkViewDTO
    {
        public Guid? resourceLinkId { get; set; }
        public string resourceLinkName { get; set; }
        public string resourceLinkTitle { get; set; }
        public string resourceLinkUrl { get; set; }
        public string? resourceLinkOverview { get; set; }
        public DateTime? eventDateTime { get; set; }
    }
    public class PartnerSolutionIdDTO
    {
        public Guid? PartnerSolutionId { get; set; }
    }
}