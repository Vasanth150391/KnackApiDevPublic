namespace Knack.API.Models
{
    public class IndustryThemeDetailsDTO
    {
        public Guid? IndustryId { get; set; }

        public Guid? SubIndustryId { get; set; }
        public Guid? IndustryThemeId { get; set; }
        public Guid? PartnerId { get; set; }
        public string? IndustryName { get; set; } = null!;
        public string? SubIndustryName { get; set; } = null!;
        public string? Theme { get; set; } = null!;
        public string? IndustryThemeDesc { get; set; } = null!;
        public string? Image_Thumb { get; set; } = null!;   
        public string? Image_Main { get; set; } = null!;   
        public string? Image_Mobile { get; set; } = null!;
        public string? SubIndustryDescription { get; set; } = null!;
        public List<ThemeSolutionAreaDTO> ThemeSolutionAreas { get; set; }
        public List<IndustryShowcasePartnerSolutionDTO> SpotLightPartnerSolutions { get; set; }
        public List<GetPartnerSolutionDTO> SpotLights { get; set; }
        public Boolean? hasMultipleTheme { get; set; }
        public string? IndustrySlug { get; set; }
        public string? SubIndustrySlug { get; set; }
        public string? IndustryThemeSlug { get; set; }
    }    
    public class ThemeSolutionAreaDTO
    {
        public Guid? IndustryThemeBySolutionAreaId { get; set; }
        public Guid? IndustryThemeId { get; set; }
        public Guid? SolutionAreaId { get; set; }
        public string? SolutionDesc { get; set; }
        public string? SolutionAreaName { get; set; }
        public List<GetPartnerSolutionDTO> PartnerSolutions { get; set; }
    }
}
