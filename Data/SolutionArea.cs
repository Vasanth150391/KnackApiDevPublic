using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knack.DBEntities;

[Table("SolutionArea")]
public partial class SolutionArea
{
    [Key]
    public Guid SolutionAreaId { get; set; }

    public string? SolutionAreaName { get; set; } = null!;
    public string? SolutionAreaSlug { get; set; } = null!;
    public string? SolAreaDescription { get; set; }

    public int? DisplayOrder { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDisplayOnPartnerProfile { get; set; } = false;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
    public string? Image_Thumb { get; set; } = null!;
    public string? Image_Main { get; set; } = null!;
    public string? Image_Mobile { get; set; } = null!;
}
