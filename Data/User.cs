using System;
using System.Collections.Generic;

namespace Knack.DBEntities;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string PartnerUserId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string UserRole { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateOnly? RowChangedDate { get; set; }
}
