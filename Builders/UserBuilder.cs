using Knack.API.Interfaces;
using System.Runtime.InteropServices;

namespace Knack.API.Builders
{
    public class UserBuilder : IUserBuilder
    {
        private readonly IUserManager _userManager;

        public UserBuilder(IUserManager userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> DeletePartnerUser(Guid PartnerUserId)
        {
            try
            {
                var result = await _userManager.DeletePartnerUser(PartnerUserId);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
