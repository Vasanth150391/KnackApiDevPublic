using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Knack.DBEntities;

public partial class Organization
{
    [Key]
    public Guid OrgId { get; set; }

    public string? OrgName { get; set; } = null!;

    public string? OrgDescription { get; set; }

    public string? Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }
    public string? logoFileLink { get; set; }
    public string? orgWebsite { get; set; }
    public string? UserType { get; set; }
    public DateTime? RowChangedDate { get; set; }

}
public partial class MergeOrganization
{
    [Key]
    public Guid? FromOrgId { get; set; } = null!;
    public Guid? ToOrgId { get; set; } = null!;
}
