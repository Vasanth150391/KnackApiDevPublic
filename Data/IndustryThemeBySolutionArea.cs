using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knack.DBEntities;

[Table("IndustryThemeBySolutionArea")]
public partial class IndustryThemeBySolutionArea
{
    [Key]
    public Guid? IndustryThemeBySolutionAreaId { get; set; } = null!;

    public Guid IndustryThemeId { get; set; }

    public Guid? SolutionAreaId { get; set; } = null!;

    public string? SolutionDesc { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
