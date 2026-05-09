using Knack.DBEntities;

namespace Knack.API.Interfaces
{
    public interface ILookupManager
    {
        /// <summary>
        /// Get all solution status
        /// </summary>
        /// <returns></returns>
        Task<List<SolutionStatusType>> GetSolutionStatus();

        /// <summary>
        /// Get solution status based on status id.
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        Task<SolutionStatusType> GetSolutionStatus(Guid statusId);

        /// <summary>
        /// Get all geo informations.
        /// </summary>
        /// <returns></returns>
        Task<List<Geo>> GetGeos();

        /// <summary>
        /// Get geo details with geo id.
        /// </summary>
        /// <param name="geoId"></param>
        /// <returns></returns>
        Task<Geo> GetGeo(Guid geoId);
    }
}
