using Knack.API.Models;
using Knack.DBEntities;

namespace Knack.API.Interfaces
{
    public interface IPartnerSolutionBuilder
    {
        /// <summary>
        /// Update partner solution
        /// </summary>
        /// <param name="partnerSolutionDTO"></param>
        /// <returns></returns>
        public Task<Guid> UpdatePartnerSolution(PartnerSolutionDTO partnerSolutionDTO);

        /// <summary>
        /// Create a new partner profile.
        /// </summary>
        /// <param name="partnerSolutionDTO"></param>
        /// <returns></returns>
        public Task<Guid> CreatePartnerSolution(PartnerSolutionDTO partnerSolutionDTO);

        /// <summary>
        /// Update isPublish flag to true.
        /// </summary>
        /// <param name="partnerSolutionPublishDTO"></param>
        /// <returns></returns>
        public Task<Guid> PartnerSolutionPublish(PartnerSolutionPublishDTO partnerSolutionPublishDTO);

        /// <summary>
        /// Get partner solution details with partnersolutionId.
        /// </summary>
        /// <param name="partnerSolutionId"></param>
        /// <returns></returns>
        public Task<PartnerSolutionDTO> GetPartnerSolution(Guid partnerSolutionId);

        /// <summary>
        /// Get Partner solution details basedon industry and subindustry.
        /// </summary>
        /// <param name="industryId"></param>
        /// <param name="subIndustryId"></param>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetPartnerSolution(Guid industryId, Guid subIndustryId);
    }
}
