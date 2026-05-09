using Azure;
using Knack.API.Interfaces;
using Knack.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Knack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadGeneratorController : ControllerBase
    {
        private readonly ILeadGeneratorBuilder _leadGeneratorBuilder;
        private readonly ILogger<LeadGeneratorController> _logger;

        public LeadGeneratorController(ILeadGeneratorBuilder leadGeneratorBuilder,ILogger<LeadGeneratorController> logger)
        {
            _leadGeneratorBuilder = leadGeneratorBuilder;
            _logger = logger;
        }

        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(typeof(int),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Post(LeadPartnerDTO leadPartnerDto)
        {
            try
            {
                _logger.LogDebug("Entering the method RegisterLeadPartner of LeadGeneratorController");
                if(leadPartnerDto == null)
                {
                    throw new ArgumentNullException(nameof(leadPartnerDto));
                }
                var response = new ResponseDTO();
                response = await _leadGeneratorBuilder.RegisterLeadPartner(leadPartnerDto);

                return Ok(new ResponseDTO { Message = Convert.ToString(response.Message), Response = response.Response });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method RegisterLeadPartner of LeadGeneratorController {ex.InnerException}");

                throw;
            }
        }
    }
}
