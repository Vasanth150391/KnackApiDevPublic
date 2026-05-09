using Knack.API.DataManagers;
using Knack.API.Interfaces;
using Knack.API.Mapping;
using Knack.API.Models;

namespace Knack.API.Builders
{
    public class LeadGeneratorBuilder : ILeadGeneratorBuilder
    {
        private readonly ILeadGeneratorManager _leadGeneratorManager;
        private readonly ILogger<LeadGeneratorBuilder> _logger;
        public LeadGeneratorBuilder(ILogger<LeadGeneratorBuilder> logger,ILeadGeneratorManager leadGeneratorManager)
        {
            _leadGeneratorManager = leadGeneratorManager;
            _logger = logger;
        }
        public async Task<ResponseDTO> RegisterLeadPartner(LeadPartnerDTO leadPartnerDto)
        {
            try
            {
                _logger.LogDebug("Entering the method RegisterLeadPartner of class LeadGeneratorBuilder");
                var result = await _leadGeneratorManager.RegisterLeadPartner(leadPartnerDto.ToLeadPartnerModel());

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method RegisterLeadPartner of class LeadGeneratorBuilder {ex.InnerException}");
                throw;
            }
        }
    }
}
