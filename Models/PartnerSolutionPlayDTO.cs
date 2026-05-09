using Knack.DBEntities;

namespace Knack.API.Models
{
    public class PartnerSolutionPlayDTO
    {
        public Guid? PartnerSolutionPlayId { get; set; }
        public Guid? SolutionAreaId { get; set; }
        public Guid? OrgId { get; set; }
        public List<string>? SelectedGeos { get; set; }
        public string? SolutionPlayDescription { get; set; }
        public string? SolutionPlayOrgWebsite { get; set; }
        public string? SolutionPlayName { get; set; }
        public string? MarketplaceLink { get; set; }
        public string? SpecialOfferLink { get; set; }
        public string? LogoFileLink { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public int? IsPublished { get; set; }
        public Guid? RowChangedBy { get; set; }
        public string? PartnerSolutionPlaySlug { get; set; }
        public string? Image_Thumb { get; set; }
        public string? Image_Main { get; set; }
        public string? Image_Mobile { get; set; }
        public int? IndustryDesignation { get; set; }
        public List<PartnerSolutionPlayByPlayDTO>? PartnerSolutionPlays { get; set; }
        public List<PartnerSolutionPlayAvailableGeo>? PartnerSolutionPlayAvailableGeo { get; set; }


    }

    public class PartnerSolutionPlayByPlayDTO
    {
        public Guid? PartnerSolutionPlayByPlayId { get; set; }
        public Guid? SolutionPlayId { get; set; }
        public string? PlaySolutionDescription { get; set; }
        public List<PartnerSolutionPlayResourceLinkDTO> PartnerSolutionPlayResourceLinks { get; set; }
    }

    public class PartnerSolutionPlayResourceLinkDTO
    {
        public Guid? resourceLinkId { get; set; }
        public string resourceLinkTitle { get; set; }
        public string resourceLinkUrl { get; set; }
        public string? resourceLinkName { get; set; }
        public string? resourceLinkOverview { get; set; }
    }

    public class PartnerSolutionPlaySimplifiedDTO
    {
        public Guid? PartnerSolutionPlayId { get; set; }
        public Guid? OrgId { get; set; }
        public Guid? SolutionAreaId { get; set; }
        public string? OrgName { get; set; }
        public string? SolutionPlayDescription { get; set; }
        public string SolutionPlayOrgWebsite { get; set; }
        public string PartnerSolutionPlaySlug { get; set; }
        public string? SolutionPlayName { get; set; }
        public string MarketplaceLink { get; set; }
        public string? SpecialOfferLink { get; set; }
        public string? LogoFileLink { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public string? DisplayLabel { get; set; }
        public int? IsPublished { get; set; }
        public Guid? RowChangedBy { get; set; }
        public string? Image_Thumb { get; set; }
        public string? Image_Main { get; set; }
        public string? Image_Mobile { get; set; }
        public int? IndustryDesignation { get; set; }
        public List<PartnerSolutionPlayByPlayDTO>? PartnerSolutionPlays { get; set; }
        public List<PartnerSolutionPlayAvailableGeo>? PartnerSolutionPlayAvailableGeo { get; set; }
        public List<PartnerSolutionPlayGeos>? Geos { get; set; }
        public SolutionAreaWithSolutionPlay? SolutionAreaWithSolutionPlay { get; set; }
    }
    public class PartnerSolutionPlayGeos
    {
        public Guid? GeoId { get; set; }
        public string? Geoname { get; set; }
    }
    public class SolutionAreaWithSolutionPlay
    {
        public Guid? SolutionPlayId { get; set; }
        public Guid? SolutionAreaId { get; set; }
        public string? SolutionAreaDescription { get; set; }
        public string? SolutionAreaName { get; set; }
        public string? PartnerSolutionPlayName { get; set; }
        public string? SolutionPlayName { get; set; }
        public string? SolutionPlayThemeSlug { get; set; }
        public string? SolutionPlayLabel { get; set; }
        public string? SolutionAreaSlug { get; set; } = null!;

    }
    public class PartnerSolutionPlayViewDTO
    {
        public Guid? PartnerSolutionPlayId { get; set; }
        public Guid? OrgId { get; set; }
        public Guid? SolutionAreaId { get; set; }
        public string? OrgName { get; set; }
        public string? SolutionPlayDescription { get; set; }
        public string SolutionPlayOrgWebsite { get; set; }
        public string PartnerSolutionPlaySlug { get; set; }
        public string? SolutionPlayName { get; set; }
        public string? SolutionPlayTitle { get; set; }
        public string MarketplaceLink { get; set; }
        public string? SpecialOfferLink { get; set; }
        public string? LogoFileLink { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public string? DisplayLabel { get; set; }
        public int? IsPublished { get; set; }
        public Guid? RowChangedBy { get; set; }
        public string? Image_Thumb { get; set; }
        public string? Image_Main { get; set; }
        public string? Image_Mobile { get; set; }
        public Boolean? show { get; set; }
        public Guid? SolutionPlayId { get;set; }
    }
    /*
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
        public List<GetPartnerSolutionAvailableGeoDTO>? Geos { get; set; }
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

    }
    public class PartnerSolutionIdDTO
    {
        public Guid? PartnerSolutionId { get; set; }
    }*/
}