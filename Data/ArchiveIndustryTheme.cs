using Knack.DBEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knack.Entities.DBEntities
{
    [Table("ArchiveIndustryTheme")]
    public partial class ArchiveIndustryTheme
    {
        [Key]
        public Guid IndustryThemeId { get; set; }

        public Guid IndustryId { get; set; }

        public Guid? PartnerId { get; set; }

        public Guid? SubIndustryId { get; set; }

        public string? Theme { get; set; } = null!;

        public string? Image_Thumb { get; set; } = null!;

        public string? Image_Main { get; set; } = null!;

        public string? Image_Mobile { get; set; } = null!;
        
        public string? IndustryThemeDesc { get; set; } = null!;

        public Guid? RowChangedBy { get; set; }

        //public string? Status { get; set; }
        public Guid? SolutionStatusId { get; set; }

        public string? IsPublished { get; set; }
        public DateTime? RowChangedDate { get; set; }

        public string? IndustryThemeSlug { get; set;}


    }
}
