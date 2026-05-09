using Knack.API.Models;

namespace Knack.API.Interfaces
{
    public interface ILeadGeneratorBuilder
    {
        public Task<ResponseDTO> RegisterLeadPartner(LeadPartnerDTO leadPartnerModel);
    }
}
