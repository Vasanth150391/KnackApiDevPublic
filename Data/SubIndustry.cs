using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Knack.DBEntities;

[Table("SubIndustry")]
public partial class SubIndustry
{
    [Key, ForeignKey("Industry")]
    public Guid SubIndustryId { get; set; }


    public Guid IndustryId { get; set; }

    public string? SubIndustryName { get; set; } = null!;

    public string? SubIndustryDescription { get; set; }

    public string? Status { get; set; } = null!;

    public string? SubIndustrySlug { get; set; } = null!;

    public Guid? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
public partial class SubIndustryWithIndustryTheme
{
    public Guid SubIndustryId { get; set; }
    public Guid IndustryId { get; set; }
    public string? SubIndustryName { get; set; } = null!;
    public string? SubIndustryDescription { get; set; }
    public string? Status { get; set; } = null!;
    public string? SubIndustrySlug { get; set; } = null!;
    public Guid? RowChangedBy { get; set; }
    public DateTime? RowChangedDate { get; set; }
    public string? Image_main { get; set; }
    public string? Image_thumb { get; set; }
    public string? Image_mobile { get; set; }
}
