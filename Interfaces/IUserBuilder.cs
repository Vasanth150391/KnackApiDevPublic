using Knack.DBEntities;

namespace Knack.API.Interfaces
{
    public interface IUserBuilder
    {
        public Task<string> DeletePartnerUser(Guid PartnerUserId);
    }
}
