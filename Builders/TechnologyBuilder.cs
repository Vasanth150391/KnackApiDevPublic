using Knack.API.DataManagers;
using Knack.API.Interfaces;
using Knack.API.Models;

namespace Knack.API.Builders
{
    public class TechnologyBuilder : ITechnologyBuilder
    {
        private readonly ITechnologyManager _technologyManager;
        public TechnologyBuilder(ITechnologyManager technologyManger)
        {
            _technologyManager = technologyManger;
        }

        public Task<List<SolutionAreaDTO>> GetAllPublishedSolutionAreas()
        {
            return _technologyManager.GetAllPublishedSolutionAreas();
        }

        public Task<List<OrganizationPartnerSolutionDTO>> GetAssociatedPartnerSolutions(string orgId)
        {
            return _technologyManager.GetAssociatedPartnerSolutions(orgId);
        }

        public async Task<List<SolutionAreaDTO>> GetSolutionArea(string AreaId)
        {
            return await _technologyManager.GetSolutionArea(AreaId);
        }

        public async Task<List<SolutionAreaDTO>> GetSolutionAreas()
        {
            return await _technologyManager.GetSolutionAreas();
        }

        public async Task<List<SolutionAreaPlayDTO>> GetSolutionAreaWithPlays()
        {
           return await _technologyManager.GetSolutionAreaWithPlays();          
        }

        public async Task<SolutionAreaPlayDTO> GetSolutionAreaWithPlays(string solutionAreaId)
        {
            return await _technologyManager.GetSolutionAreaWithPlays(solutionAreaId);
        }

        public async Task<List<SolutionPlayViewDTO>> GetSolutionPlays(string solutionAreaSlug)
        {
            return await _technologyManager.GetSolutionPlays(solutionAreaSlug);
        }
        public async Task<List<TechnologyShowcasePartnerSolutionDTO>> GetSolutionSpotlight(string solutionPlayId)
        {
            return await _technologyManager.GetSolutionSpotlight(solutionPlayId);
        }
        public Task<TechnologyShowcasePartnerSolutionViewDTO> GetTechnologySpotlightSolutionByViewId(string showcaseSolutionId)
        {
            return _technologyManager.GetTechnologySpotlightSolutionByViewId(showcaseSolutionId);
        }
        public async Task<List<PartnerSolutionPlayViewDTO>> GetSolutionProfiles(string solutionPlayId)
        {
            return await _technologyManager.GetSolutionProfiles(solutionPlayId);
        }
    }
}
