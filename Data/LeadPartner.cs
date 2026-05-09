using System;
using System.Collections.Generic;

namespace Knack.DBEntities;

public partial class LeadPartner
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? EmailAddress { get; set; }

    public string? Status { get; set; }

    public DateTime? Createdon { get; set; }

    public string? Createby { get; set; }

    public string? CompanyName { get; set; }

    public string? Modifiedby { get; set; }

    public int LeadPartnerId { get; set; }
    public string? FtpuserName { get; set; }
    public string? FTPFolderName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? SFTPGoResponse { get; set; }
    public string? SFTPGoRequest { get; set; }
}
