using Knack.API.Models;
using Knack.DBEntities;

namespace Knack.API.Interfaces
{
    public interface ILeadGeneratorManager
    {
        public Task<ResponseDTO> RegisterLeadPartner(LeadPartner leadPartnerModel);
    }
}
