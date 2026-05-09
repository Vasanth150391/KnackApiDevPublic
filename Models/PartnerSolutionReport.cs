namespace Knack.API.Models
{
    public class PartnerSolutionReport
    {
        public string OrgName { get; set; }
        public Guid? OrgId { get; set; }
        public string IndustryName { get; set; }
        public string SubIndustryName { get; set; }
        public Guid? SubIndustryId { get; set; }
        public string SolutionName { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public string SolutionStatus { get; set; }
        public DateTime RowChangedDate { get; set; }
    }
    public class PartnerSolutionReportCSV
    {
        public string OrgName { get; set; }
        public Guid? OrgId { get; set; }
        public string IndustryName { get; set; }
        public string SubIndustryName { get; set; }
        public Guid? SubIndustryId { get; set; }
        public string? SolutionName { get; set; }
        public string? solutionOrgWebsite { get; set; }
        public string? marketplaceLink { get; set; }
        public string? specialOfferLink { get; set; }
        public string? logoFileLink { get; set; }
        public string? GeoName { get; set; }
        public string? SolutionAreaName { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public string SolutionStatus { get; set; }
        public DateTime RowChangedDate { get; set; }        
    }
    public class TechnologySolutionReport
    {
        public string OrgName { get; set; }
        public Guid? OrgId { get; set; }
        public string SolutionAreaName { get; set; }
        public Guid? SolutionAreaId { get; set; }
        public string SolutionPlayName { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public string SolutionStatus { get; set; }
        public DateTime RowChangedDate { get; set; }
    }

    public class TechnologySolutionReportCSV
    {
        public string OrgName { get; set; }
        public Guid? OrgId { get; set; }
        public string SolutionAreaName { get; set; }
        public Guid? SolutionAreaId { get; set; }
        public string? SolutionPlayName { get; set; }
        public string? solutionPlayOrgWebsite { get; set; }
        public string? marketplaceLink { get; set; }
        public string? specialOfferLink { get; set; }
        public string? logoFileLink { get; set; }
        public string? GeoName { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public string SolutionStatus { get; set; }
        public DateTime RowChangedDate { get; set; }
    }
    public class PartnerSolutionFullReport
    {
        public string OrgName { get; set; }
        public string? PartnerFirstName { get; set; }
        public string? PartnerLastName { get; set; }
        public string? PartnerEmail { get; set; }
        public string IndustryName { get; set; }
        public string SubIndustryName { get; set; }
        public string SolutionName { get; set; }
        public string SolutionStatus { get; set; }
        public DateTime? RowChangedDate { get; set; }
        public string? MarketPlaceLink { get; set; }
        public string? SolutionDescription { get; set; }
        public string? SolutionOrgWebsite { get; set; }
        public string? SpecialOfferLink { get; set; }
        public string? GeoName { get; set; }
        public string? SolutionAreaName { get; set; }
        public string? SolutionAreaDescription { get; set; }
        public string? ResourceLinkName { get; set; }
        public string? ResourceLinkTitle { get; set; }
        public string? ResourceLinkUrl { get; set; }
        public DateTime? EventDateTime { get; set; }
    }
    public class IndustrySolutionFullReport
    {
        public string OrgName { get; set; }
        public string IndustryName { get; set; }
        public string UseCase { get; set; }
        public string SolutionName { get; set; }
        public string SolutionStatus { get; set; }
        public string? MarketPlaceLink { get; set; }
        public string? SolutionDescription { get; set; }
        public string? SolutionOrgWebsite { get; set; }
        public string? SpecialOfferLink { get; set; }
        public string? Canada { get; set; }
        public string? LatinAmerica { get; set; }
        public string? UnitedStates { get; set; }
        public string? SolutionAreaName { get; set; }
        public string? SolutionAreaDescription { get; set; }
        //public string? ResourceLinkName { get; set; }
        //public string? ResourceLinkTitle { get; set; }
        //public string? ResourceLinkUrl { get; set; }
        public Guid? PartnerSolutionId { get; set; }
       // public DateTime? RowChangedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class IndustrySolutionGeoReport
    {
        public string GeoName { get; set; }
    }
    public class IndustrySolutionAreaReport
    {
        public string SolutionAreaName { get; set; }
        public string SolutionAreaDescription { get; set; }
    }
}
