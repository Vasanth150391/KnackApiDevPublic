using Knack.API.Data;
using Knack.API.Interfaces;
using Knack.API.Models;
using Knack.DBEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Globalization;

namespace Knack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnologyController : ControllerBase
    {
        private readonly ITechnologyBuilder _technologyBuilder;
        public TechnologyController(ITechnologyBuilder technologyBuilder)
        {
            _technologyBuilder = technologyBuilder;
        }

        /// <summary>
        /// Get all solution areas.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SolutionAreas")]
        public async Task<List<SolutionAreaDTO>> GetSolutionAreas()
        {
            return await _technologyBuilder.GetSolutionAreas();          
        }

        /// <summary>
        /// Get solution area with id.
        /// </summary>
        /// <param name="AreaId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SolutionArea")]
        public async Task<List<SolutionAreaDTO>> GetSolutionArea(string AreaId)
        {
            return await _technologyBuilder.GetSolutionArea(AreaId);
        }

        /// <summary>
        /// Get all solution areas and its associated plays.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SolutionAreasWithPlays")]
        public async Task<List<SolutionAreaPlayDTO>> GetSolutionAreasWithPlays()
        {
            return await _technologyBuilder.GetSolutionAreaWithPlays();
        }

        /// <summary>
        /// Get solution area and its associated plays with given areaid.
        /// </summary>
        /// <param name="solutionAreaId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SolutionAreaWithPlays")]
        public async Task<SolutionAreaPlayDTO> GetSolutionAreaWithPlays(string solutionAreaId)
        {
            return await _technologyBuilder.GetSolutionAreaWithPlays(solutionAreaId);
        }

        /// <summary>
        /// Get all the solution plays associated with solutionareaslug.
        /// </summary>
        /// <param name="solutionAreaSlug"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SolutionPlays")]
        public async Task<List<SolutionPlayViewDTO>> GetSolutionPlays(string solutionAreaSlug)
        {
            return await _technologyBuilder.GetSolutionPlays(solutionAreaSlug);
        }

        /// <summary>
        /// Get all profiles belong to solution play with solution play id.
        /// </summary>
        /// <param name="solutionPlayId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SolutionProfiles")]
        public async Task<List<PartnerSolutionPlayViewDTO>> GetSolutionProfiles(string solutionPlayId)
        {
           return await _technologyBuilder.GetSolutionProfiles(solutionPlayId);
        }

        /// <summary>
        /// Get all profiles belong to solution play with spot light.
        /// </summary>
        /// <param name="solutionPlayId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SolutionSpotlight")]
        public async Task<List<TechnologyShowcasePartnerSolutionDTO>> GetSolutionSpotlight(string solutionPlayId)
        {
            return await _technologyBuilder.GetSolutionSpotlight(solutionPlayId);
        }

        [HttpGet]
        [Route("TechnologySpotlightSolutionByViewId")]
        public Task<TechnologyShowcasePartnerSolutionViewDTO> GetTechnologySpotlightSolutionByViewId(string solutionAreaSlug, string solutionPlaySlug, string showcaseSolutionId)
        {
            return _technologyBuilder.GetTechnologySpotlightSolutionByViewId(showcaseSolutionId);
        }

        /// <summary>
        /// Get all solution area with published solution play and profiles.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("PublisedSolutionAreas")]
        public async Task<List<SolutionAreaDTO>> GetAllPublishedSolutionAreas()
        {
            return await _technologyBuilder.GetAllPublishedSolutionAreas();
        }

        /// <summary>
        /// Get all the published Partner Solutions for the given organizationid.
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AssociatedPartnerSolutions")]
        public async Task<IEnumerable<OrganizationPartnerSolutionDTO>> GetAssociatedPartnerSolutions(string orgId)
        {            
            return await _technologyBuilder.GetAssociatedPartnerSolutions(orgId);
        }
    }
}
