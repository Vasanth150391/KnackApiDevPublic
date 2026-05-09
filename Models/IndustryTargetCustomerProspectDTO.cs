namespace Knack.API.Models
{
    public class IndustryTargetCustomerProspectDTO
    {
        public Guid? IndustryTargetCustomerProspectId { get; set; } = null!;

        public Guid? IndustryThemeId { get; set; } = null!;

        public string TargetPersonaTitle { get; set; } = null!;
    }
}
