using System;
using System.Collections.Generic;

namespace Knack.DBEntities;

public partial class PartnerUser
{
    public Guid? PartnerUserId { get; set; } = null!;

    public Guid? OrgId { get; set; } = null!;

    public string? LastName { get; set; } = null!;
    public string? FirstName { get; set; } = null!;

    public string? PartnerEmail { get; set; }

    public string? PartnerTitle { get; set; }

    public string? Status { get; set; } = null!;

    public string? UserType { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
