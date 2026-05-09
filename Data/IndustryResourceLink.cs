using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knack.DBEntities;

[Table("IndustryResourceLink")]
public partial class IndustryResourceLink
{
    [Key]
    public Guid? IndustryResourceLinkId { get; set; } = null!;

    public Guid? IndustryThemeId { get; set; } = null!;

    public Guid? ResourceLinkId { get; set; } = null!;

    public string? Title { get; set; } = null!;

    public string? ResourceLink { get; set; } = null!;

    public string? Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}