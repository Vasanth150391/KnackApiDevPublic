using Knack.API.Data;
using Knack.API.Models;
using Knack.DBEntities;

namespace Knack.API.Mapping
{
    public static class PartnerSolutionMapping
    {
        private static KnackContext _knackContext;
        static PartnerSolutionMapping()
        {
            _knackContext = new KnackContext();
        }

        /// <summary>
        /// Mapping PartnerSolutionDTO to model.
        /// </summary>
        /// <param name="partnerSolutionDTO"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        public static PartnerSolution ToPartnerSolutionModel(this PartnerSolutionDTO partnerSolutionDTO)
        {
            try
            {

                var partnerSolution = new PartnerSolution();
                partnerSolution.PartnerSolutionId = partnerSolutionDTO.PartnerSolutionId!=null?
                    (Guid)partnerSolutionDTO.PartnerSolutionId:Guid.NewGuid();
                partnerSolution.IndustryId = partnerSolutionDTO.IndustryId.GetValueOrDefault();
                partnerSolution.SubIndustryId = partnerSolutionDTO.SubIndustryId.GetValueOrDefault();
                partnerSolution.LogoFileLink = partnerSolutionDTO.LogoFileLink;
                partnerSolution.MarketplaceLink = partnerSolutionDTO.MarketplaceLink;
                partnerSolution.SolutionDescription = partnerSolutionDTO.SolutionDescription ?? "Test Description";
                partnerSolution.SpecialOfferLink = partnerSolutionDTO.SpecialOfferLink;
                partnerSolution.SolutionOrgWebsite = partnerSolutionDTO.SolutionOrgWebsite;
                partnerSolution.SolutionName = partnerSolutionDTO.SolutionName;
                partnerSolution.SolutionStatusId = partnerSolutionDTO.SolutionStatusId;
                partnerSolution.IsPublished = 0;
                partnerSolution.RowChangedBy = partnerSolutionDTO.RowChangedBy;
                partnerSolution.RowChangedDate = DateTime.UtcNow;
                partnerSolution.PartnerSolutionSlug = partnerSolutionDTO.PartnerSolutionSlug;
                partnerSolution.IndustryDesignation = partnerSolutionDTO.IndustryDesignation;
                partnerSolution.UserId=(Guid)partnerSolutionDTO.UserId;
                partnerSolution.OrganizationId = partnerSolutionDTO.OrganizationId;
                return partnerSolution;

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Mapping partnersolution model to partnersolutionDTO
        /// </summary>
        /// <param name="partnerSolution"></param>
        /// <returns></returns>
        public static PartnerSolutionDTO ToPartnerSolutionDTO(this PartnerSolution partnerSolution)
        {
            try
            {
                var partnerSolutionAreas = _knackContext.PartnerSolutionByArea.Where(x => x.PartnerSolutionId == partnerSolution.PartnerSolutionId).ToList();
                var partnerSolutionAvailableGeo = _knackContext.PartnerSolutionAvailableGeo.Where(x => x.PartnerSolutionId == partnerSolution.PartnerSolutionId).ToList();
                var partnerSolutionDTO = new PartnerSolutionDTO()
                {
                    IndustryDesignation = partnerSolution.IndustryDesignation,
                    IndustryId = partnerSolution.IndustryId,
                    IsPublished = partnerSolution.IsPublished,
                    LogoFileLink = partnerSolution.LogoFileLink,
                    MarketplaceLink = partnerSolution.MarketplaceLink,
                    OrganizationId = partnerSolution.OrganizationId,
                    ParentSolutionId = partnerSolution.ParentSolutionId,
                   // PartnerSolutionAreas = ToPartnerSolutionAreasDTOs(partnerSolutionAreas),
                    PartnerSolutionAvailableGeo = ToPartnerSolutionAvailableGeoDTOs(partnerSolutionAvailableGeo)
                };

                return partnerSolutionDTO;

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Mapping partnersolutionArea model to dto.
        /// </summary>
        /// <param name="solutionAreas"></param>
        /// <returns></returns>
        public static List<PartnerSolutionAreaDTO> ToPartnerSolutionAreasDTOs(this List<PartnerSolutionAreaModel> partnerSolutionByAreaModels)
        {
            try
            {
                var PartnerSolutionAreasDTO = new List<PartnerSolutionAreaDTO>();
                var partnerSolutionResourceLinks=new List<PartnerSolutionResourceLink>();
                _knackContext = new KnackContext();
                foreach (var solutionArea in partnerSolutionByAreaModels)
                {
                    //var partnerSolutionResourceLink = _knackContext.PartnerSolutionResourceLink.Where(x => x.PartnerSolutionByAreaId == solutionArea.SolutionAreaId).ToList();

                    foreach (var item in solutionArea.PartnerSolutionResourceLinks)
                    {
                        var partnerSolutionResourceLink = new PartnerSolutionResourceLink()
                        {
                            EventDateTime = item.EventDateTime,
                            ResourceLinkOverview = item.ResourceLinkOverview,
                            PartnerSolutionByAreaId = (Guid)item.PartnerSolutionByAreaId,
                            PartnerSolutionResourceLinkId = (Guid)item.ResourceLinkId,
                            ResourceLinkTitle = item.ResourceLinkTitle,
                            ResourceLinkUrl = item.ResourceLinkUrl,
                            ResourceLinkId = (Guid)item.ResourceLinkId,
                            Status = item.Status.ToString()
                        };

                        partnerSolutionResourceLinks.Add(partnerSolutionResourceLink);
                    }
                    
                    var areaDTO = new PartnerSolutionAreaDTO()
                    {
                        areaSolutionDescription = solutionArea.AreaSolutionDescription,
                        partnersolutionByAreaId = solutionArea.PartnerSolutionByAreaId,
                        PartnerSolutionResourceLinks = ToPartnerSolutionResourceLinkDTOs(partnerSolutionResourceLinks)
                    };

                    PartnerSolutionAreasDTO.Add(areaDTO);
                }

                return PartnerSolutionAreasDTO;

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Mapping PartnersolutionResourcelink model to dto.
        /// </summary>
        /// <param name="partnerSolutionResourceLinks"></param>
        /// <returns></returns>
        public static List<PartnerSolutionResourceLinkDTO> ToPartnerSolutionResourceLinkDTOs(
           this List<PartnerSolutionResourceLink> partnerSolutionResourceLinks)
        {
            try
            {

                var partnerSolutionResourceLinkDTOs = new List<PartnerSolutionResourceLinkDTO>();

                foreach (var item in partnerSolutionResourceLinks)
                {
                    var partnerSolutionResourceLinkDTO = new PartnerSolutionResourceLinkDTO()
                    {
                        resourceLinkId = item.ResourceLinkId,
                        resourceLinkOverview = item.ResourceLinkOverview,
                        resourceLinkTitle = item.ResourceLinkTitle,
                        resourceLinkUrl = item.ResourceLinkUrl,
                    };

                    partnerSolutionResourceLinkDTOs.Add(partnerSolutionResourceLinkDTO);
                }

                return partnerSolutionResourceLinkDTOs;

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Mapping PartnerSolutionAvailableGeo model to dto.
        /// </summary>
        /// <param name="partnerSolutionAvailableGeos"></param>
        /// <returns></returns>
        public static List<PartnerSolutionAvailableGeoDTO> ToPartnerSolutionAvailableGeoDTOs(
           this List<PartnerSolutionAvailableGeo> partnerSolutionAvailableGeos)
        {
            try
            {

                var partnerSolutionAvailableGeoDTOs = new List<PartnerSolutionAvailableGeoDTO>();

                foreach (var item in partnerSolutionAvailableGeos)
                {
                    var partnerSolutionAvailableGeoDTO = new PartnerSolutionAvailableGeoDTO()
                    {
                        GeoId = item.GeoId,
                        Geoname = _knackContext.Geos.Where(x => x.GeoId == item.GeoId).FirstOrDefault().Geoname
                    };
                    partnerSolutionAvailableGeoDTOs.Add(partnerSolutionAvailableGeoDTO);

                }

                return partnerSolutionAvailableGeoDTOs;

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Mapping partner solution area dto to model.
        /// </summary>
        /// <param name="partnerSolutionAreaDTOs"></param>
        /// <returns></returns>
        public static List<PartnerSolutionAreaModel> ToPartnerSolutionAreaModel(this List<PartnerSolutionAreaDTO> partnerSolutionAreaDTOs)
        {
            var PartnerSolutionByAreas = new List<PartnerSolutionAreaModel>();
            try
            {
                foreach (var areaDTO in partnerSolutionAreaDTOs)
                {
                    var partnerSolutionByArea = new PartnerSolutionAreaModel()
                    {
                        AreaSolutionDescription = areaDTO.areaSolutionDescription,
                        PartnerSolutionByAreaId = areaDTO.partnersolutionByAreaId !=null?(Guid)areaDTO.partnersolutionByAreaId:Guid.NewGuid(),
                        PartnerSolutionId = (Guid)areaDTO.ParentSolutionId,
                        SolutionAreaId = areaDTO.solutionAreaId ,
                        PartnerSolutionResourceLinks= areaDTO.PartnerSolutionResourceLinks.ToPartnerSolutionResourceLinkModel(areaDTO.partnersolutionByAreaId != null ? 
                        (Guid)areaDTO.partnersolutionByAreaId : Guid.NewGuid())

                    };
                    PartnerSolutionByAreas.Add(partnerSolutionByArea);
                }

                return PartnerSolutionByAreas;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Mapp partnersolutionareas to partnerareasolutionmodel.
        /// </summary>
        /// <param name="partnerSolutionByAreas"></param>
        /// <returns></returns>
        public static List<PartnerSolutionAreaModel> ToPartnerSolutionAreaModel(this List<PartnerSolutionByArea> partnerSolutionByAreas)
        {
            var PartnerSolutionByAreas = new List<PartnerSolutionAreaModel>();
            try
            {
                foreach (var areas in partnerSolutionByAreas)
                {
                    var partnerSolutionByArea = new PartnerSolutionAreaModel()
                    {
                        AreaSolutionDescription = areas.AreaSolutionDescription,
                        PartnerSolutionByAreaId = areas.PartnerSolutionByAreaId != null ? (Guid)areas.PartnerSolutionByAreaId : Guid.NewGuid(),
                        PartnerSolutionId = (Guid)areas.PartnerSolutionId,
                        SolutionAreaId = areas.SolutionAreaId,
                       // PartnerSolutionResourceLinks = areas..ToPartnerSolutionResourceLinkModel(areas.partnersolutionByAreaId != null ?
                        //(Guid)areas.PartnerSolutionByAreaId : Guid.NewGuid())
                        
                    };
                    PartnerSolutionByAreas.Add(partnerSolutionByArea);
                }

                return PartnerSolutionByAreas;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Mapping partner solution area model to partner solutionbyarea.
        /// </summary>
        /// <param name="partnerSolutionAreaModels"></param>
        /// <returns></returns>
        public static List<PartnerSolutionByArea> ToPartnerSolutionArea(this List<PartnerSolutionAreaModel> partnerSolutionAreaModels)
        {
            var PartnerSolutionByAreas = new List<PartnerSolutionByArea>();
            try
            {
                foreach (var areas in partnerSolutionAreaModels)
                {
                    var partnerSolutionByArea = new PartnerSolutionByArea()
                    {
                        AreaSolutionDescription = areas.AreaSolutionDescription,
                        PartnerSolutionByAreaId = areas.PartnerSolutionByAreaId != null ? (Guid)areas.PartnerSolutionByAreaId : Guid.NewGuid(),
                        PartnerSolutionId = (Guid)areas.PartnerSolutionId,
                        SolutionAreaId = areas.SolutionAreaId,
                        // PartnerSolutionResourceLinks = areas..ToPartnerSolutionResourceLinkModel(areas.partnersolutionByAreaId != null ?
                        //(Guid)areas.PartnerSolutionByAreaId : Guid.NewGuid())

                    };
                    PartnerSolutionByAreas.Add(partnerSolutionByArea);
                }

                return PartnerSolutionByAreas;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Mapping PartnerSolutionAvailableGeoDTO dto to model.
        /// </summary>
        /// <param name="partnerSolutionAvailableGeoDTOs"></param>
        /// <returns></returns>
        public static List<PartnerSolutionAvailableGeo> ToPartnerSolutionAvailableGeoModel(
           this List<PartnerSolutionAvailableGeoDTO> partnerSolutionAvailableGeoDTOs)
        {
            try
            {
                var partnerSolutionAvailableGeos = new List<PartnerSolutionAvailableGeo>();

                foreach (var item in partnerSolutionAvailableGeoDTOs)
                {
                    var partnerSolutionAvailableGeo = new PartnerSolutionAvailableGeo()
                    {
                        GeoId = item.GeoId,
                        PartnerSolutionAvailableGeoId = Guid.NewGuid(),
                        PartnerSolutionId = item.ParentSolutionId,
                        Status="Created",
                        RowChangedDate = DateTime.Now
                    };
                    partnerSolutionAvailableGeos.Add(partnerSolutionAvailableGeo);
                }

                return partnerSolutionAvailableGeos;

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Mapping PartnersolutionResourcelink dto to model.
        /// </summary>
        /// <param name="partnerSolutionResourceLinkDTOs"></param>
        /// <returns></returns>
        public static List<PartnerSolutionResourceLinkModel> ToPartnerSolutionResourceLinkModel(
           this List<PartnerSolutionResourceLinkDTO> partnerSolutionResourceLinkDTOs,Guid partnerSolutionAreaId)
        {
            try
            {
                var partnerSolutionResourceLinks = new List<PartnerSolutionResourceLinkModel>();

                foreach (var item in partnerSolutionResourceLinkDTOs)
                {
                    var partnerSolutionResourceLink = new PartnerSolutionResourceLinkModel()
                    {
                        ResourceLinkId = item.resourceLinkId != null ? (Guid)item.resourceLinkId : Guid.NewGuid(),
                        ResourceLinkOverview = item.resourceLinkOverview,
                        ResourceLinkTitle = item.resourceLinkTitle,
                        ResourceLinkUrl = item.resourceLinkUrl,
                        PartnerSolutionByAreaId = partnerSolutionAreaId,
                        EventDateTime=item.eventDateTime,
                       // PartnerSolutionResourceLinkId=item.
                    };

                    partnerSolutionResourceLinks.Add(partnerSolutionResourceLink);
                }

                return partnerSolutionResourceLinks;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Mapping partnersolution resource links models to partnersolution resource links.
        /// </summary>
        /// <param name="partnerSolutionResourceLinkModels"></param>      
        /// <returns></returns>
        public static List<PartnerSolutionResourceLink> ToPartnerSolutionResourceLink(
          this List<PartnerSolutionResourceLinkModel> partnerSolutionResourceLinkModels)
        {
            try
            {
                var partnerSolutionResourceLinks = new List<PartnerSolutionResourceLink>();

                foreach (var item in partnerSolutionResourceLinkModels)
                {
                    var partnerSolutionResourceLink = new PartnerSolutionResourceLink()
                    {
                        ResourceLinkId = item.ResourceLinkId != null ? (Guid)item.ResourceLinkId : Guid.NewGuid(),
                        ResourceLinkOverview = item.ResourceLinkOverview,
                        ResourceLinkTitle = item.ResourceLinkTitle,
                        ResourceLinkUrl = item.ResourceLinkUrl,
                        PartnerSolutionByAreaId = (Guid)item.PartnerSolutionByAreaId,
                        EventDateTime = item.EventDateTime,
                        PartnerSolutionResourceLinkId = item.PartnerSolutionResourceLinkId,
                    };

                    partnerSolutionResourceLinks.Add(partnerSolutionResourceLink);
                }

                return partnerSolutionResourceLinks;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<PartnerSolutionResourceLinkModel> ToPartnerSolutionResourceLinkModel(
          this List<PartnerSolutionResourceLink> partnerSolutionResourceLinks, Guid partnerSolutionAreaId)
        {
            try
            {
                var partnerSolutionResourceLinkModels = new List<PartnerSolutionResourceLinkModel>();

                foreach (var item in partnerSolutionResourceLinks)
                {
                    var partnerSolutionResourceLink = new PartnerSolutionResourceLinkModel()
                    {
                        ResourceLinkId = item.ResourceLinkId != null ? (Guid)item.ResourceLinkId : Guid.NewGuid(),
                        ResourceLinkOverview = item.ResourceLinkOverview,
                        ResourceLinkTitle = item.ResourceLinkTitle,
                        ResourceLinkUrl = item.ResourceLinkUrl,
                        PartnerSolutionByAreaId = partnerSolutionAreaId,
                        EventDateTime = item.EventDateTime,
                        PartnerSolutionResourceLinkId = item.PartnerSolutionResourceLinkId
                    };

                    partnerSolutionResourceLinkModels.Add(partnerSolutionResourceLink);
                }

                return partnerSolutionResourceLinkModels;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Mapping spotlight dto to model.
        /// </summary>
        /// <param name="spotLightDTO"></param>
        /// <returns></returns>
        public static SpotLight ToSpotLoghtModel(this SpotLightDTO spotLightDTO)
        {
            try
            {
                return new SpotLight
                {
                    OrganizationId = spotLightDTO.OrganizationId,
                    PartnerSolutionId = spotLightDTO.PartnerSolutionId,
                    SpotlightId = spotLightDTO.SpotlightId,
                    SpotlightOverview = spotLightDTO.SpotlightOverview,
                    UsecaseId = spotLightDTO.UsecaseId
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Mapping spotlight dto to model.
        /// </summary>
        /// <param name="spotLightDTO"></param>
        /// <returns></returns>
        public static SpotLightDTO ToSpotLoghtDto(this SpotLight spotLight)
        {
            try
            {
                return new SpotLightDTO
                {
                    OrganizationId = (Guid)spotLight.OrganizationId,
                    PartnerSolutionId = spotLight.PartnerSolutionId,
                    SpotlightId = (Guid)spotLight.SpotlightId,
                    SpotlightOverview = spotLight.SpotlightOverview,
                    UsecaseId = spotLight.UsecaseId
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Maps IEnumerable<PartnerSolution> to List<PartnerSolutionDTO>.
        /// </summary>
        /// <param name="partnerSolutions"></param>
        /// <returns></returns>
        public static List<PartnerSolutionDTO> ToPartnerSolutionDTOList(this IEnumerable<PartnerSolution> partnerSolutions)
        {
            var dtoList = new List<PartnerSolutionDTO>();
            foreach (var solution in partnerSolutions)
            {
                dtoList.Add(solution.ToPartnerSolutionDTO());
            }
            return dtoList;
        }

    }
}
