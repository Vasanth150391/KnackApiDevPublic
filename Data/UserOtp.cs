using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knack.DBEntities;
[Table("UserOtp")]
public partial class UserOtp
{
    [Key]
    public Guid? UserOtpId { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public string OtpNumber { get; set; } = null!;

    public DateTime? RowChangedDate { get; set; }
}
