using Knack.API.Interfaces;
using Knack.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Knack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBuilder _userBuilder;

        public UserController(IUserBuilder userBuilder)
        {
            _userBuilder = userBuilder;
        }

        [HttpPost]
        [Route("PartnerUser")]
        public async Task<IActionResult> DeletePartnerUser([FromBody] PartnerUserDTO partnerUserDTO)
        {
            var response = await _userBuilder.DeletePartnerUser(partnerUserDTO.PartnerUserId);

            if (response == null)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity,
                "Error occured while deleting the user. Please contact the admin for more details");
            }

            return Ok(new ResponseDTO { Message=response,Response=true});
        }

    }
}
