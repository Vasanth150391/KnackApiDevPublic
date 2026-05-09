namespace Knack.API.Models
{
    public class IndustryRegistrationDTO
    {
        //public PartnerUserDTO User { get; set; }
        public SubIndustryRegistrationDTO SubIndustry { get; set; }
        public IndustryThemeDTO? Theme { get; set; }
        public List<IndustryTargetCustomerProspectDTO> CustomerProspects { get; set; }
        public List<IndustryShowcasePartnerSolutionDTO> PartnersSolutions { get; set; }
        public List<IndustryThemeBySolutionAreaDTO> ThemeBySolutionAreaModels { get; set; }
        public List<IndustryResourceLinkDTO> ResourceLinks { get; set; }
        public List<string>? SelectedOrg { get; set; }
    }

    public class IndustryRegistrationPublishedDTO
    {
        public Guid IndustryThemeId { get; set; }
        public string? IsPublished { get; set; } = null!;
    }

    public class SubIndustryRegistrationDTO
    {
        public Guid SubIndustryId { get; set; }
        public string SubIndustryDescription { get; set; }
        public string SubIndustryName { get; set; }
    }
    public class IndustryThemeIdDTO
    {
        public Guid IndustryThemeId { get; set; }
    }
}
