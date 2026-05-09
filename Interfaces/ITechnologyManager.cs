using Knack.API.Models;

namespace Knack.API.Interfaces
{
    public interface ITechnologyManager
    {

        /// <summary>
        /// Getting the solution areas from DB
        /// </summary>
        /// <returns></returns>
        Task<List<SolutionAreaDTO>> GetSolutionAreas();

        /// <summary>
        /// Getting the solution areas from DB with solution area Id
        /// </summary>
        /// <returns></returns>
        Task<List<SolutionAreaDTO>> GetSolutionArea(string AreaId);

        /// <summary>
        /// Getting the solution area with play from DB
        /// </summary>
        /// <returns></returns>
        Task<List<SolutionAreaPlayDTO>> GetSolutionAreaWithPlays();

        /// <summary>
        /// Getting the solution area and its corresponding plays based on given areaid.
        /// </summary>
        /// <param name="solutionAreaId"></param>
        /// <returns></returns>
        Task<SolutionAreaPlayDTO> GetSolutionAreaWithPlays(string solutionAreaId);

        /// <summary>
        /// Getting the solution play by area id from DB
        /// </summary>
        /// <returns></returns>
        Task<List<SolutionPlayViewDTO>> GetSolutionPlays(string solutionAreaSlug);

        /// <summary>
        /// Get the list of partnersolution profiles with solution play id.
        /// </summary>
        /// <param name="solutionPlayId"></param>
        /// <returns></returns>
        Task<List<PartnerSolutionPlayViewDTO>> GetSolutionProfiles(string solutionPlayId);

        /// <summary>
        /// Get the list of partnersolution profiles with solution spot light.
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
        /// Getting all the published profiles associated for the given organization id.
        /// </summary>
        /// <returns></returns>
        Task<List<OrganizationPartnerSolutionDTO>> GetAssociatedPartnerSolutions(string orgId);

    }
}
