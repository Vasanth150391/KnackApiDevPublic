using Knack.API.Data;
using Knack.API.Interfaces;
using Knack.API.Models;
using Microsoft.EntityFrameworkCore;
using Knack.API.Mapping;
using Knack.DBEntities;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Knack.API.DataManagers
{
    public class TechnologyManager : ITechnologyManager
    {
        private readonly KnackContext _context;
        public TechnologyManager(KnackContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Getting all the solution areas whose solution play and solution profiles are active and published.
        /// </summary>
        /// <returns></returns>
        public Task<List<SolutionAreaDTO>> GetAllPublishedSolutionAreas()
        {
            var solutionAreas = (from sa in _context.SolutionAreas
                                 join psp in _context.PartnerSolutionPlay
                                 on sa.SolutionAreaId equals psp.SolutionAreaId
                                 join playByPlay in _context.PartnerSolutionPlayByPlay
                                 on psp.PartnerSolutionPlayId equals playByPlay.PartnerSolutionPlayId
                                 join solutionPlay in _context.SolutionPlay
                                 on playByPlay.SolutionPlayId equals solutionPlay.SolutionPlayId
                                 join ss in _context.SolutionStatusType
                                 on psp.SolutionStatusId equals ss.SolutionStatusId
                                 join org in _context.Organization
                                 on psp.OrgId equals org.OrgId
                                 where solutionPlay.SolutionPlayId == playByPlay.SolutionPlayId
                                 && ss.SolutionStatus == "Approved" && solutionPlay.IsPublished==1
                                 && psp.IsPublished == 1
                                 orderby sa.SolutionAreaName ascending
                                 select new SolutionAreaDTO
                                 {
                                     SolutionAreaId = sa.SolutionAreaId,
                                     ImageMain = sa.Image_Main,
                                     ImageMobile = sa.Image_Mobile,
                                     ImageThumb = sa.Image_Thumb,
                                     SolAreaDescription = sa.SolAreaDescription,
                                     SolutionAreaName = sa.SolutionAreaName,
                                     SolutionAreaSlug = sa.SolutionAreaSlug
                                 }).Distinct().OrderBy(i => i.SolutionAreaName).ToListAsync();
            
            return solutionAreas;
        }

        public async Task<List<OrganizationPartnerSolutionDTO>> GetAssociatedPartnerSolutions(string orgId)
        {
            var organizationId = new Guid(orgId);
            var organizationPartnerSolutions = await (from org in _context.Organization
                                                      join ps in _context.PartnerSolution
                                                      on org.OrgId equals ps.OrganizationId
                                                      join ss in _context.SolutionStatusType
                                                      on ps.SolutionStatusId equals ss.SolutionStatusId
                                                      where org.OrgId.Equals(organizationId) && ps.IsPublished.Equals(1)
                                                      where ss.SolutionStatus == "Approved"
                                                      select new OrganizationPartnerSolutionDTO
                                                      {
                                                          PartnerSolutionId = ps.PartnerSolutionId,
                                                          IndustryId = ps.IndustryId,
                                                          SubIndustryId = ps.SubIndustryId,
                                                          OrganizationId = ps.OrganizationId,
                                                          OrganizationName = org.OrgName,
                                                          SolutionStatusId = ps.SolutionStatusId,
                                                          SolutionStatus = ss.SolutionStatus,
                                                          DisplayLabel = ss.DisplayLabel,
                                                          SolutionName = ps.SolutionName,
                                                          IsPublished = ps.IsPublished,
                                                          PartnerSolutionSlug = ps.PartnerSolutionSlug,
                                                          PartnerSolutionDesc = ps.SolutionDescription,
                                                          LogoFileLink = ps.LogoFileLink
                                                      }).ToListAsync();

            return organizationPartnerSolutions;
        }

        /// <summary>
        /// Getting solution area based on area id.
        /// </summary>
        /// <param name="AreaId"></param>
        /// <returns></returns>
        public async Task<List<SolutionAreaDTO>> GetSolutionArea(string AreaId)
        {
            try
            {

                var solutionAreas = await _context.SolutionAreas
                 .Where(x => x.Status.Equals("Active") && x.SolutionAreaId.Equals(AreaId)).OrderBy(i => i.DisplayOrder).ToListAsync();

                return solutionAreas.ToSolutionAreaDTO();

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Getting all solution areas.
        /// </summary>
        /// <returns></returns>
        public async Task<List<SolutionAreaDTO>> GetSolutionAreas()
        {
            try
            {

                var solutionAreas = await _context.SolutionAreas
                 .Where(x => x.Status.Equals("Active")).OrderBy(i => i.DisplayOrder).ToListAsync();

                return solutionAreas.ToSolutionAreaDTO();

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Getting all the active solution area and its corresponding plays.
        /// </summary>
        /// <returns></returns>
        public async Task<List<SolutionAreaPlayDTO>> GetSolutionAreaWithPlays()
        {
            try
            {

                var solutionAreasWithPlays = await (from sa in _context.SolutionAreas
                                                    join sp in _context.SolutionPlay on sa.SolutionAreaId equals sp.SolutionAreaId
                                                    join ss in _context.SolutionStatusType on sp.SolutionStatusId equals ss.SolutionStatusId
                                                    where ss.SolutionStatus.Equals("Approved") && sa.Status.Equals("Active") && sp.IsPublished.Equals(1)
                                                    orderby sa.SolutionAreaName
                                                    select new
                                                    {
                                                        sa.SolutionAreaId,
                                                        sa.Image_Main,
                                                        sa.Image_Mobile,
                                                        sa.Image_Thumb,
                                                        sa.SolAreaDescription,
                                                        sa.SolutionAreaName,
                                                        sa.SolutionAreaSlug,
                                                        sa.DisplayOrder,
                                                        sp
                                                    }).OrderBy(i => i.DisplayOrder).GroupBy(x => x.SolutionAreaId).ToListAsync();

                var solutionAreaPlayDTOs = new List<SolutionAreaPlayDTO>();
                foreach (var solutionArea in solutionAreasWithPlays)
                {
                    //Getting the first record to create the Solution area details
                    var firstsolutionArea = (from sa in solutionArea select sa).FirstOrDefault();

                    var solutionAreas = (from sa in solutionArea select sa).ToList();

                    var solutionPlayDtos = new List<SolutionPlayDTO>();
                    var solutionPlayDto = new SolutionPlayDTO();
                    var solutionAreaPlayDTO = new SolutionAreaPlayDTO();

                    solutionAreaPlayDTO.SolutionAreaSlug = firstsolutionArea.SolutionAreaSlug;
                    solutionAreaPlayDTO.SolutionAreaId = firstsolutionArea.SolutionAreaId;
                    solutionAreaPlayDTO.SolutionAreaName = firstsolutionArea.SolutionAreaName;
                    solutionAreaPlayDTO.ImageMain = firstsolutionArea.Image_Main;
                    solutionAreaPlayDTO.ImageThumb = firstsolutionArea.Image_Thumb;
                    solutionAreaPlayDTO.SolutionAreaDescription = firstsolutionArea.SolAreaDescription;
                    solutionAreaPlayDTO.ImageMobile = firstsolutionArea.Image_Mobile;
                    //Looping the grouped solutionarea to get the list of solution plays corresponding to it.
                    foreach (var item in solutionAreas)
                    {
                        solutionPlayDto.SolutionAreaId = item.sp.SolutionAreaId;
                        solutionPlayDto.SolutionAreaName = item.SolutionAreaName;
                        solutionPlayDto.SolutionPlayLabel = item.sp.SolutionPlayLabel;
                        solutionPlayDto.SolutionPlayName = item.sp.SolutionPlayName;
                        solutionPlayDto.SolutionPlayId = item.sp.SolutionPlayId;
                        solutionPlayDto.SolutionPlayThemeSlug = item.sp.SolutionPlayThemeSlug;
                        solutionPlayDto.ImageMain = item.sp.Image_Main;
                        solutionPlayDto.ImageThumb = item.sp.Image_Thumb;
                        solutionPlayDto.ImageMobile = item.sp.Image_Mobile;
                        solutionPlayDto.SolutionPlayDesc = item.sp.SolutionPlayDesc;
                        solutionPlayDto.SolutionStatusId = item.sp.SolutionStatusId;
                        solutionPlayDto.IsPublished = item.sp.IsPublished;
                        solutionPlayDtos.Add(solutionPlayDto);
                    }
                    solutionAreaPlayDTO.SolutionPlays = solutionPlayDtos;
                    solutionAreaPlayDTOs.Add(solutionAreaPlayDTO);
                }

                return solutionAreaPlayDTOs;

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Getting all the active solution area and its corresponding spot light.
        /// </summary>
        /// <returns></returns>
        public async Task<List<TechnologyShowcasePartnerSolutionDTO>> GetSolutionSpotlight(string solutionPlayId)
        {
            try
            {

                var showcasePartnerSolution = (from Ts in _context.TechnologyShowcasePartnerSolution
                                               join sp in _context.SolutionPlay
                                               on Ts.SolutionPlayId equals sp.SolutionPlayId
                                               where Ts.SolutionPlayId.Equals(new Guid(solutionPlayId))
                                               orderby Ts.PartnerName
                                               select new TechnologyShowcasePartnerSolutionDTO
                                               {
                                                   PartnerName = Ts.PartnerName,
                                                   overviewDescription = Ts.overviewDescription,
                                                   websiteLink = Ts.websiteLink,
                                                   logoFileLink = Ts.logoFileLink,
                                                   TechnologyShowcasePartnerSolutionId = Ts.TechnologyShowcasePartnerSolutionId,
                                                   PartnerNameSlug = Ts.PartnerNameSlug,
                                               }).ToList();
                return showcasePartnerSolution;

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Getting all the active solution area and its corresponding spot light.
        /// </summary>
        /// <returns></returns>
        public async Task<TechnologyShowcasePartnerSolutionViewDTO> GetTechnologySpotlightSolutionByViewId(string showcaseSolutionId)
        {
            try
            {

                var showcasePartnerSolution = (from Ts in _context.TechnologyShowcasePartnerSolution
                                               join sp in _context.SolutionPlay
                                               on Ts.SolutionPlayId equals sp.SolutionPlayId
                                               join sa in _context.SolutionAreas
                                               on sp.SolutionAreaId equals sa.SolutionAreaId
                                               where Ts.TechnologyShowcasePartnerSolutionId.Equals(new Guid(showcaseSolutionId))
                                               orderby Ts.PartnerName
                                               select new TechnologyShowcasePartnerSolutionViewDTO
                                               {
                                                   PartnerName = Ts.PartnerName,
                                                   overviewDescription = Ts.overviewDescription,
                                                   websiteLink = Ts.websiteLink,
                                                   MarketPlaceLink = Ts.MarketPlaceLink,
                                                   logoFileLink = Ts.logoFileLink,
                                                   TechnologyShowcasePartnerSolutionId = Ts.TechnologyShowcasePartnerSolutionId,
                                                   PartnerNameSlug = Ts.PartnerNameSlug,
                                                   SolutionAreaId = sa.SolutionAreaId,
                                                   SolutionPlayName = sp.SolutionPlayName,
                                                   SolutionAreaName = sa.SolutionAreaName,
                                                   SolutionAreaSlug = sa.SolutionAreaSlug,
                                                   SolutionPlaySlug = sp.SolutionPlayThemeSlug,
                                                   SolutionPlayId = sp.SolutionPlayId
                                               }).FirstOrDefault();
                return showcasePartnerSolution;

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Getting all the active solution area and its corresponding spot light.
        /// </summary>
        /// <returns></returns>

        public GetIndustryShowcasePartnerSolutionDTO GetTechnologySpotlightByViewId(string industrySlug, string subIndustrySlug, string partnerNameSlug)
        {
            var showcasePartnerSolution = (from ps in _context.IndustryShowcasePartnerSolution
                                           join it in _context.IndustryTheme
                                           on ps.IndustryThemeId equals it.IndustryThemeId
                                           join ind in _context.Industries
                                           on it.IndustryId equals ind.IndustryId
                                           join subInd in _context.SubIndustries
                                           on it.SubIndustryId equals subInd.SubIndustryId
                                           where ps.PartnerNameSlug == partnerNameSlug
                                           where ind.IndustrySlug == industrySlug
                                           where subInd.SubIndustrySlug == subIndustrySlug
                                           select new GetIndustryShowcasePartnerSolutionDTO
                                           {
                                               PartnerName = ps.PartnerName,
                                               overviewDescription = ps.overviewDescription,
                                               websiteLink = ps.websiteLink,
                                               logoFileLink = ps.logoFileLink,
                                               IndustryId = it.IndustryId,
                                               SubIndustryId = it.SubIndustryId,
                                               MarketPlaceLink = ps.MarketPlaceLink,
                                               IndustryShowcasePartnerSolutionId = ps.IndustryShowcasePartnerSolutionId,
                                               IndustrySlug = ind.IndustrySlug,
                                               SubIndustrySlug = subInd.SubIndustrySlug,
                                               PartnerNameSlug = ps.PartnerNameSlug,
                                               IndustryThemeSlug = it.IndustryThemeSlug
                                           }).FirstOrDefault();
            
            return showcasePartnerSolution;
        }
        /// <summary>
        /// Getting solution area and play for the given solution area id.
        /// </summary>
        /// <param name="solutionAreaId"></param>
        /// <returns></returns>
        public async Task<SolutionAreaPlayDTO> GetSolutionAreaWithPlays(string solutionAreaId)
        {
            try
            {

                var solutionAreasWithPlay = await (from sa in _context.SolutionAreas
                                                   join sp in _context.SolutionPlay on sa.SolutionAreaId equals sp.SolutionAreaId
                                                   join ss in _context.SolutionStatusType on sp.SolutionStatusId equals ss.SolutionStatusId
                                                   where ss.SolutionStatus.Equals("Approved") && sa.Status.Equals("Active") && sp.IsPublished.Equals(1)
                                                   && sa.SolutionAreaId.Equals(new Guid(solutionAreaId))
                                                   orderby sp.SolutionPlayName
                                                   select new
                                                   {
                                                       sa.SolutionAreaId,
                                                       sa.Image_Main,
                                                       sa.Image_Mobile,
                                                       sa.Image_Thumb,
                                                       sa.SolAreaDescription,
                                                       sa.SolutionAreaName,
                                                       sa.SolutionAreaSlug,
                                                       sa.DisplayOrder,
                                                       sp
                                                   }).ToListAsync();
                //.FirstOrDefaultAsync();

                
                var solutionAreaPlayDto = new SolutionAreaPlayDTO();
                var solutionPlayDtos = new List<SolutionPlayDTO>();
                foreach (var solutionArea in solutionAreasWithPlay)
                {
                    var solutionPlayDto = new SolutionPlayDTO();
                    solutionAreaPlayDto.SolutionAreaName = solutionArea.SolutionAreaName;
                    solutionAreaPlayDto.SolutionAreaId = solutionArea.SolutionAreaId;
                    solutionAreaPlayDto.SolutionAreaSlug = solutionArea.SolutionAreaSlug;
                    solutionAreaPlayDto.SolutionAreaDescription = solutionArea.SolAreaDescription;
                    solutionAreaPlayDto.ImageMain = solutionArea.Image_Main;
                    solutionAreaPlayDto.ImageThumb = solutionArea.Image_Thumb;
                    solutionAreaPlayDto.ImageMobile = solutionArea.Image_Mobile;

                    solutionPlayDto.SolutionPlayId = solutionArea.sp.SolutionPlayId;
                    solutionPlayDto.SolutionPlayLabel = solutionArea.sp.SolutionPlayLabel;
                    solutionPlayDto.SolutionPlayName = solutionArea.sp.SolutionPlayName;
                    solutionPlayDto.ImageMain = solutionArea.sp.Image_Main;
                    solutionPlayDto.ImageThumb = solutionArea.sp.Image_Thumb;
                    solutionPlayDto.ImageMobile = solutionArea.sp.Image_Mobile;
                    solutionPlayDto.SolutionPlayThemeSlug = solutionArea.sp.SolutionPlayThemeSlug;
                    solutionPlayDto.IsPublished = solutionArea.sp.IsPublished;
                    solutionPlayDto.SolutionAreaId = solutionArea.sp.SolutionAreaId;
                    solutionPlayDto.SolutionPlayDesc = solutionArea.sp.SolutionPlayDesc;
                    solutionPlayDtos.Add(solutionPlayDto);
                }
               solutionAreaPlayDto.SolutionPlays = solutionPlayDtos;

                return solutionAreaPlayDto;

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Getting the play for the selected Areaslug (Custom uniq column to query the solution area data).
        /// </summary>
        /// <param name="solutionAreaSlug"></param>
        /// <returns></returns>
        public async Task<List<SolutionPlayViewDTO>> GetSolutionPlays(string solutionAreaSlug)
        {
            try
            {

                var solutionPlays = await (from sp in _context.SolutionPlay.Distinct()
                                           join sa in _context.SolutionAreas on sp.SolutionAreaId equals sa.SolutionAreaId
                                           join ss in _context.SolutionStatusType on sp.SolutionStatusId equals ss.SolutionStatusId
                                           join pspp in _context.PartnerSolutionPlayByPlay on sp.SolutionPlayId equals pspp.SolutionPlayId into solutioPlayPlay
                                           from spp in solutioPlayPlay.DefaultIfEmpty()
                                           join psp in _context.PartnerSolutionPlay on spp.PartnerSolutionPlayId equals psp.PartnerSolutionPlayId
                                           where ss.SolutionStatus.Equals("Approved") && sp.IsPublished.Equals(1)
                                           && psp.IsPublished.Equals(1) && sa.SolutionAreaSlug.Equals(solutionAreaSlug)
                                           select new SolutionPlayViewDTO
                                           {
                                               SolutionPlayId = sp.SolutionPlayId,
                                               SolutionAreaId = sp.SolutionAreaId,
                                               SolutionPlayLabel = sp.SolutionPlayLabel,
                                               SolutionPlayThemeSlug = sp.SolutionPlayThemeSlug,
                                               SolutionPlayName = sp.SolutionPlayName,
                                               ImageThumb = sp.Image_Thumb,
                                               SolutionAreaName = sa.SolutionAreaName,
                                               SolutionAreaSlug = sa.SolutionAreaSlug
                                           }).Distinct().ToListAsync();

                return solutionPlays;

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Get all the solution profiles with solutionplayid.
        /// </summary>
        /// <param name="solutionPlayId"></param>
        /// <returns></returns>
        public async Task<List<PartnerSolutionPlayViewDTO>> GetSolutionProfiles(string solutionPlayId)
        {
            var pSolution = await (from partnerSoln in _context.PartnerSolutionPlay
                                   join solArea in _context.SolutionAreas
                                   on partnerSoln.SolutionAreaId equals solArea.SolutionAreaId
                                   join playByPlay in _context.PartnerSolutionPlayByPlay
                                   on partnerSoln.PartnerSolutionPlayId equals playByPlay.PartnerSolutionPlayId
                                   join solutionPlay in _context.SolutionPlay
                                   on playByPlay.SolutionPlayId equals solutionPlay.SolutionPlayId
                                   join ss in _context.SolutionStatusType
                                   on partnerSoln.SolutionStatusId equals ss.SolutionStatusId
                                   join org in _context.Organization
                                   on partnerSoln.OrgId equals org.OrgId
                                   where solutionPlay.SolutionPlayId.Equals(new Guid(solutionPlayId)) &&
                                   ss.SolutionStatus.Equals("Approved") && partnerSoln.IsPublished.Equals(1)
                                   select new PartnerSolutionPlayViewDTO
                                   {
                                       PartnerSolutionPlayId = partnerSoln.PartnerSolutionPlayId,
                                       SolutionAreaId = partnerSoln.SolutionAreaId,
                                       OrgId = partnerSoln.OrgId,
                                       OrgName = org.OrgName,
                                       SolutionPlayName = partnerSoln.SolutionPlayName,
                                       SolutionPlayOrgWebsite = partnerSoln.SolutionPlayOrgWebsite,
                                       MarketplaceLink = partnerSoln.MarketplaceLink,
                                       SpecialOfferLink = partnerSoln.SpecialOfferLink,
                                       SolutionStatusId = partnerSoln.SolutionStatusId,
                                       IsPublished = partnerSoln.IsPublished,
                                       RowChangedBy = partnerSoln.RowChangedBy,
                                       Image_Thumb = partnerSoln.Image_Thumb,
                                       Image_Main = partnerSoln.Image_Main,
                                       Image_Mobile = partnerSoln.Image_Mobile,
                                       PartnerSolutionPlaySlug = partnerSoln.PartnerSolutionPlaySlug,
                                       SolutionPlayTitle = solutionPlay.SolutionPlayLabel,
                                       SolutionPlayDescription = partnerSoln.SolutionPlayDescription,
                                       LogoFileLink = partnerSoln.LogoFileLink
                                   }).ToListAsync();
            return pSolution;
        }
    }
}
