using Knack.API.Models;
using Knack.DBEntities;

namespace Knack.API.Interfaces
{
    public interface IPartnerSolutionManager
    {
        /// <summary>
        /// Update Partner solution details.
        /// </summary>
        /// <param name="partnerSolution"></param>
        /// <returns></returns>
        public Task<Guid> UpdatePartnerSolution(PartnerSolution partnerSolution);
        /// <summary>
        /// Create a new partner profile.
        /// </summary>
        /// <param name="partnerSolution"></param>
        /// <returns></returns>
        public Task<Guid> CreatePartnerSolution(PartnerSolution partnerSolution);

        /// <summary>
        /// Get partner solution details with partnersolutionId.
        /// </summary>
        /// <param name="partnerSolutionId"></param>
        /// <returns></returns>
        public Task<PartnerSolution> GetPartnerSolution(Guid partnerSolutionId);

        /// <summary>
        /// Create partner available geos.
        /// </summary>
        /// <param name="partnerSolutionAvailableGeos"></param>
        /// <returns></returns>
        public  Task CreatePartnerAvailableGeo(List<PartnerSolutionAvailableGeo> partnerSolutionAvailableGeos);

        /// <summary>
        /// create partner available solution areas.
        /// </summary>
        /// <param name="partnerSolutionAreaModels"></param>
        /// <returns></returns>
        public  Task CreatePartnerSolutionArea(List<PartnerSolutionAreaModel> partnerSolutionAreaModels);

        /// <summary>
        /// create partner spotlight.
        /// </summary>
        /// <param name="spotLight"></param>
        /// <returns></returns>
        public  Task CreatePartnerSpotLight(SpotLight spotLight);

        /// <summary>
        /// update partner solution areas.
        /// </summary>
        /// <param name="partnerSolutionByAreas"></param>
        /// <returns></returns>
        public  Task UpdatePartnerSolutionArea(List<PartnerSolutionAreaModel> partnerSolutionAreaModels);

        /// <summary>
        /// Update partner solutin geo.
        /// </summary>
        /// <param name="partnerSolutionAvailableGeo"></param>
        /// <returns></returns>
        public  Task UpdatePartnerSolutionAvailableGeo(List<PartnerSolutionAvailableGeo> partnerSolutionAvailableGeo);

        /// <summary>
        /// update partner spotlight.
        /// </summary>
        /// <param name="spotLight"></param>
        /// <returns></returns>
        public  Task UpdatePartnerSolutionSpotlight(SpotLight spotLight);

        /// <summary>
        /// update partner resource links.
        /// </summary>
        /// <param name="resourceLinks"></param>
        /// <returns></returns>
        public  Task CreatePartnerSolutionResourceLink(List<PartnerSolutionResourceLink> resourceLinks);

        /// <summary>
        /// Update ispublish to true.
        /// </summary>
        /// <param name="partnerSolution"></param>
        /// <returns></returns>
        public Task<Guid> PartnerSolutionPublish(PartnerSolutionPublishDTO partnerSolutionPublishDTO);

        /// <summary>
        /// Update the parent partner solution while publishing the child partner solution.
        /// </summary>
        /// <param name="parentPartnerSolution"></param>
        /// <returns></returns>
        public Task<Guid> UpdateParentPartnerSolution(PartnerSolution parentPartnerSolution);

        /// <summary>
        /// Update Parent partner available geos.
        /// </summary>
        /// <param name="partnerSolutionAvailableGeo"></param>
        /// <param name="parentPartnerSolutionId"></param>
        /// <returns></returns>
        public Task UpdateParentPartnerSolutionAvailableGeos(List<PartnerSolutionAvailableGeo>
            partnerSolutionAvailableGeo, Guid parentPartnerSolutionId);

        /// <summary>
        /// Update Parent partner spotlight.
        /// </summary>
        /// <param name="spotLight"></param>
        /// <param name="parentPartnerSolutionId"></param>
        /// <returns></returns>
        public Task UpdateParentPartnerSolutionSpotlight(SpotLight spotLight, Guid parentPartnerSolutionId);

        /// <summary>
        /// Update parent partner resource links.
        /// </summary>
        /// <param name="partnerSolutionAvailableGeo"></param>
        /// <param name="parentPartnerSolutionId"></param>
        /// <returns></returns>
        public Task UpdateParentPartnerSolutionResourceLinks(List<PartnerSolutionResourceLink> resourceLinks,
            Guid parentPartnerSolutionId);

        /// <summary>
        /// update parent partner solution areas.
        /// </summary>
        /// <param name="partnerSolutionAreaModels"></param>
        /// <returns></returns>
        public Task UpdateParentPartnerSolutionAreas(List<PartnerSolutionByArea> partnerSolutionAreas,
            Guid parentPartnerSolutionId);

        /// <summary>
        /// Get all the available geos for the given partenrsolutionid.I
        /// </summary>
        /// <param name="partnerSolutionId"></param>
        /// <returns></returns>
        public Task<List<PartnerSolutionAvailableGeo>> GetPartnerSolutionAvailableGeos(Guid partnerSolutionId);

        /// <summary>
        /// Get all the available partner solution areas for the given partnersolutionId.
        /// </summary>
        /// <param name="partnerSolutionId"></param>
        /// <returns></returns>
        public Task<List<PartnerSolutionByArea>> GetPartnerSolutionAreas(Guid partnerSolutionId);

        /// <summary>
        /// Get the spot light details for the given partnersolutionId.
        /// </summary>
        /// <param name="partnerSolutionId"></param>
        /// <returns></returns>
        public Task<SpotLight> GetPartnerSolutionSpotLight(Guid partnerSolutionId);

        /// <summary>
        /// Get the all the resource link for the given partnersolutionId.
        /// </summary>
        /// <param name="partnerSolutionAreaId"></param>
        /// <returns></returns>
        public Task<List<PartnerSolutionResourceLink>> GetPartnerSolutionResourceLinks(Guid partnerSolutionAreaId);

        /// <summary>
        /// Delete the partner solution based on partner solutionId.
        /// </summary>
        /// <param name="partnerSolutionId"></param>
        /// <returns></returns>
        public Task RemovePartnerSolution(Guid partnerSolutionId);

        /// <summary>
        /// Update partner solution resource links.
        /// </summary>
        /// <param name="partnerSolutionResourceLink"></param>
        /// <returns></returns>
        public Task UpdatePartnerSolutionResourceLink(List<PartnerSolutionResourceLink> partnerSolutionResourceLink);

        /// <summary>
        /// Get partnersolution detail with industryid and subindustryid
        /// </summary>
        /// <param name="industryId"></param>
        /// <param name="subIndustryId"></param>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetPartnerSolution(Guid industryId, Guid subIndustryId);


    }
}
