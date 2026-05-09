using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Knack.API.Models;

namespace Knack.DBEntities;

[Table("SolutionPlay")]
public partial class SolutionPlay
{
    [Key]
    public Guid? SolutionPlayId { get; set; }
    public Guid? SolutionAreaId { get; set; }
    public string? SolutionPlayThemeSlug { get; set; }
    public string? SolutionPlayName { get; set; }
    public string? SolutionPlayDesc { get; set; }
    public string? SolutionPlayLabel { get; set; }   
    public Guid? SolutionStatusId { get; set; }
    public Guid? RowChangedBy { get; set; }
    public DateTime? RowChangedDate { get; set; }
    public string? Image_Thumb { get; set; } = null!;
    public string? Image_Main { get; set; } = null!;
    public string? Image_Mobile { get; set; } = null!;
    public int? IsPublished { get; set; }

    //public List<TechnologyShowcasePartnerSolutionDTO> PartnersSolutions { get; set; }

}
