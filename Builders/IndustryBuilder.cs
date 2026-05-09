using System.Configuration;
using Knack.API.Common;
using Knack.API.Data;
using Knack.API.Interfaces;
using Knack.API.Mapping;
using Knack.API.Models;
using Knack.API.UnitOfWork;
using Knack.DBEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

namespace Knack.API.Builders
{
    public class IndustryBuilder : IPartnerSolutionBuilder
    {
        private readonly IPartnerSolutionManager _partnerSolutionManager;
        private readonly ILogger<IndustryBuilder> _logger;
        private readonly Utilities _utilities;
        private readonly IUserManager _userManager;
        private readonly string _basefilePath = string.Empty;
        private readonly IConfiguration _configuration;
        private readonly ILookupManager _lookupManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration configuration;

        public IndustryBuilder(IPartnerSolutionManager partnerSolutionManager,
            ILogger<IndustryBuilder> logger,
            Utilities utilities, IUserManager userManager,
            IConfiguration iConfig, ILookupManager lookupManager, IUnitOfWork unitOfWork)
        {
            _partnerSolutionManager = partnerSolutionManager;
            _logger = logger;
            _utilities = utilities;
            _userManager = userManager;
            _basefilePath = Directory.GetCurrentDirectory() + "\\Mail_Templates";
            _configuration = iConfig;
            _lookupManager = lookupManager;
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Creating the partner solution details.
        /// </summary>
        /// <param name="partnerSolutionDTO"></param>
        /// <returns></returns>
        public async Task<Guid> CreatePartnerSolution(PartnerSolutionDTO partnerSolutionDTO)
        {
            try
            {
                _logger.LogDebug("Entering the method CreatePartnerSolution");
                var partnerSolutionId = await _partnerSolutionManager.CreatePartnerSolution(partnerSolutionDTO.ToPartnerSolutionModel());

                //Creating the dependent objects.
                partnerSolutionDTO.PartnerSolutionId = partnerSolutionId;
                var partnerSolutionAvailableGeosDto = new List<PartnerSolutionAvailableGeoDTO>();

                if (partnerSolutionDTO?.SelectedGeos?.Count > 0)
                {
                    partnerSolutionDTO.SelectedGeos.ForEach(x =>
                    {
                        var partnerSolutionAvailableGeo = new PartnerSolutionAvailableGeoDTO()
                        {
                            GeoId = new Guid(x),
                            Geoname = string.Empty,
                            ParentSolutionId = partnerSolutionId
                        };
                        partnerSolutionAvailableGeosDto.Add(partnerSolutionAvailableGeo);
                    });
                    await _partnerSolutionManager.CreatePartnerAvailableGeo(partnerSolutionAvailableGeosDto?.ToPartnerSolutionAvailableGeoModel());
                }

                if (partnerSolutionDTO?.PartnerSolutionAreas?.Count > 0)
                {
                    partnerSolutionDTO?.PartnerSolutionAreas?.ForEach(x => x.ParentSolutionId = partnerSolutionId);
                    await _partnerSolutionManager.CreatePartnerSolutionArea(partnerSolutionDTO?.PartnerSolutionAreas?.ToPartnerSolutionAreaModel());
                }

                if (partnerSolutionDTO?.SpotLight != null)
                {
                    partnerSolutionDTO.SpotLight.PartnerSolutionId = partnerSolutionId;
                    await _partnerSolutionManager.CreatePartnerSpotLight(partnerSolutionDTO.SpotLight.ToSpotLoghtModel());
                }

                await _unitOfWork.CommitAsync();
                //await _knackContext.SaveChangesAsync();
                var filePath = _basefilePath + "\\Industry_Solution_Submitted.html";
                var emailBody = File.ReadAllText(filePath);

                var partnerUserId = (Guid)partnerSolutionDTO?.UserId;
                var partnerUser = await _userManager.GetPartnerUser(partnerUserId);

                if (partnerUser != null)
                {
                    //sending email after successful partner solution creation.
                    _utilities.PrepareEmail(
                        "Your Microsoft Solutions Directory Partner Profile is under review",
                        partnerUser?.PartnerEmail, emailBody);
                }
                return partnerSolutionId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in class IndustryBuilder and method CreatePartnerSolution {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get the partner solution with partnerid.
        /// </summary>
        /// <param name="partnerSolutionId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<PartnerSolutionDTO> GetPartnerSolution(Guid partnerSolutionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update ispublish to true and remove the draft version of same partner solution if any.
        /// </summary>
        /// <param name="partnerSolutionPublishDTO"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Guid> PartnerSolutionPublish(PartnerSolutionPublishDTO partnerSolutionPublishDTO)
        {
            try
            {
                _logger.LogDebug("Entering the method PartnerSolutionPublish of class IndustryBuilder");
                var partnerSolution = await _partnerSolutionManager.GetPartnerSolution((Guid)partnerSolutionPublishDTO.PartnerSolutionId);

                if (partnerSolution == null)
                    throw new Exception("Partner Solution not found for the given partner solutionId");

                if (partnerSolution?.ParentSolutionId != null)
                {
                    var ParentPartnerSolutionId = new Guid(partnerSolution?.ParentSolutionId);

                    var partnerSolutionAreas = await _partnerSolutionManager.GetPartnerSolutionAreas((Guid)partnerSolutionPublishDTO.PartnerSolutionId);

                    var partnerSolutionAreaModels = partnerSolutionAreas.ToPartnerSolutionAreaModel();
                    foreach (var areaModel in partnerSolutionAreaModels)
                    {
                        var resourceModel = (await _partnerSolutionManager.GetPartnerSolutionResourceLinks((Guid)areaModel.PartnerSolutionByAreaId))
                            .ToPartnerSolutionResourceLinkModel(areaModel.PartnerSolutionByAreaId);

                        areaModel.PartnerSolutionResourceLinks = resourceModel;
                    }
                    await _partnerSolutionManager.UpdateParentPartnerSolutionAreas(partnerSolutionAreas, ParentPartnerSolutionId);
                    await _unitOfWork.CommitAsync();

                    var partnerSolutionAvailableGeos = await _partnerSolutionManager.GetPartnerSolutionAvailableGeos((Guid)partnerSolutionPublishDTO.PartnerSolutionId);

                    await _partnerSolutionManager.UpdateParentPartnerSolutionAvailableGeos(partnerSolutionAvailableGeos, ParentPartnerSolutionId);
                    await _unitOfWork.CommitAsync();

                    var partnerSolutionSpotLight = await _partnerSolutionManager.GetPartnerSolutionSpotLight((Guid)partnerSolutionPublishDTO.PartnerSolutionId);

                    await _partnerSolutionManager.UpdateParentPartnerSolutionSpotlight(partnerSolutionSpotLight, ParentPartnerSolutionId);
                    await _unitOfWork.CommitAsync();

                    await _partnerSolutionManager.UpdateParentPartnerSolution(partnerSolution);
                    await _partnerSolutionManager.RemovePartnerSolution((Guid)partnerSolutionPublishDTO.PartnerSolutionId);

                    await _unitOfWork.CommitAsync();

                    if (partnerSolutionAvailableGeos.Count > 0)
                    {
                        partnerSolutionAvailableGeos.ForEach(x =>
                        {
                            x.PartnerSolutionAvailableGeoId = Guid.NewGuid();
                            x.PartnerSolutionId = ParentPartnerSolutionId;
                        });
                        await _partnerSolutionManager.CreatePartnerAvailableGeo(partnerSolutionAvailableGeos);
                    }
                    if (partnerSolutionAreaModels.Count > 0)
                    {
                        partnerSolutionAreaModels?.ForEach(x =>
                        {
                            x.PartnerSolutionId = ParentPartnerSolutionId;

                        });
                        await _partnerSolutionManager.CreatePartnerSolutionArea(partnerSolutionAreaModels);
                    }

                    if (partnerSolutionSpotLight != null)
                    {
                        partnerSolutionSpotLight.SpotlightId = Guid.NewGuid();
                        partnerSolutionSpotLight.PartnerSolutionId = ParentPartnerSolutionId;
                        await _partnerSolutionManager.CreatePartnerSpotLight(partnerSolutionSpotLight);
                    }

                    await _unitOfWork.CommitAsync();
                }
                /*else
                {
                    await _partnerSolutionManager.PartnerSolutionPublish(partnerSolutionPublishDTO);
                    await _unitOfWork.CommitAsync();
                }*/

                return new Guid(partnerSolution.ParentSolutionId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in class IndustryBuilder and method PartnerSolutionPublish {ex.Message}");

                throw;
            }
        }

        /// <summary>
        /// Updating the partner solution details.
        /// </summary>
        /// <param name="partnerSolutionDTO"></param>
        /// <returns></returns>
        public async Task<Guid> UpdatePartnerSolution(PartnerSolutionDTO partnerSolutionDTO)
        {
            try
            {
                _logger.LogDebug("Entering the method UpdatePartnerSolution");
                var partnerSolutionId = await _partnerSolutionManager.UpdatePartnerSolution(partnerSolutionDTO.ToPartnerSolutionModel());

                //Creating the dependent objects.
                partnerSolutionDTO.PartnerSolutionId = partnerSolutionId;
                var partnerSolutionAvailableGeos = new List<PartnerSolutionAvailableGeo>();
                if (partnerSolutionDTO.SelectedGeos?.Count > 0)
                {
                    foreach (var geoId in partnerSolutionDTO.SelectedGeos)
                    {
                        var partnerSolutionAvailableGeo = new PartnerSolutionAvailableGeo
                        {
                            PartnerSolutionAvailableGeoId = Guid.NewGuid(),
                            PartnerSolutionId = partnerSolutionId,
                            Status = "Created",
                            RowChangedBy = partnerSolutionDTO?.RowChangedBy.ToString(),
                            RowChangedDate = DateTime.UtcNow,
                        };

                        if (Guid.TryParse(geoId, out var partnerSolutionAvailableGeoId))
                        {
                            partnerSolutionAvailableGeo.GeoId = partnerSolutionAvailableGeoId;
                        }

                        partnerSolutionAvailableGeos.Add(partnerSolutionAvailableGeo);
                    }
                }

                //partnerSolutionDTO.PartnerSolutionAvailableGeo?.ForEach(x => x.ParentSolutionId = partnerSolutionId);
                await _partnerSolutionManager.UpdatePartnerSolutionAvailableGeo(partnerSolutionAvailableGeos);

                partnerSolutionDTO?.PartnerSolutionAreas?.ForEach(x => x.ParentSolutionId = partnerSolutionId);
                await _partnerSolutionManager.UpdatePartnerSolutionArea(partnerSolutionDTO?.PartnerSolutionAreas?.ToPartnerSolutionAreaModel());

                //Updating the spotlight only if it exist.
                if (partnerSolutionDTO.SpotLight != null)
                {
                    partnerSolutionDTO.SpotLight.PartnerSolutionId = partnerSolutionId;

                    await _partnerSolutionManager.UpdatePartnerSolutionSpotlight(partnerSolutionDTO.SpotLight.ToSpotLoghtModel());
                }

                await _unitOfWork.CommitAsync();


                var partnerSolution = await _partnerSolutionManager.GetPartnerSolution(partnerSolutionId);
                var solutionStatus = await _lookupManager.GetSolutionStatus((Guid)partnerSolution.SolutionStatusId);
                // _knackContext.SolutionStatusType.Where(x => x.SolutionStatusId == partnerSolution.SolutionStatusId).First();
                string encKey = _configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
                if (solutionStatus.SolutionStatus.Equals("Approved", StringComparison.CurrentCultureIgnoreCase) ||
                    solutionStatus.SolutionStatus.Equals("Pending approval", StringComparison.CurrentCultureIgnoreCase) ||
                     solutionStatus.SolutionStatus.Equals("Correction required", StringComparison.CurrentCultureIgnoreCase))
                {
                    var newUserRecipientEmail = _configuration.GetSection("EmailSettings").GetSection("NewUserRecipientEmail").Value;
                    var filePath = _basefilePath + "\\Update_Industry_Solution.html";
                    var emailBody = File.ReadAllText(filePath);
                    emailBody = emailBody.Replace("#desc#", "Industry solution has been updated successfully.");
                    emailBody = emailBody.Replace("#website#", partnerSolution.SolutionOrgWebsite);
                    emailBody = emailBody.Replace("#status#", solutionStatus.SolutionStatus);
                    emailBody = emailBody.Replace("#description#", partnerSolution.SolutionDescription);
                    emailBody = emailBody.Replace("#solutionname#", partnerSolution.SolutionName);

                    var partnerUserId = (Guid)partnerSolutionDTO?.UserId;
                    var partnerUser = await _userManager.GetPartnerUser(partnerUserId);

                    if (partnerUser != null)
                    {

                        var partnerEmailAddress = _utilities.DecryptString(encKey, partnerUser.PartnerEmail);
                        //sending email after successful partner solution status update.                       
                        _utilities.SendMail(partnerEmailAddress, "Industry Solution Updated", emailBody);
                    }

                }
                else if (solutionStatus.SolutionStatus.Equals("Submitted for approval", StringComparison.CurrentCultureIgnoreCase))
                {

                    var newUserRecipientEmail = _configuration.GetSection("EmailSettings").GetSection("KnackAdmin").Value;
                    var filePath = _basefilePath + "\\Update_Industry_Solution.html";
                    var emailBody = File.ReadAllText(filePath);
                    emailBody = emailBody.Replace("#desc#", "Industry solution has been updated successfully.");
                    emailBody = emailBody.Replace("#website#", partnerSolution.SolutionOrgWebsite);
                    emailBody = emailBody.Replace("#status#", solutionStatus.SolutionStatus);
                    emailBody = emailBody.Replace("#description#", partnerSolution.SolutionDescription);
                    emailBody = emailBody.Replace("#solutionname#", partnerSolution.SolutionName);

                    _utilities.SendMail(newUserRecipientEmail, "Industry Solution Updated", emailBody);
                }

                return partnerSolutionId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in class IndustryBuilder and method UpdatePartnerSolution {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetPartnerSolution(Guid industryId, Guid subIndustryId)
        {
            var result = await _partnerSolutionManager.GetPartnerSolution(industryId, subIndustryId);

            return result;
        }
    }
}
