using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knack.DBEntities;
[Table("usecaseOrganization")]
public partial class UsecaseOrganization
{
    [Key]
    public Guid? usecaseId { get; set; }
    public Guid? organizationId { get; set; }
}
