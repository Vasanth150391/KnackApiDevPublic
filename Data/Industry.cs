using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knack.DBEntities;

[Table("Industry")]
public partial class Industry
{
    [Key]
    public Guid IndustryId { get; set; }

    public string IndustryName { get; set; } = null!;

    public string? IndustryDescription { get; set; }

    public string Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
    public string? IndustrySlug { get; set; }
    public string? Image_main { get; set; }
    public string? Image_mobile { get; set; }
    public virtual Collection<SubIndustry>? SubIndustries { get; set; }
}
