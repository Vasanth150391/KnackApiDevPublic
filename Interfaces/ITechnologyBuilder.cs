using Knack.API.DataManagers;
using Knack.API.Models;

namespace Knack.API.Interfaces
{
    public interface ITechnologyBuilder
    {
        /// <summary>
        /// Getting the solution areas.
        /// </summary>
        public Task<List<SolutionAreaDTO>> GetSolutionAreas();

        /// <summary>
        /// Getting the solution area by area id
        /// </summary>
        public Task<List<SolutionAreaDTO>> GetSolutionArea(string AreaId);

        /// <summary>
        /// Getting the solution area with solution plays.
        /// </summary>
        public Task<List<SolutionAreaPlayDTO>> GetSolutionAreaWithPlays();

        /// <summary>
        /// Getting the solution area and its corresponding plays based on given areaid.
        /// </summary>
        /// <param name="solutionAreaId"></param>
        /// <returns></returns>
        Task<SolutionAreaPlayDTO> GetSolutionAreaWithPlays(string solutionAreaId);

        /// <summary>
        /// Getting the solution play with solution area id.
        /// </summary>
        Task<List<SolutionPlayViewDTO>> GetSolutionPlays(string solutionAreaSlug);

        /// <summary>
        /// Get the list of partnersolution profiles with solution play id.
        /// </summary>
        /// <param name="solutionPlayId"></param>
        /// <returns></returns>
        Task<List<PartnerSolutionPlayViewDTO>> GetSolutionProfiles(string solutionPlayId);

        /// <summary>
        /// Get the list of partnersolution profiles with solution spotlight.
        /// </summary>
        /// <param name="solutionPlayId"></param>
        /// <returns></returns>
        Task<List<TechnologyShowcasePartnerSolutionDTO>> GetSolutionSpotlight(string solutionPlayId);

        /// <summary>
        /// Get the list of partnersolution profiles with solution spotlight.
        /// </summary>
        /// <param name="showcaseSolutionId"></param>
        /// <returns></returns>
        Task<TechnologyShowcasePartnerSolutionViewDTO> GetTechnologySpotlightSolutionByViewId(string showcaseSolutionId);
        
        /// <summary>
        /// Getting all the solution areas whose solution play and solution profiles are active and published.
        /// </summary>
        /// <returns></returns>
        Task<List<SolutionAreaDTO>> GetAllPublishedSolutionAreas();

        /// <summary>
        /// Get all the published Partner Solutions for the given organizationid.
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        Task<List<OrganizationPartnerSolutionDTO>> GetAssociatedPartnerSolutions(string orgId);

    }
}
