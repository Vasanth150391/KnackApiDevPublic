using Knack.API.Models;
using Knack.DBEntities;

namespace Knack.API.Interfaces
{
    public interface ILookupBuilder
    {
        /// <summary>
        /// Get all solution status
        /// </summary>
        /// <returns></returns>
        Task<List<SolutionStatusDTO>> GetSolutionStatus();

        /// <summary>
        /// Get solution status based on status id.
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        Task<SolutionStatusDTO> GetSolutionStatus(Guid statusId);
    }
}
