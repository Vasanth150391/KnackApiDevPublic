namespace Knack.API.Models
{
    public class IndustryResourceLinkDTO
    {
        public Guid? IndustryResourceLinkId { get; set; } = null!;

        public Guid? IndustryThemeId { get; set; } = null!;

        public Guid? ResourceLinkId { get; set; } = null!;

        public string? Title { get; set; } = null!;

        public string? ResourceLink { get; set; } = null!;
    }
}
