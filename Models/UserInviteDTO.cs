using Knack.DBEntities;

namespace Knack.API.Models
{
    public partial class UserInviteDTO
    {
        public Guid? UserInviteId { get; set; }
        public string? UserInviteEmail { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Status { get; set; }
        public string? RowChangedBy { get; set; }
        public string? LoginUrl { get; set; }
        public DateTime? RowChangedDate { get; set; }
    }
}
