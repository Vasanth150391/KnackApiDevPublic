using Knack.API.Models;
using Knack.DBEntities;

namespace Knack.API.Interfaces
{
    public interface IUserManager
    {
        /// <summary>
        /// Delete the partner user with id.
        /// </summary>
        /// <param name="partnerUserId"></param>
        /// <returns></returns>
        Task<string> DeletePartnerUser(Guid partnerUserId);

        /// <summary>
        /// Getting the partner user details with partneruserid.
        /// </summary>
        /// <param name="partnerUserId"></param>
        /// <returns></returns>
        Task<PartnerUser> GetPartnerUser(Guid partnerUserId);
    }
}
