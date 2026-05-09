using System.ComponentModel.DataAnnotations;

namespace Knack.DBEntities;

public partial class SolutionAvailableGeo
{
    [Key]
    public string SolutionAvailableGeoId { get; set; } = null!;

    public string SolutionId { get; set; } = null!;

    public Guid? GeoId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateOnly? RowChangedDate { get; set; }
}
