using Azure.Core;
using Knack.API.Common;
using Knack.API.Data;
using Knack.API.Interfaces;
using Knack.API.Mapping;
using Knack.API.Models;
using Knack.DBEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Knack.API.DataManagers
{
    public class PartnerSolutionManager : IPartnerSolutionManager
    {
        private readonly ILogger<PartnerSolutionManager> _logger;
        private readonly KnackContext _knackContext;
        private Utilities _utilities;
        public PartnerSolutionManager(ILogger<PartnerSolutionManager> logger,
            KnackContext knackContext, Utilities utilities)
        {
            _logger = logger;
            _knackContext = knackContext;
            _utilities = utilities;
        }
        /// <summary>
        /// Updating the partnersolution values.
        /// </summary>
        /// <param name="partnerSolution"></param>
        /// <returns></returns>
        public async Task<Guid> UpdatePartnerSolution(PartnerSolution partnerSolution)
        {
            try
            {
                _logger.LogDebug("Entering the method UpdatePartnerSolution");

                var solution = _knackContext.PartnerSolution.Where(t => t.PartnerSolutionId == partnerSolution.PartnerSolutionId).FirstOrDefault();
                var solutionStatus = _knackContext.SolutionStatusType.Where(t => t.SolutionStatusId == partnerSolution.SolutionStatusId).FirstOrDefault();
                //var slug = _utilities.GenerateSlug(partnerSolution.SolutionName);

                //  if (solution != null) { }

                //Inserting a new record when there is a published solution profile already available.
                //This functionality will give us the flexibility of showing the
                //existing partner profile to the user until the updated profile is approves.
                if (solution?.IsPublished == 1 && solutionStatus.SolutionStatus != "Hold" && solutionStatus.SolutionStatus != "Correction required")
                {
                    partnerSolution.ParentSolutionId = solution.PartnerSolutionId.ToString();
                    partnerSolution.OrganizationId = solution.OrganizationId;
                    var partnerSolutionId = await CreatePartnerSolution(partnerSolution);
                    partnerSolution.PartnerSolutionId = partnerSolutionId; //Setting the new partner solutionid.

                    return partnerSolutionId;
                }
                else
                {
                    //solution = partnerSolution;

                    //solution = partnerSolutionDTO.ToPartnerSolutionModel(slug);
                    solution.IndustryId = partnerSolution.IndustryId.GetValueOrDefault();
                    solution.SubIndustryId = partnerSolution.SubIndustryId.GetValueOrDefault();
                    solution.LogoFileLink = partnerSolution.LogoFileLink;
                    solution.MarketplaceLink = partnerSolution.MarketplaceLink;
                    solution.SolutionDescription = partnerSolution.SolutionDescription ?? "Test Description";
                    solution.SpecialOfferLink = partnerSolution.SpecialOfferLink;
                    solution.SolutionOrgWebsite = partnerSolution.SolutionOrgWebsite;
                    solution.SolutionName = partnerSolution.SolutionName;
                    solution.SolutionStatusId = partnerSolution.SolutionStatusId;
                    solution.IsPublished = 0;
                    solution.RowChangedBy = partnerSolution.RowChangedBy;
                    solution.RowChangedDate = DateTime.UtcNow;
                    
                    var resCheck = _knackContext.PartnerSolution.Where(t => t.PartnerSolutionSlug == solution.PartnerSolutionSlug).Where(t => t.PartnerSolutionId != partnerSolution.PartnerSolutionId).ToList();
                    
                    //    if (resCheck == null)
                    if (solution.PartnerSolutionSlug == null || resCheck.Count > 0)
                    {
                        var slug = _utilities.GenerateSlug(partnerSolution.SolutionName);
                        solution.PartnerSolutionSlug = slug;
                    }
                    solution.IndustryDesignation = partnerSolution.IndustryDesignation;

                    _knackContext.PartnerSolution.Update(solution);
                }

                return solution.PartnerSolutionId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method UpdatePartnerSolution{ex.Message}");

                throw;
            }
        }

        /// <summary>
        /// Create partner solution.
        /// </summary>
        /// <param name="partnerSolution"></param>
        /// <returns></returns>
        public async Task<Guid> CreatePartnerSolution(PartnerSolution partnerSolution)
        {
            try
            {
                _logger.LogDebug("Entering the method CreatePartnerSolution");

                var slug = _utilities.GenerateSlug(partnerSolution.SolutionName);
                partnerSolution.PartnerSolutionId = Guid.NewGuid();
                partnerSolution.IsPublished = 0;
                partnerSolution.RowChangedDate = DateTime.UtcNow;
                partnerSolution.RowCreatedDate = DateTime.UtcNow;
                partnerSolution.PartnerSolutionSlug = slug;

               await _knackContext.PartnerSolution.AddAsync(partnerSolution);
                //await _knackContext.SaveChangesAsync();

                return partnerSolution.PartnerSolutionId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method CreatePartnerSolution {ex.Message}");

                throw;
            }
        }

        /// <summary>
        /// Creating the available geo entries in DB.
        /// </summary>
        /// <param name="partnerSolutionAvailableGeos"></param>
        public async Task CreatePartnerAvailableGeo(List<PartnerSolutionAvailableGeo> partnerSolutionAvailableGeos)
        {
            try
            {
                _logger.LogDebug("Entering the method CreatePartnerAvailableGeo");

                await _knackContext.PartnerSolutionAvailableGeo.AddRangeAsync(partnerSolutionAvailableGeos);
                //await _knackContext.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method CreatePartnerAvailableGeo {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create Partner solution areas.
        /// </summary>
        /// <param name="partnerSolutionByAreas"></param>
        public async Task CreatePartnerSolutionArea(List<PartnerSolutionAreaModel> partnerSolutionAreaModels)
        {
            try
            {
                _logger.LogDebug("Entering the method CreatePartnerSolutionArea");

                if (partnerSolutionAreaModels?.Count> 0)
                {
                    foreach (var area in partnerSolutionAreaModels)
                    {
                        var partnerSolutionId = new Guid(area?.PartnerSolutionId.ToString());
                        var partnerSolutionArea = new PartnerSolutionByArea
                        {
                            PartnerSolutionByAreaId = area.PartnerSolutionByAreaId == null ? Guid.NewGuid() : area.PartnerSolutionByAreaId,
                            PartnerSolutionId = partnerSolutionId,
                            SolutionAreaId = area.SolutionAreaId,
                            //AreaSolutionDescription = area.AreaSolutionDescription,
                            Status = "Created",
                            RowChangedDate = DateTime.UtcNow,
                        };

                        await _knackContext.PartnerSolutionByArea.AddAsync(partnerSolutionArea);
                        await _knackContext.SaveChangesAsync();

                        if (area.PartnerSolutionResourceLinks?.Count > 0)
                        {
                            foreach (var resourceLink in area.PartnerSolutionResourceLinks)
                            {
                                var partnerSolutionResourceLink = new PartnerSolutionResourceLink
                                {
                                    PartnerSolutionResourceLinkId = Guid.NewGuid(),
                                    PartnerSolutionByAreaId = partnerSolutionArea.PartnerSolutionByAreaId,
                                    ResourceLinkId = (Guid)resourceLink.ResourceLinkId,
                                    ResourceLinkTitle = resourceLink.ResourceLinkTitle,
                                    ResourceLinkUrl = resourceLink.ResourceLinkUrl,
                                    EventDateTime = resourceLink.EventDateTime,
                                    ResourceLinkOverview = resourceLink.ResourceLinkOverview,
                                    Status = "Created",
                                    RowChangedDate = DateTime.UtcNow

                                };

                                await _knackContext.PartnerSolutionResourceLink.AddAsync(partnerSolutionResourceLink);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method CreatePartnerSolutionArea {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Creating the Partner spotlight entries in DB.
        /// </summary>
        /// <param name="spotLight"></param>
        public async Task CreatePartnerSpotLight(SpotLight spotLight)
        {
            try
            {
                _logger.LogDebug("Entering the method CreatePartnerSpotLight");

                if (spotLight != null)
                {
                    _knackContext.SpotLight.Add(spotLight);
                   // await _knackContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method CreatePartnerSpotLight. {ex.Message} ");
                throw;
            }
        }

        /// <summary>
        /// Updating the partner solution areas.
        /// </summary>
        /// <param name="partnerSolutionByAreas"></param>
        public async Task UpdatePartnerSolutionArea(List<PartnerSolutionAreaModel> partnerSolutionAreaModels)
        {
            try
            {
                _logger.LogDebug("Entering the method UpdatePartnerSolutionArea");
                if (partnerSolutionAreaModels.Count > 0)
                {
                    var solutionAreas = _knackContext.PartnerSolutionByArea.Where(t => t.PartnerSolutionId ==
                    partnerSolutionAreaModels.Take(1).FirstOrDefault().PartnerSolutionId).ToList();

                    if (solutionAreas?.Count > 0)
                    {
                        foreach (var area in solutionAreas)
                        {
                            var resourceses = _knackContext.PartnerSolutionResourceLink.Where(t => t.PartnerSolutionByAreaId == area.PartnerSolutionByAreaId).ToList();
                            if (resourceses?.Count > 0)
                            {
                                _knackContext.RemoveRange(resourceses);
                            }
                            _knackContext.Remove(area);
                        }
                    }

                    await CreatePartnerSolutionArea(partnerSolutionAreaModels);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method UpdatePartnerSolutionArea {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updating the partner solution geos.
        /// </summary>
        /// <param name="partnerSolutionAvailableGeo"></param>
        public async Task UpdatePartnerSolutionAvailableGeo(List<PartnerSolutionAvailableGeo> partnerSolutionAvailableGeo)
        {
            try
            {
                _logger.LogDebug("Entering the method UpdatePartnerSolutionGeo");

                var existingGeos = _knackContext.PartnerSolutionAvailableGeo.Where(t => t.PartnerSolutionId == partnerSolutionAvailableGeo.Take(1).FirstOrDefault().PartnerSolutionId).ToList();
                if (existingGeos?.Count > 0)
                {
                    foreach (var geo in existingGeos)
                    {
                        _knackContext.Remove(geo);
                       // await _knackContext.SaveChangesAsync();
                    }
                }

                await CreatePartnerAvailableGeo(partnerSolutionAvailableGeo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method UpdatePartnerSolutionGeo {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update Partner Spotlights
        /// </summary>
        /// <param name="spotLight"></param>
        public async Task UpdatePartnerSolutionSpotlight(SpotLight spotLight)
        {
            try
            {
                _logger.LogDebug("Entering the method UpdatePartnerSolutionSpotlight");
                if (spotLight != null)
                {
                    var existingSpotLight = _knackContext.SpotLight.FirstOrDefault(s => s.PartnerSolutionId == spotLight.PartnerSolutionId);
                    if (existingSpotLight == null)
                    {
                        await CreatePartnerSpotLight(spotLight);
                    }
                    else
                    {
                        spotLight.RowChangedDate = DateTime.UtcNow;
                        _knackContext.SpotLight.Update(spotLight);
                        //await _knackContext.SaveChangesAsync();
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($" Error occurred in method UpdatePartnerSolutionSpotlight{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceLinks"></param>
        public async Task CreatePartnerSolutionResourceLink(List<PartnerSolutionResourceLink> resourceLinks)
        {
            try
            {
                _logger.LogDebug("Entering the method CreatePartnerSolutionResourceLink");
                if (resourceLinks?.Count > 0)
                {
                    foreach (var resourceLink in resourceLinks)
                    {
                        var partnerSolutionResourceLink = new PartnerSolutionResourceLink
                        {
                            PartnerSolutionResourceLinkId = Guid.NewGuid(),
                            PartnerSolutionByAreaId = resourceLink.PartnerSolutionByAreaId,
                            ResourceLinkId = resourceLink.ResourceLinkId,
                            ResourceLinkTitle = resourceLink.ResourceLinkTitle,
                            ResourceLinkUrl = resourceLink.ResourceLinkUrl,
                            ResourceLinkOverview = resourceLink.ResourceLinkOverview,
                            Status = "Created",
                            RowChangedDate = DateTime.UtcNow,

                        };

                        _knackContext.PartnerSolutionResourceLink.Add(partnerSolutionResourceLink);
                       // await _knackContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method CreatePartnerSolutionResourceLink {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get partner solutin by partner solution Id.
        /// </summary>
        /// <param name="partnerSolutionId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PartnerSolution> GetPartnerSolution(Guid partnerSolutionId)
        {
            try
            {
                _logger.LogDebug($"Entering it to GetPartnerSolution: {partnerSolutionId}");

                var partnerSolution = await _knackContext.PartnerSolution.Where(x => x.PartnerSolutionId == partnerSolutionId).FirstOrDefaultAsync();
                return partnerSolution ?? throw new Exception($"No record found for the given partnersolutionId{partnerSolutionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method GetPartnerSolution {ex.Message}");
                throw;
            }

        }

        /// <summary>
        /// Update ispublish to true
        /// </summary>
        /// <param name="partnerSolution"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Guid> PartnerSolutionPublish(PartnerSolutionPublishDTO partnerSolutionPublishDTO)
        {
            try
            {
                _logger.LogDebug($"Entering it to PartnerSolutionPublish: {partnerSolutionPublishDTO.PartnerSolutionId}");
                var solution = _knackContext.PartnerSolution.Where(t => t.PartnerSolutionId == partnerSolutionPublishDTO.PartnerSolutionId).FirstOrDefault();
                solution.IsPublished = partnerSolutionPublishDTO.IsPublished;
                var resCheck = _knackContext.PartnerSolution.Where(t => t.PartnerSolutionSlug == solution.PartnerSolutionSlug).Where(t => t.PartnerSolutionId != solution.PartnerSolutionId).ToList();
                if (solution.PartnerSolutionSlug == null || resCheck.Count > 0)
                {
                    var slug = _utilities.GenerateSlug(solution.SolutionName);
                    solution.PartnerSolutionSlug = slug;
                }                
                _knackContext.PartnerSolution.Update(solution);
                await _knackContext.SaveChangesAsync();
                return solution.PartnerSolutionId;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method PartnerSolutionPublish {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update the parent partner availability geos.
        /// </summary>
        /// <param name="partnerSolutionAvailableGeo"></param>
        /// <param name="parentPartnerSolutionId"></param>
        /// <returns></returns>
        public async Task UpdateParentPartnerSolutionAvailableGeos(List<PartnerSolutionAvailableGeo> partnerSolutionAvailableGeo,
            Guid parentPartnerSolutionId)
        {
            try
            {
                _logger.LogDebug($"Entering it to UpdateParentPartnerSolutionAvailableGeos");
                if (partnerSolutionAvailableGeo?.Count > 0)
                {
                    var parentPartnerSolutionAvailableGeos = await GetPartnerSolutionAvailableGeos(parentPartnerSolutionId);
                    if (parentPartnerSolutionAvailableGeos?.Count > 0)
                    {
                        _knackContext.RemoveRange(parentPartnerSolutionAvailableGeos);

                    }
                    _knackContext.RemoveRange(partnerSolutionAvailableGeo);                
                  
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method UpdateParentPartnerSolutionAvailableGeos {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// update partner solution spotlight with parent partner solutionId.
        /// </summary>
        /// <param name="spotLight"></param>
        /// <param name="parentPartnerSolutionId"></param>
        /// <returns></returns>
        public async Task UpdateParentPartnerSolutionSpotlight(SpotLight spotLight, Guid parentPartnerSolutionId)
        {
            try
            {

                _logger.LogDebug($"Entering it to UpdateParentPartnerSolutionAvailableGeos");
                if (spotLight != null)
                {
                    var parentPartnerSolutionSpotLight = await GetPartnerSolutionSpotLight(parentPartnerSolutionId);
                    if (parentPartnerSolutionSpotLight != null)
                    {
                        _knackContext.Remove(parentPartnerSolutionSpotLight);

                    }
                    _knackContext.Remove(spotLight);
                    

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method UpdateParentPartnerSolutionAvailableGeos {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update partner solution resource links with parent partner solutionId.
        /// </summary>
        /// <param name="resourceLinks"></param>
        /// <param name="parentPartnerSolutionId"></param>
        /// <returns></returns>
        public async Task UpdateParentPartnerSolutionResourceLinks(List<PartnerSolutionResourceLink> resourceLinks,
            Guid parentPartnerSolutionId)
        {
            try
            {
                _logger.LogDebug($"Entering it to UpdateParentPartnerSolutionResourceLinks");
                if (resourceLinks?.Count > 0)
                {
                    var partnerSolutionResourceLink = resourceLinks.FirstOrDefault();
                    var parentPartnerSolutionResourceLink = await GetPartnerSolutionResourceLinks(partnerSolutionResourceLink.PartnerSolutionByAreaId);
                    if (parentPartnerSolutionResourceLink?.Count > 0)
                    {
                        _knackContext.RemoveRange(parentPartnerSolutionResourceLink);
                    }
                    _knackContext.RemoveRange(resourceLinks);                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method UpdateParentPartnerSolutionResourceLinks {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update partner solution areas based on partnersolution area.
        /// </summary>
        /// <param name="partnerSolutionAreas"></param>
        /// <param name="parentPartnerSolutionId"></param>
        /// <returns></returns>
        public async Task UpdateParentPartnerSolutionAreas(List<PartnerSolutionByArea> partnerSolutionAreas,
            Guid parentPartnerSolutionId)
        {
            try
            {
                _logger.LogDebug($"Entering it to UpdateParentPartnerSolutionAreas");
                if (partnerSolutionAreas?.Count > 0)
                {
                    foreach ( var area in partnerSolutionAreas)
                    {
                        var resourceLinks = await GetPartnerSolutionResourceLinks((Guid)area.PartnerSolutionByAreaId);
                        if (resourceLinks.Count > 0)
                        {
                            _knackContext.RemoveRange(resourceLinks);
                            _knackContext.SaveChanges();
                        }
                    }
                    //var partnerSolutionAreas = partnerSolutionArea.ToPartnerSolutionArea();
                    if (partnerSolutionAreas.Count > 0)
                    {
                        _knackContext.RemoveRange(partnerSolutionAreas);
                    }
                    var parentpartnerSolutionByAreas = await GetPartnerSolutionAreas(parentPartnerSolutionId);
                    if (parentpartnerSolutionByAreas?.Count > 0)
                    {
                        _knackContext.RemoveRange(parentpartnerSolutionByAreas);
                    }              
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method UpdateParentPartnerSolutionAreas {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all the partner solution available Geos based on partner solutionId.
        /// </summary>
        /// <param name="partnerSolutionId"></param>
        /// <returns></returns>
        public async Task<List<PartnerSolutionAvailableGeo>> GetPartnerSolutionAvailableGeos(Guid partnerSolutionId)
        {
            try
            {
                _logger.LogDebug($"Entering it to GetPartnerSolutionAvailableGeos");
                var partnerSolutionAvailableGeos = await _knackContext.PartnerSolutionAvailableGeo.Where(x => x.PartnerSolutionId == partnerSolutionId).ToListAsync();
                return partnerSolutionAvailableGeos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method GetPartnerSolutionAvailableGeos {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all the partner solution areas based on partnersolutionId.
        /// </summary>
        /// <param name="partnerSolutionId"></param>
        /// <returns></returns>
        public async Task<List<PartnerSolutionByArea>> GetPartnerSolutionAreas(Guid partnerSolutionId)
        {
            try
            {
                _logger.LogDebug($"Entering it to GetPartnerSolutionAreas");
                var partnerSolutionAreas = await _knackContext.PartnerSolutionByArea.Where(x => x.PartnerSolutionId == partnerSolutionId).ToListAsync();
                return partnerSolutionAreas;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method GetPartnerSolutionAreas {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Getting the partner solution spotlight details.
        /// </summary>
        /// <param name="parentPartnerSolutionId"></param>
        /// <returns></returns>
        public async Task<SpotLight> GetPartnerSolutionSpotLight(Guid parentPartnerSolutionId)
        {
            try
            {
                _logger.LogDebug($"Entering it to GetPartnerSolutionSpotLight");
                var parentSolutionSpotLight = await _knackContext.SpotLight.Where(x => x.PartnerSolutionId == parentPartnerSolutionId).FirstOrDefaultAsync();
                return parentSolutionSpotLight;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method GetPartnerSolutionSpotLight {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get all the partner solution resource links based on partnersolutionId.
        /// </summary>
        /// <param name="partnerSolutionId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<PartnerSolutionResourceLink>> GetPartnerSolutionResourceLinks(Guid partnerSolutionByAreaId)
        {
            try
            {
                _logger.LogDebug($"Entering it to GetPartnerSolutionResourceLinks");
                var partnerSolutionResourceLink = await _knackContext.PartnerSolutionResourceLink.Where(x => x.PartnerSolutionByAreaId == partnerSolutionByAreaId).ToListAsync();
                return partnerSolutionResourceLink;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method GetPartnerSolutionResourceLinks {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update parent partner solution with child partner solution details.
        /// </summary>
        /// <param name="childPartnerSolution"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Guid> UpdateParentPartnerSolution(PartnerSolution childPartnerSolution)
        {
            try
            {
                if (childPartnerSolution == null)
                {
                throw new ArgumentNullException(nameof(childPartnerSolution));
                }
                _logger.LogDebug("Entering the method UpdateParentPartnerSolution");

                var parentPartnersolution = _knackContext.PartnerSolution.Where(t => t.PartnerSolutionId == new Guid(childPartnerSolution.ParentSolutionId)).FirstOrDefault();

                parentPartnersolution.IndustryId = childPartnerSolution.IndustryId.GetValueOrDefault();
                parentPartnersolution.SubIndustryId = childPartnerSolution.SubIndustryId.GetValueOrDefault();
                parentPartnersolution.LogoFileLink = childPartnerSolution.LogoFileLink;
                parentPartnersolution.MarketplaceLink = childPartnerSolution.MarketplaceLink;
                parentPartnersolution.SolutionDescription = childPartnerSolution.SolutionDescription ?? "Test Description";
                parentPartnersolution.SpecialOfferLink = childPartnerSolution.SpecialOfferLink;
                parentPartnersolution.SolutionOrgWebsite = childPartnerSolution.SolutionOrgWebsite;
                parentPartnersolution.SolutionName = childPartnerSolution.SolutionName;
                parentPartnersolution.SolutionStatusId = childPartnerSolution.SolutionStatusId;
                parentPartnersolution.IsPublished = 1;
                parentPartnersolution.RowChangedBy = childPartnerSolution.RowChangedBy;
                parentPartnersolution.RowChangedDate = DateTime.UtcNow;
                var resCheck = _knackContext.PartnerSolution.Where(t => t.PartnerSolutionSlug == parentPartnersolution.PartnerSolutionSlug).Where(t => t.PartnerSolutionId != parentPartnersolution.PartnerSolutionId).ToList();

                if (parentPartnersolution.PartnerSolutionSlug == null || resCheck.Count > 0)
                {
                    var slug = _utilities.GenerateSlug(childPartnerSolution.SolutionName);
                    parentPartnersolution.PartnerSolutionSlug = slug;
                }
                //parentPartnersolution.PartnerSolutionSlug = childPartnerSolution.PartnerSolutionSlug;
                parentPartnersolution.IndustryDesignation = childPartnerSolution.IndustryDesignation;

                _knackContext.PartnerSolution.Update(parentPartnersolution);               

                //await _knackContext.SaveChangesAsync();

                return parentPartnersolution.PartnerSolutionId;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method UpdateParentPartnerSolution{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete the partner solution details based on partner solutionId.
        /// </summary>
        /// <param name="partnerSolutionId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task RemovePartnerSolution(Guid partnerSolutionId)
        {
            try
            {            
            _logger.LogDebug("Entering the method RemovePartnerSolution");
            _knackContext.PartnerSolution.Remove(await GetPartnerSolution(partnerSolutionId));

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in the method RemovePartnerSolution {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update the partner solution resource links.
        /// </summary>
        /// <param name="partnerSolutionResourceLink"></param>
        /// <returns></returns>
        public async Task UpdatePartnerSolutionResourceLink(List<PartnerSolutionResourceLink> partnerSolutionResourceLink)
        {
            try
            {
                _logger.LogDebug("Entering the method UpdatePartnerSolutionResourceLink");

                if (partnerSolutionResourceLink?.Count > 0)
                {

                    _knackContext.PartnerSolutionResourceLink.UpdateRange(partnerSolutionResourceLink);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method UpdatePartnerSolutionResourceLink {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get partnersolution detail with industryid and subindustryid
        /// </summary>
        /// <param name="industryId"></param>
        /// <param name="subIndustryId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetPartnerSolution(Guid industryId, Guid subIndustryId)
        {
            var result = await _knackContext.PartnerSolution
                .Where(ps => ps.IndustryId == industryId && ps.SubIndustryId == subIndustryId)
                .Select(x=>x.SolutionDescription)
                .ToListAsync();
            return result;
        }

    }
}
