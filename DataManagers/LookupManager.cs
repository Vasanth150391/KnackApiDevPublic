using Knack.API.Data;
using Knack.API.Interfaces;
using Knack.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace Knack.API.DataManagers
{
    public class LookupManager : ILookupManager
    {
        private readonly KnackContext _knackContext;
        private readonly ILogger<LookupManager> _logger;

        public LookupManager(KnackContext knackContext, ILogger<LookupManager> logger)
        {
            _knackContext = knackContext;
            _logger = logger;
        }

        /// <summary>
        /// Get geo based on geo id.
        /// </summary>
        /// <param name="geoId"></param>
        /// <returns></returns>
        public async Task<Geo> GetGeo(Guid geoId)
        {
            try
            {
                _logger.LogDebug("Entering the method GetGeo");
                var geo= await _knackContext.Geos.Where(x=>x.GeoId == geoId).FirstOrDefaultAsync();

                return geo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in the method GetGeo {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<List<Geo>> GetGeos()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Getting all the solution status.
        /// </summary>
        /// <returns></returns>
        public async Task<List<SolutionStatusType>> GetSolutionStatus()
        {
            try
            {
                _logger.LogDebug($"Entering in to class {nameof(LookupManager)} and method GetSolutionStatus");

                var solutionStatus = await _knackContext.SolutionStatusType.ToListAsync();
                return solutionStatus;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred in class {nameof(LookupManager)} and method GetSolutionStatus {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get solution status detail with solutionstatus id.
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        public async Task<SolutionStatusType> GetSolutionStatus(Guid statusId)
        {
            try
            {
                _logger.LogDebug($"Entering in to class {nameof(LookupManager)} and method GetSolutionStatus");

                var solutionStatus = await _knackContext.SolutionStatusType.Where(x => x.SolutionStatusId == statusId).FirstOrDefaultAsync();
                return solutionStatus ?? throw new Exception($"No record found for the given solutin statusid {statusId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred in class {nameof(LookupManager)} and method GetSolutionStatus {ex.Message}");
                throw;
            }
        }
    }
}
