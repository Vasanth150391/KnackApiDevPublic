using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knack.DBEntities;
[Table("SolutionStatus")]
public partial class SolutionStatusType
{
    [Key]
    public Guid SolutionStatusId { get; set; }
    public string SolutionStatus { get; set; } = null!;
    public string DisplayLabel { get; set; } = null!;
}
