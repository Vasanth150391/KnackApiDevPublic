using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knack.DBEntities;

[Table("IndustryTargetCustomerProspect")]
public partial class IndustryTargetCustomerProspect
{
    [Key]
    public Guid? IndustryTargetCustomerProspectId { get; set; } = null!;

    public Guid? IndustryThemeId { get; set; } = null!;

    public string TargetPersonaTitle { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
