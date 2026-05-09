using System;
using System.Collections.Generic;

namespace Knack.DBEntities;

public partial class UserInvite
{
    public Guid? UserInviteId { get; set; }
    public string? UserInviteEmail { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Status { get; set; }
    public string? RowChangedBy { get; set; }
    public DateTime? RowChangedDate { get; set; }
}
