namespace Knack.API.Models
{
    public class IndustryThemeBySolutionAreaDTO
    {
        public Guid? IndustryThemeBySolutionAreaId { get; set; } = null!;

        public Guid? IndustryThemeId { get; set; } = null!;

        public Guid? SolutionAreaId { get; set; } = null!;

        public string SolutionDesc { get; set; } = null!;
    }
}
