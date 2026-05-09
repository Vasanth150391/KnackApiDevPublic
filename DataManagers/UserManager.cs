using Knack.API.Data;
using Knack.API.Interfaces;
using Knack.DBEntities;

namespace Knack.API.DataManagers
{
    public class UserManager : IUserManager
    {
        private readonly KnackContext _knackContext;
        public UserManager(KnackContext context)
        {
            _knackContext = context;
        }

        /// <summary>
        /// Get the partner user details with id.
        /// </summary>
        /// <param name="partnerUserId"></param>
        /// <returns></returns>
        public async Task<PartnerUser> GetPartnerUser(Guid partnerUserId)
        {
            try
            {
                var org = _knackContext.PartnerUser.Where(t => t.PartnerUserId == partnerUserId).FirstOrDefault();

               if (org == null) { throw new Exception("No user found for the given user"); }

                return org;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Delete the partner user with given partneruser id.
        /// </summary>
        /// <param name="partnerUserId"></param>
        /// <returns></returns>
        public async Task<string> DeletePartnerUser(Guid partnerUserId)
        {
            try
            {
                var userDetails = _knackContext.PartnerUser.Where(x => x.PartnerUserId.Equals(partnerUserId)).FirstOrDefault();

                if (userDetails != null)
                {
                    userDetails.FirstName = "Deleted User";
                    userDetails.LastName = "Deleted User";
                    userDetails.PartnerEmail = "Deleted User";
                    userDetails.Status = "Deleted";
                    userDetails.RowChangedDate= DateTime.Now;
                }

               _knackContext?.Update(userDetails);
                await _knackContext?.SaveChangesAsync();
                return "User Deleted Successfully";

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
