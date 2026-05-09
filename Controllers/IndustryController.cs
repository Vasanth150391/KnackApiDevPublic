using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Knack.DBEntities;
using Knack.API.Data;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using Knack.API.Models;
using Knack.Entities.DBEntities;
using System.Net.Mail;
using System.Resources;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Routing.Constraints;
using static System.Net.WebRequestMethods;
using System.Linq;
using Azure;
using Microsoft.Extensions.Configuration;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using Knack.API.Common;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.IdentityModel.Tokens;
using Knack.API.Interfaces;
using Microsoft.AspNetCore.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace Knack.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class IndustryController : ControllerBase
    {

        private readonly KnackContext _context;
        private readonly IConfiguration configuration;
        private readonly IPartnerSolutionBuilder _industryBuilder;
        private readonly string _basefilePath;
        public IndustryController(KnackContext context, IConfiguration iConfig, IPartnerSolutionBuilder industryBuilder)
        {
            _context = context;
            configuration = iConfig;
            _industryBuilder = industryBuilder;
            _basefilePath = Directory.GetCurrentDirectory() + "\\Mail_Templates";
        }

        /// <summary>
        /// ///////////////////// master


        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<IndustryDTO>> Get()
        {
            var ind = await _context.Industries.OrderBy(i => i.IndustryName).ToListAsync();

            List<IndustryDTO> iDTOs = new List<IndustryDTO>();

            foreach (Industry i in ind)
            {
                List<IndustryThemeDTO> iTDTO = new List<IndustryThemeDTO>();
                IndustryDTO iDTO = new IndustryDTO();

                iDTO.IndustryId = i.IndustryId;
                iDTO.IndustryName = i.IndustryName;
                var subInds = await _context.SubIndustries.Where(t => t.IndustryId == i.IndustryId).OrderBy(t=>t.SubIndustryName).ToListAsync();
                foreach (SubIndustry subInd in subInds)
                {                    
                    var iTheme = (from it in _context.IndustryTheme
                                  join ss in _context.SolutionStatusType
                                  on it.SolutionStatusId equals ss.SolutionStatusId
                                  join si in _context.SubIndustries
                                  on it.SubIndustryId equals si.SubIndustryId
                                  where si.SubIndustryId == subInd.SubIndustryId
                                  select new IndustryThemeDTO
                                  {
                                      SubIndustryId = si.SubIndustryId,
                                      SubIndustryName = si.SubIndustryName,
                                      IndustryThemeId = it.IndustryThemeId,
                                      Theme = it.Theme,
                                      Status = ss.SolutionStatus,
                                      DisplayLabel = ss.DisplayLabel,
                                      IsPublished = it.IsPublished,
                                      IndustryThemeDesc = it.IndustryThemeDesc,
                                      IndustryId = si.IndustryId,
                                      IndustryName = i.IndustryName
                                  }).FirstOrDefault();
                    if(iTheme != null)
                    iTDTO.Add(iTheme);
                }
                iDTO.SubIndustries = iTDTO;
                iDTOs.Add(iDTO);
                /*using (var command = this._context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "select Top (1) SI.SubIndustryId, SI.IndustryId, SubIndustryName, SubIndustryDescription, industryThemeId, Theme, industryThemeDesc, ss.SolutionStatus, ss.DisplayLabel, isPublished, IT.rowChangedBy, IT.rowChangedDate from SubIndustry SI, IndustryTheme IT join SolutionStatus ss On IT.solutionStatusId = ss.SolutionStatusId where SI.SubIndustryId = IT.subIndustryId AND  SI.subIndustryId = @subIndustryId";
                    command.CommandType = CommandType.Text;
                    SqlParameter param = new SqlParameter("@subIndustryId", subInd.SubIndustryId);
                    command.Parameters.Add(param);
                    this._context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            IndustryThemeDTO iTheme = new IndustryThemeDTO();
                            iTheme.SubIndustryId = Guid.Parse(result["SubIndustryId"].ToString());
                            iTheme.SubIndustryName = result["SubIndustryName"].ToString();
                            iTheme.IndustryThemeId = Guid.Parse(result["industryThemeId"].ToString());
                            iTheme.Theme = result["Theme"].ToString();
                            iTheme.Status = result["SolutionStatus"].ToString();
                            iTheme.DisplayLabel = result["DisplayLabel"].ToString();
                            iTheme.IsPublished = result["isPublished"].ToString();
                            iTheme.IndustryThemeDesc = result["industryThemeDesc"].ToString();
                            iTheme.IndustryId = Guid.Parse(result["IndustryId"].ToString());
                            iTheme.IndustryName = i.IndustryName;
                            iTDTO.Add(iTheme);
                        }
                    }
                    iDTO.SubIndustries = iTDTO;
                }*/
            }

            return iDTOs;
        }
        [HttpGet]
        [Route("GetIndustryList")]
        public async Task<IEnumerable<IndustryDTO>> GetIndustryList()
        {
            var ind = await _context.Industries.OrderBy(i => i.IndustryName).ToListAsync();
            List<IndustryDTO> iDTOs = new List<IndustryDTO>();
            foreach (Industry i in ind)
            {
                List<IndustryThemeDTO> iTDTO = new List<IndustryThemeDTO>();
                IndustryDTO iDTO = new IndustryDTO();
                iDTO.IndustryId = i.IndustryId;
                iDTO.IndustryName = i.IndustryName;
                iTDTO = (from subInd in _context.SubIndustries
                         join it in _context.IndustryTheme
                         on subInd.SubIndustryId equals it.SubIndustryId
                         where subInd.IndustryId == i.IndustryId
                         where subInd.Status == "Approved"
                         orderby subInd.SubIndustryName
                         select new IndustryThemeDTO
                         {
                             SubIndustryId = subInd.SubIndustryId,
                             SubIndustryName = subInd.SubIndustryName,
                             IndustryId = subInd.IndustryId,
                             IndustryName = i.IndustryName
                         }).ToList();
                iDTO.SubIndustries = iTDTO;
                iDTOs.Add(iDTO);
            }
            return iDTOs;
        }
        [HttpGet]
        [Route("GetIndustryListOrg")]
        public async Task<IEnumerable<IndustryDTO>> GetIndustryListOrg(Guid orgId)
        {
            var ind = await _context.Industries.OrderBy(i => i.IndustryName).ToListAsync();

            List<IndustryDTO> iDTOs = new List<IndustryDTO>();

            foreach (Industry i in ind)
            {
                List<IndustryThemeDTO> iTDTO = new List<IndustryThemeDTO>();
                IndustryDTO iDTO = new IndustryDTO();

                iDTO.IndustryId = i.IndustryId;
                iDTO.IndustryName = i.IndustryName;
                var indThemeRes = (from subInd in _context.SubIndustries
                                   join indTheme in _context.IndustryTheme
                                   on subInd.SubIndustryId equals indTheme.SubIndustryId
                                   where subInd.IndustryId == i.IndustryId && subInd.Status == "Approved"
                                   orderby subInd.SubIndustryName
                                   select new IndustryThemeDTO
                                   {
                                       SubIndustryId = subInd.SubIndustryId,
                                       SubIndustryName = subInd.SubIndustryName,
                                       IndustryId = subInd.IndustryId,
                                       IndustryThemeId = indTheme.IndustryThemeId
                                   }).ToList();
                foreach (IndustryThemeDTO indTheme in indThemeRes)
                {
                    IndustryThemeDTO iTheme = new IndustryThemeDTO();
                    iTheme.SubIndustryId = indTheme.SubIndustryId;
                    iTheme.SubIndustryName = indTheme.SubIndustryName;
                    iTheme.IndustryId = indTheme.IndustryId;
                    iTheme.IndustryName = i.IndustryName;
                    iTheme.IndustryThemeId = indTheme.IndustryThemeId;
                    iTDTO.Add(iTheme);
                }
                iDTO.SubIndustries = iTDTO;
                iDTOs.Add(iDTO);
            }
            foreach (IndustryDTO inds in iDTOs)
            {
                inds.showInd = false;
                int i = 0;
                foreach (IndustryThemeDTO subInd in inds.SubIndustries)
                {
                    subInd.showSubInd = false;

                    var usecaseOrg = _context.UsecaseOrganization.Where(t => t.organizationId == orgId).Where(t => t.usecaseId == subInd.IndustryThemeId).FirstOrDefault();
                    if (usecaseOrg != null)
                    {
                        subInd.showSubInd = false;
                    }
                    else
                    {
                        subInd.showSubInd = true;
                        i++;
                    }

                }
                if (i > 0)
                {
                    inds.showInd = true;
                }
            }
            return iDTOs;
        }
        [HttpGet]
        [Route("GetIndustryDetails")]
        public async Task<IEnumerable<Industry>> GetIndustryDetails(Guid IndID, Guid SubIndId)
        {
            var ind = await _context.Industries.Where(t => t.IndustryId == IndID).OrderBy(i => i.IndustryName).ToListAsync();



            List<SubIndustry> usersInDb = _context.SubIndustries.FromSqlRaw
                   (
                       "select * from SubIndustry where subindustryid = @subindustryid order by SubIndustryName",
                       new SqlParameter("@subindustryid", SubIndId)
                   )
                   .ToList();


            return ind;
        }

        [HttpGet]
        [Route("GetPartnerSolutionList")]
        public async Task<IEnumerable<PartnerSolution>> GetPartnerSolutionList()
        {
            var pSolution = await _context.PartnerSolution.Take(2).OrderByDescending(i => i.RowChangedDate).ToListAsync();

            return pSolution;
        }
        /*
        [HttpGet]
        [Route("GetIndustryThemeList")]
        public async Task<IEnumerable<IndustryTheme>> GetIndustryThemeList()
        {
            var IndThemeList = await _context.IndustryTheme.Take(9).OrderByDescending(i => i.RowChangedDate).ToListAsync();
            return IndThemeList;
        }
        */
        [HttpGet]
        [Route("GetIndustryThemeList")]
        public async Task<IEnumerable<IndustryThemeDTO>> GetIndustryThemeList()
        {
            var ind = await _context.Industries.OrderBy(i => i.IndustryName).ToListAsync();

            List<IndustryThemeDTO> itDTOs = new List<IndustryThemeDTO>();

            foreach (Industry i in ind)
            {
                var subInd = await _context.SubIndustries.Where(t => t.IndustryId == i.IndustryId).OrderBy(i => i.SubIndustryName).ToListAsync();
                foreach (SubIndustry si in subInd)
                {
                    IndustryThemeDTO iTDTO = new IndustryThemeDTO();
                    var subIndustryList = (from indTheme in _context.SubIndustries
                                           join it in _context.IndustryTheme
                                           on indTheme.SubIndustryId equals it.SubIndustryId
                                           join ss in _context.SolutionStatusType
                                           on it.SolutionStatusId equals ss.SolutionStatusId
                                           where it.SubIndustryId.Equals(si.SubIndustryId)
                                           where ss.SolutionStatus.Equals("Approved")
                                           where it.IsPublished == "1"
                                           select new
                                           {
                                               indTheme.IndustryId,
                                               it.SubIndustryId,
                                               it.IndustryThemeId,
                                               it.IndustryThemeSlug,
                                               it.Theme,
                                               indTheme.SubIndustryName,
                                               it.Image_Thumb,
                                               indTheme.SubIndustrySlug
                                           }).ToList();
                    foreach (var indData in subIndustryList)
                    {
                        iTDTO.IndustryId = indData.IndustryId;
                        iTDTO.SubIndustryId = indData.SubIndustryId;
                        iTDTO.IndustryThemeId = indData.IndustryThemeId;
                        iTDTO.IndustryThemeSlug = indData.IndustryThemeSlug;
                        iTDTO.Theme = indData.Theme;
                        iTDTO.IndustryName = i.IndustryName;
                        iTDTO.SubIndustryName = indData.SubIndustryName;
                        iTDTO.Image_Thumb = indData.Image_Thumb;
                        iTDTO.IndustrySlug = i.IndustrySlug;
                        iTDTO.SubIndustrySlug = indData.SubIndustrySlug;
                        itDTOs.Add(iTDTO);
                    }
                    
                }
            }

            return itDTOs;
        }
        [HttpGet]
        [Route("GetIndustryThemeListByIndustryId")]
        public async Task<IndustryViewDTO> GetIndustryThemeListByIndustryId(string slug)
        {
            var ind = _context.Industries.Where(t => t.IndustrySlug == slug).OrderBy(i => i.IndustryName).FirstOrDefault();
            Guid industryId = ind.IndustryId;
            IndustryViewDTO ivDTO = new IndustryViewDTO();
            List<IndustryThemeDTO> itDTOs = new List<IndustryThemeDTO>();

            if (ind != null)
            {
                ivDTO.IndustryName = ind.IndustryName;
                ivDTO.IndustryId = ind.IndustryId;
                ivDTO.IndustryDescription = ind.IndustryDescription;
                ivDTO.ImageURL = ind.Image_main;
                ivDTO.ImageMobileURL = ind.Image_mobile;
                ivDTO.IndustrySlug = ind.IndustrySlug;

                var subInd = await _context.SubIndustries.Where(t => t.IndustryId == ind.IndustryId).OrderBy(i=>i.SubIndustryName).ToListAsync();
                foreach (SubIndustry si in subInd)
                {
                    IndustryThemeDTO iTDTO = new IndustryThemeDTO();
                    var subIndustryList = (from subind in _context.SubIndustries
                                           join it in _context.IndustryTheme
                                           on subind.SubIndustryId equals it.SubIndustryId
                                           join ss in _context.SolutionStatusType
                                           on it.SolutionStatusId equals ss.SolutionStatusId
                                           where it.SubIndustryId.Equals(si.SubIndustryId)
                                           where ss.SolutionStatus.Equals("Approved")
                                           where it.IsPublished == "1"
                                           select new
                                           {
                                               it.IndustryThemeId,
                                               it.IndustryThemeSlug,
                                               it.IndustryId,
                                               subind.SubIndustryId,
                                               subind.SubIndustrySlug,
                                               it.Theme,
                                               subind.SubIndustryName,
                                               it.Image_Thumb
                                           }).ToList();
                    foreach (var data in subIndustryList)
                    {
                        iTDTO.IndustryId = data.IndustryId;
                        iTDTO.SubIndustryId = data.SubIndustryId;
                        iTDTO.SubIndustrySlug = data.SubIndustrySlug;
                        if (Guid.TryParse(data.IndustryThemeId.ToString(), out var newGuid))
                        {
                            iTDTO.IndustryThemeId = newGuid;
                        }

                        iTDTO.Theme = data.Theme;
                        iTDTO.IndustryThemeSlug = data.IndustryThemeSlug;
                        iTDTO.IndustryName = ind.IndustryName;
                        iTDTO.SubIndustryName = data.SubIndustryName;
                        iTDTO.Image_Thumb = data.Image_Thumb;
                        itDTOs.Add(iTDTO);
                    }
                }
                ivDTO.SubIndustriesTheme = itDTOs;
            }

            return ivDTO;
        }
        [HttpGet]
        [Route("GetIndustryThemeData")]
        public async Task<IEnumerable<IndustryTheme>> GetIndustryThemeData(Guid ThemeID)
        {
            var IndThemeData = await _context.IndustryTheme.Where(t => t.IndustryThemeId == ThemeID).ToListAsync();
            return IndThemeData;
        }

        [HttpGet]
        [Route("GetIndustryThemeBySolutionArea")]
        public async Task<IEnumerable<IndustryThemeBySolutionArea>> GetIndustryThemeBySolutionArea(Guid ThemeID)
        {
            var result = await _context.IndustryThemeBySolutionArea.Where(t => t.IndustryThemeId == ThemeID).ToListAsync();
            return result;
        }

        [HttpGet]
        [Route("GetSolutionAreaName")]
        public async Task<IEnumerable<SolutionArea>> GetSolutionAreaName(Guid SolutionAreaID)
        {
            var result = await _context.SolutionAreas.Where(t => t.SolutionAreaId == SolutionAreaID).ToListAsync();
            return result;
        }

        [HttpGet]
        [Route("GetPartnerUserSolution")]
        public async Task<IEnumerable<PartnerSolution>> GetPartnerUserSolution(String PartnerUserEmail, Guid SubIndustryId)
        {
            List<PartnerSolution> partnerUser = _context.PartnerSolution.FromSqlRaw
                (
                    "Select ps.*, ss.SolutionStatus from organization as org join partnerUser as pu on org.orgId = pu.orgId join partnerSolution as ps on ps.partnerUserId = pu.partnerUserId join SolutionStatus as ss ON ss.SolutionStatusId = ps.SolutionStatusId where pu.partnerEmail = @partneruseremail and ps.SubIndustryId = @SubIndustryId",
                    new SqlParameter("@partneruseremail", PartnerUserEmail), new SqlParameter("@SubIndustryId", SubIndustryId)
                )
                .ToList();

            return partnerUser;
        }

        [HttpGet]
        [Route("GetPartnerUserSolutionByOrgId")]
        public async Task<IEnumerable<PartnerSolution>> GetPartnerUserSolutionByOrgIdAsync(Guid orgId, Guid SubIndustryId)
        {
            List<PartnerSolution> partnerUser = _context.PartnerSolution.FromSqlRaw
                (
                    "Select ps.*, ss.SolutionStatus from organization as org join partnerUser as pu on org.orgId = pu.orgId join partnerSolution as ps on ps.partnerUserId = pu.partnerUserId join SolutionStatus as ss ON ss.SolutionStatusId = ps.SolutionStatusId where org.orgid = @orgid and ps.SubIndustryId = @SubIndustryId",
                    new SqlParameter("@orgid", orgId), new SqlParameter("@SubIndustryId", SubIndustryId)
                )
                .ToList();

            return partnerUser;
        }

        [HttpGet]
        [Route("GetIndustryThemeSolution")]
        public async Task<IEnumerable<IndustryTheme>> GetIndustryThemeSolution(String PartnerUserEmail, Guid SubIndustryId)
        {
            List<IndustryTheme> industryTheme = _context.IndustryTheme.FromSqlRaw
                (
                    "Select it.* from partnerUser as pu join IndustryTheme as it on pu.partnerUserId = it.partnerId where pu.partnerEmail = @partneruseremail and it.subIndustryId = @SubIndustryId",
                    new SqlParameter("@partneruseremail", PartnerUserEmail), new SqlParameter("@SubIndustryId", SubIndustryId)
                )
                .ToList();

            return industryTheme;
        }


        [HttpGet]
        [Route("GetAllThemes")]
        public async Task<IEnumerable<IndustryThemeDTO>> GetAllThemes()
        {

            List<IndustryThemeDTO> itDTOs = new List<IndustryThemeDTO>();
            using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                //command.CommandText = "SELECT i.industryId, SI.subIndustryId, i.IndustryName, SI.SubIndustryName , IT.industryThemeId, Theme, IT.status FROM Industry I LEFT JOIN SubIndustry SI  ON I.IndustryId = SI.industryId LEFT JOIN  IndustryTheme IT ON SI.SubIndustryId = IT.subIndustryId";
                command.CommandText = "SELECT i.industryId, SI.subIndustryId, i.IndustryName, SI.SubIndustryName , IT.industryThemeId, Theme, IT.status, IT.isPublished FROM IndustryTheme IT LEFT JOIN SubIndustry SI  ON IT.SubIndustryId = SI.SubIndustryId LEFT JOIN Industry I ON I.IndustryId = IT.IndustryId";

                command.CommandType = CommandType.Text;


                this._context.Database.OpenConnection();


                IndustryThemeDTO iTDTO = new IndustryThemeDTO();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        iTDTO = new IndustryThemeDTO();

                        iTDTO.IndustryId = Guid.Parse(result["IndustryId"].ToString());
                        iTDTO.SubIndustryId = Guid.Parse(result["SubIndustryId"].ToString());

                        if (Guid.TryParse(result["IndustryThemeId"].ToString(), out var newGuid))
                        {
                            iTDTO.IndustryThemeId = newGuid;
                        }

                        iTDTO.Theme = result["Theme"].ToString();
                        iTDTO.IndustryName = result["IndustryName"].ToString();
                        iTDTO.SubIndustryName = result["SubIndustryName"].ToString();
                        iTDTO.Status = result["Status"].ToString();
                        iTDTO.IsPublished = result["isPublished"].ToString();


                        itDTOs.Add(iTDTO);
                    }
                }
            }



            return itDTOs;
        }

        [HttpGet]
        [Route("Geo")]
        public async Task<IEnumerable<Geo>> GetGeo()
        {
            var geo = await _context.Geos.OrderBy(i => i.DisplayOrder).ToListAsync();

            return geo;
        }

        /*[HttpGet]
        [Route("SolutionAreaForPartnerProfile")]
        public async Task<IEnumerable<SolutionArea>> GetSolutionAreasForPartnerProfile()
        {
            var sarea = await _context.SolutionAreas.Where(s => s.IsDisplayOnPartnerProfile).OrderBy(i => i.DisplayOrder).ToListAsync();

            return sarea;
        }*/
        [HttpGet]
        [Route("SolutionAreaForPartnerProfile")]
        public async Task<IEnumerable<SolutionAreaDTO>> GetSolutionAreasForPartnerProfile(Guid subIndustryId)
        {
            //var sarea = await _context.SolutionAreas.Where(s => s.IsDisplayOnPartnerProfile).OrderBy(i => i.DisplayOrder).ToListAsync();
            var solutionArea = (from theme in _context.IndustryTheme
                                join iTSA in _context.IndustryThemeBySolutionArea
                                on theme.IndustryThemeId equals iTSA.IndustryThemeId
                                join sa in _context.SolutionAreas
                                on iTSA.SolutionAreaId equals sa.SolutionAreaId
                                where theme.SubIndustryId == subIndustryId
                                select new SolutionAreaDTO
                                {
                                    SolutionAreaId = sa.SolutionAreaId,
                                    SolutionAreaName = sa.SolutionAreaName,

                                }).ToList();
            return solutionArea;
        }

        [HttpGet]
        [Route("SolutionArea")]
        public async Task<IEnumerable<SolutionArea>> GetSolutionAreas()
        {
            var sarea = await _context.SolutionAreas.OrderBy(i => i.DisplayOrder).ToListAsync();

            return sarea;
        }

        [HttpGet]
        [Route("ResourceLink")]
        public async Task<IEnumerable<ResourceLink>> GetResourceLinks()
        {
            var sarea = await _context.ResourceLinks.OrderBy(i => i.DisplayOrder).ToListAsync();

            return sarea;
        }

        [HttpGet]
        [Route("IndustryThemeByIndustry")]
        public async Task<IEnumerable<IndustryTheme>> GetIndustryThemeByIndustry(Guid IndID)
        {
            var sarea = await _context.IndustryTheme.Where(t => t.SubIndustryId == IndID).ToListAsync();

            return sarea;
        }

        // [HttpGet]
        // [Route("IndustryThemeByIndustry")]
        // public IndustryTheme GetIndustryThemeByIndustry(Guid IndID)
        // {
        //     IndustryTheme? it = new IndustryTheme();
        //      it =   _context.IndustryThemes.Where(t=> t.IndustryId == IndID).FirstOrDefault();

        //     return it;
        // }
        /// </summary>

        [HttpPost]
        [Route("OrganizationPartnerUser")]
        public async Task<Guid> PostOrganizationPartnerUser([FromBody] OrganizationPartnerUserDTO orgPartnerUserDTO)
        {
            try
            {
                var utilities = new Utilities(configuration);
                string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
                string encEmail = utilities.EncryptString(encKey, orgPartnerUserDTO.PartnerEmail);
                string encFirstName = utilities.EncryptString(encKey, orgPartnerUserDTO.FirstName);
                string encLastName = utilities.EncryptString(encKey, orgPartnerUserDTO.LastName);

                var newUserRecipientEmail = configuration.GetSection("EmailSettings").GetSection("NewUserRecipientEmail").Value;

                var email = _context.PartnerUser.Where(t => t.PartnerEmail == encEmail).FirstOrDefault();
                string domain = orgPartnerUserDTO.PartnerEmail.Split('@')[1];
                var response = new OrganizationWithPartnerUserDTO();
                var filePath = _basefilePath + "\\New_Partner_Register.html";
                var emailBody = System.IO.File.ReadAllText(filePath);
                emailBody = emailBody.Replace("#firstname#", orgPartnerUserDTO.FirstName);
                emailBody = emailBody.Replace("#lastname#", orgPartnerUserDTO.LastName);
                emailBody = emailBody.Replace("#email#", orgPartnerUserDTO.PartnerEmail);
                emailBody = emailBody.Replace("#orgname#", orgPartnerUserDTO.OrgName);

                if (domain.ToLower() == "microsoft.com")
                {
                    response.UserType = "Microsoft";
                }
                else if (domain.ToLower() == "knackcollective.com")
                {
                    response.UserType = "Knack";
                }
                else
                {
                    response.UserType = "Partner";
                }
                if (email == null)
                {
                    //if (response.UserType == "Partner" || response.UserType == "Microsoft")
                    //{
                    var orgname = orgPartnerUserDTO.OrgName.Trim().ToLower();
                    var orgs = _context.Organization.Where(t => t.OrgName.ToLower().Trim() == orgname).FirstOrDefault();
                    Guid orgId = Guid.Empty;
                    Boolean chkOrgId = false;
                    if (orgs != null)
                    {
                        chkOrgId = true;
                        orgId = (Guid)orgs.OrgId;
                    }
                    if (chkOrgId == false)
                    {
                        var org = new Organization
                        {
                            OrgId = Guid.NewGuid(),
                            OrgName = orgPartnerUserDTO.OrgName,
                            Status = "Created",
                            RowChangedDate = DateTime.UtcNow,
                            UserType = response.UserType,
                        };
                        _context.Organization.Add(org);
                        orgId = (Guid)org.OrgId;
                    }
                    var pUser = new PartnerUser
                    {
                        PartnerUserId = Guid.NewGuid(),
                        OrgId = orgId,
                        LastName = encLastName,
                        FirstName = encFirstName,
                        PartnerEmail = encEmail,
                        PartnerTitle = orgPartnerUserDTO.PartnerTitle,
                        UserType = response.UserType,
                        Status = "Created",
                        RowChangedDate = DateTime.UtcNow,
                    };
                    _context.PartnerUser.Add(pUser);
                    await _context.SaveChangesAsync();
                    var userInvitedata = _context.UserInvite.Where(t => t.UserInviteEmail == encEmail).FirstOrDefault();
                    if (userInvitedata != null)
                    {
                        userInvitedata.Status = "Registered";
                        _context.UserInvite.Update(userInvitedata);
                        _context.SaveChanges();
                    }

                    utilities.SendMail(newUserRecipientEmail, "New Partner Registration", emailBody);
                    return pUser.PartnerUserId.Value;
                    /*}
                    else
                    {
                        var pUser = new PartnerUser
                        {
                            PartnerUserId = Guid.NewGuid(),
                            OrgId = Guid.Empty,
                            LastName = orgPartnerUserDTO.LastName,
                            FirstName = orgPartnerUserDTO.FirstName,
                            PartnerEmail = encEmail,
                            PartnerTitle = orgPartnerUserDTO.PartnerTitle,
                            UserType = response.UserType,
                            Status = "Created",
                            RowChangedDate = DateTime.UtcNow,
                        };
                        _context.PartnerUser.Add(pUser);
                        await _context.SaveChangesAsync();
                        var userInvitedata = _context.UserInvite.Where(t => t.UserInviteEmail == encEmail).FirstOrDefault();
                        if (userInvitedata != null)
                        {
                            userInvitedata.Status = "Registered";
                            _context.UserInvite.Update(userInvitedata);
                            _context.SaveChanges();
                        }
                        return pUser.PartnerUserId.Value;
                    } */
                }
                else
                {
                    var orgname = orgPartnerUserDTO.OrgName.Trim().ToLower();
                    var orgs = _context.Organization.Where(t => t.OrgName.ToLower().Trim() == orgname).FirstOrDefault();
                    Guid orgId = Guid.Empty;
                    Boolean chkOrgId = false;
                    if (orgs != null)
                    {
                        chkOrgId = true;
                        orgId = (Guid)orgs.OrgId;
                    }
                    if (chkOrgId == false)
                    {
                        var org = new Organization
                        {
                            OrgId = Guid.NewGuid(),
                            OrgName = orgPartnerUserDTO.OrgName,
                            Status = "Created",
                            UserType = response.UserType,
                            RowChangedDate = DateTime.UtcNow,
                        };
                        _context.Organization.Add(org);
                        orgId = (Guid)org.OrgId;
                    }
                    email.OrgId = orgId;
                    email.LastName = encLastName;
                    email.FirstName = encFirstName;
                    email.PartnerTitle = orgPartnerUserDTO.PartnerTitle;
                    _context.PartnerUser.Update(email);
                    _context.SaveChanges();
                    //utilities.sendMail(newUserRecipientEmail, "Partner Details Updated", emailBody);
                    return email.PartnerUserId.Value;
                }

            }
            catch (Exception ex)
            {
                return default(Guid);
            }
        }
        [HttpPost]
        [Route("CheckOrganization")]
        public CheckOrganizationDTO PostCheckOrganization([FromBody] OrganizationDTO organizationDTO)
        {
            var organization = new CheckOrganizationDTO();
            var orgname = organizationDTO.OrgName.Trim().ToLower();
            var orgs = _context.Organization.Where(t => t.OrgName.ToLower().Trim() == orgname).FirstOrDefault();
            Boolean chkOrgId = false;
            if (orgs != null)
                organization.orgExists = true;
            else
                organization.orgExists = false;
            return organization;
        }
        [HttpPost]
        [Route("CreateOrganization")]
        public async Task<IEnumerable<Organization>> PostCreateOrganization([FromBody] OrganizationDTO organizationDTO)
        {
            var org = new Organization
            {
                OrgId = Guid.NewGuid(),
                OrgName = organizationDTO.OrgName,
                Status = "Created",
                RowChangedDate = DateTime.UtcNow,
            };
            _context.Organization.Add(org);
            await _context.SaveChangesAsync();
            return await _context.Organization.ToListAsync();
        }
        [HttpPost]
        [Route("organization")]
        public async Task<Guid> PostOrganization([FromBody] OrganizationDTO orgDTO)
        {
            try
            {
                var org = new Organization
                {
                    OrgId = Guid.NewGuid(),
                    OrgName = orgDTO.OrgName,
                    Status = "Created",
                    RowChangedDate = DateTime.UtcNow,
                };
                _context.Organization.Add(org);
                await _context.SaveChangesAsync();
                return org.OrgId;
            }
            catch (Exception ex)
            {
                return default(Guid);
            }
        }

        [HttpPost]
        [Route("partneruser")]
        public async Task<Guid> PostPartner([FromBody] PartnerUserDTO partnerUserDTO)
        {
            try
            {
                var pUser = new PartnerUser
                {
                    PartnerUserId = Guid.NewGuid(),
                    OrgId = partnerUserDTO.OrgId,
                    LastName = partnerUserDTO.LastName,
                    FirstName = partnerUserDTO.FirstName,
                    PartnerEmail = partnerUserDTO.PartnerEmail,
                    PartnerTitle = partnerUserDTO.PartnerTitle,
                    Status = "Created",
                    RowChangedDate = DateTime.UtcNow,
                };
                _context.PartnerUser.Add(pUser);
                await _context.SaveChangesAsync();
                return pUser.PartnerUserId.Value;
            }
            catch (Exception ex)
            {
                return default(Guid);
            }
        }

        [HttpPost]
        [Route("createpartnersolution")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostPartnerSolution([FromBody] PartnerSolutionDTO partnerSolutionDTO)
        {
            try
            {
                if (partnerSolutionDTO == null)
                {
                    return BadRequest("PartnerSolution body cannot be empty");
                }
                var partnerSolutionId = await _industryBuilder.CreatePartnerSolution(partnerSolutionDTO);
                return Ok(partnerSolutionId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("IndustryRegistration")]
        public async Task<Guid> PostIndustryRegistrationAsync([FromBody] IndustryRegistrationDTO industryRegistrationDTO)
        {
            try
            {
                //var pUser = _context.PartnerUser.Where(t => t.PartnerEmail == industryRegistrationDTO.User.PartnerEmail).FirstOrDefault();
                /*var pUser = new PartnerUser
                {
                    PartnerUserId = Guid.NewGuid(),
                    OrgId = Guid.NewGuid(),
                    PartnerName = industryRegistrationDTO.User.PartnerName,
                    PartnerEmail = industryRegistrationDTO.User.PartnerEmail,
                    PartnerTitle = industryRegistrationDTO.User.PartnerTitle,
                    Status = "Created",
                    RowChangedDate = DateTime.UtcNow,
                };
                _context.PartnerUser.Add(pUser);*/
                string subslug = Regex.Replace(industryRegistrationDTO.SubIndustry.SubIndustryName, @"<[^>]+>|&nbsp;", "").Trim();
                subslug = Regex.Replace(subslug, @"\s{2,}", " ");
                subslug = Regex.Replace(subslug, "[^0-9A-Za-z _-]", "");
                subslug = Regex.Replace(subslug, @" ", "-");
                subslug = subslug.ToLower();
                Boolean slugCheck = true;
                string slugTmp = subslug;
                while (slugCheck)
                {
                    var resCheck = _context.SubIndustries.Where(t => t.SubIndustrySlug == subslug).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                    {
                        Random rnd = new Random();
                        int j = rnd.Next(1, 1003);
                        subslug = slugTmp + "-" + j;
                    }

                }
                var subIndustry = new SubIndustry
                {
                    SubIndustryId = new Guid(),
                    IndustryId = industryRegistrationDTO.Theme.IndustryId,
                    SubIndustryDescription = industryRegistrationDTO.SubIndustry.SubIndustryDescription,
                    SubIndustryName = industryRegistrationDTO.SubIndustry.SubIndustryName,
                    RowChangedBy = industryRegistrationDTO.Theme.PartnerId,
                    RowChangedDate = DateTime.UtcNow,
                    Status = industryRegistrationDTO.Theme.Status,
                    SubIndustrySlug = subslug
                };
                _context.SubIndustries.Add(subIndustry);
                string slug = Regex.Replace(industryRegistrationDTO.Theme.Theme, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                slugCheck = true;
                slugTmp = slug;
                while (slugCheck)
                {
                    var resCheck = _context.IndustryTheme.Where(t => t.IndustryThemeSlug == slug).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                    {
                        Random rnd = new Random();
                        int j = rnd.Next(1, 1003);
                        slug = slugTmp + "-" + j;
                    }

                }
                var industryTheme = new IndustryTheme
                {
                    IndustryThemeId = Guid.NewGuid(),
                    PartnerId = industryRegistrationDTO.Theme.PartnerId,
                    IndustryId = industryRegistrationDTO.Theme.IndustryId,
                    SubIndustryId = subIndustry.SubIndustryId,
                    Theme = industryRegistrationDTO.Theme.Theme,
                    IndustryThemeDesc = industryRegistrationDTO.Theme.IndustryThemeDesc,
                    Image_Thumb = industryRegistrationDTO.Theme.Image_Thumb,
                    Image_Main = industryRegistrationDTO.Theme.Image_Main,
                    Image_Mobile = industryRegistrationDTO.Theme.Image_Mobile,
                    RowChangedDate = DateTime.UtcNow,
                    RowChangedBy = industryRegistrationDTO.Theme.PartnerId,
                    //Status = industryRegistrationDTO.Theme.Status,
                    SolutionStatusId = industryRegistrationDTO.Theme.SolutionStatusId,
                    IsPublished = "0",
                    IndustryThemeSlug = slug
                };
                _context.IndustryTheme.Add(industryTheme);

                industryRegistrationDTO.CustomerProspects?.ForEach(pros =>
                {
                    var industryTargetCustomerProspect = new IndustryTargetCustomerProspect
                    {
                        IndustryTargetCustomerProspectId = Guid.NewGuid(),
                        IndustryThemeId = industryTheme.IndustryThemeId,
                        TargetPersonaTitle = pros.TargetPersonaTitle,
                        Status = "Created",
                        RowChangedDate = DateTime.UtcNow,
                    };
                    _context.IndustryTargetCustomerProspect.Add(industryTargetCustomerProspect);
                });

                industryRegistrationDTO.PartnersSolutions?.ForEach(solution =>
                {
                    var orgDetails = _context.Organization.Where(t => t.OrgName == solution.PartnerName).FirstOrDefault();
                    string slug = Regex.Replace(solution.PartnerName, @"<[^>]+>|&nbsp;", "").Trim();
                    slug = Regex.Replace(slug, @"\s{2,}", " ");
                    slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                    slug = Regex.Replace(slug, @" ", "-");
                    slug = slug.ToLower();
                    var industryShowcasePartnerSolution = new IndustryShowcasePartnerSolution
                    {
                        IndustryShowcasePartnerSolutionId = Guid.NewGuid(),
                        IndustryThemeId = industryTheme.IndustryThemeId,
                        PartnerId = industryRegistrationDTO.Theme.PartnerId,
                        PartnerName = solution.PartnerName,
                        MarketPlaceLink = solution.MarketPlaceLink,
                        websiteLink = solution.websiteLink,
                        overviewDescription = solution.overviewDescription,
                        logoFileLink = solution.logoFileLink ?? orgDetails.logoFileLink,
                        Status = "Created",
                        RowChangedDate = DateTime.UtcNow,
                        PartnerNameSlug = slug
                    };
                    _context.IndustryShowcasePartnerSolution.Add(industryShowcasePartnerSolution);

                    /*var orgDetails = _context.Organization.Where(t => t.OrgId == solution.OrgId).FirstOrDefault();
                    if (orgDetails != null)
                    {
                        orgDetails.logoFileLink = solution.logoFileLink;
                        _context.Organization.Update(orgDetails);
                    }*/
                });

                industryRegistrationDTO.ThemeBySolutionAreaModels?.ForEach(area =>
                {
                    var industryThemeBySolutionArea = new IndustryThemeBySolutionArea
                    {
                        IndustryThemeBySolutionAreaId = Guid.NewGuid(),
                        IndustryThemeId = industryTheme.IndustryThemeId,
                        SolutionAreaId = area.SolutionAreaId,
                        SolutionDesc = area.SolutionDesc,
                        Status = "Created",
                        RowChangedDate = DateTime.UtcNow,
                    };
                    _context.IndustryThemeBySolutionArea.Add(industryThemeBySolutionArea);
                });

                industryRegistrationDTO.ResourceLinks?.ForEach(link =>
                {
                    var industryResourceLink = new IndustryResourceLink
                    {
                        IndustryResourceLinkId = Guid.NewGuid(),
                        IndustryThemeId = industryTheme.IndustryThemeId,
                        ResourceLink = link.ResourceLink,
                        ResourceLinkId = link.ResourceLinkId,
                        Title = link.Title,
                        Status = "Created",
                        RowChangedDate = DateTime.UtcNow,
                    };
                    _context.IndustryResourceLink.Add(industryResourceLink);
                });


                if (industryRegistrationDTO.SelectedOrg?.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("usecaseOrgId");

                    foreach (var orgId in industryRegistrationDTO.SelectedOrg)
                    {
                        using (var command = this._context.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = "INSERT INTO usecaseOrganization (usecaseId,organizationId) VALUES (@usecaseId,@organizationId)";
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@usecaseId", industryTheme.IndustryThemeId));
                            command.Parameters.Add(new SqlParameter("@organizationId", orgId));
                            this._context.Database.OpenConnection();
                            int result = command.ExecuteNonQuery();
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return industryTheme.IndustryThemeId;
            }
            catch (Exception ex)
            {
                return default(Guid);
            }
        }

        [HttpPost]
        [Route("UpdateIndustry")]
        public async Task<Guid> PostUpdateIndustryAsync([FromBody] IndustryRegistrationDTO industryRegistrationDTO)
        {
            try
            {
                var subIndustry = _context.SubIndustries.Where(t => t.SubIndustryId == industryRegistrationDTO.SubIndustry.SubIndustryId).FirstOrDefault();
                string updatedSubIndName = industryRegistrationDTO.SubIndustry.SubIndustryName;

                string subslug = Regex.Replace(updatedSubIndName, @"<[^>]+>|&nbsp;", "").Trim();
                subslug = Regex.Replace(subslug, @"\s{2,}", " ");
                subslug = Regex.Replace(subslug, "[^0-9A-Za-z _-]", "");
                subslug = Regex.Replace(subslug, @" ", "-");
                subslug = subslug.ToLower();
                Boolean slugCheck = true;
                string slugTmp = subslug;
                while (slugCheck)
                {
                    var resCheck = _context.SubIndustries.Where(t => t.SubIndustrySlug == subslug).Where(t => t.SubIndustryId == industryRegistrationDTO.SubIndustry.SubIndustryId).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                    {
                        Random rnd = new Random();
                        int j = rnd.Next(1, 1003);
                        subslug = slugTmp + "-" + j;
                    }

                }
                if (!string.IsNullOrEmpty(updatedSubIndName))
                {
                    subIndustry.SubIndustryName = updatedSubIndName;
                    subIndustry.SubIndustryDescription = industryRegistrationDTO.SubIndustry.SubIndustryDescription;
                    subIndustry.Status = industryRegistrationDTO.Theme.Status;
                    subIndustry.SubIndustrySlug = subslug;
                    _context.SubIndustries.Update(subIndustry);

                }

                string slug = Regex.Replace(industryRegistrationDTO.Theme.Theme, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                slugCheck = true;
                slugTmp = slug;
                while (slugCheck)
                {
                    var resCheck = _context.IndustryTheme.Where(t => t.IndustryThemeSlug == slug).Where(t => t.IndustryThemeId == industryRegistrationDTO.Theme.IndustryThemeId).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                    {
                        Random rnd = new Random();
                        int j = rnd.Next(1, 1003);
                        slug = slugTmp + "-" + j;
                    }

                }
                var industrytheme = _context.IndustryTheme.Where(t => t.IndustryThemeId == industryRegistrationDTO.Theme.IndustryThemeId).FirstOrDefault();
                //industrytheme.PartnerId = industryRegistrationDTO.Theme.PartnerId;
                industrytheme.IndustryId = industryRegistrationDTO.Theme.IndustryId;
                industrytheme.SubIndustryId = industryRegistrationDTO.Theme.SubIndustryId;
                industrytheme.Theme = industryRegistrationDTO.Theme.Theme;
                industrytheme.IndustryThemeDesc = industryRegistrationDTO.Theme.IndustryThemeDesc;
                industrytheme.RowChangedDate = DateTime.UtcNow;
                industrytheme.RowChangedBy = industryRegistrationDTO.Theme.PartnerId;
                //industrytheme.Status = industryRegistrationDTO.Theme.Status;
                industrytheme.SolutionStatusId = industryRegistrationDTO.Theme.SolutionStatusId;
                industrytheme.Image_Thumb = industryRegistrationDTO.Theme.Image_Thumb;
                industrytheme.Image_Main = industryRegistrationDTO.Theme.Image_Main;
                industrytheme.Image_Mobile = industryRegistrationDTO.Theme.Image_Mobile;
                industrytheme.IsPublished = "0";
                industrytheme.IndustryThemeSlug = slug;


                _context.IndustryTheme.Update(industrytheme);
                await _context.SaveChangesAsync();
                var customerProspect = _context.IndustryTargetCustomerProspect.Where(t => t.IndustryThemeId == industrytheme.IndustryThemeId).ToList();
                if (customerProspect.Count > 0)
                {
                    foreach (var cs in customerProspect)
                    {
                        _context.Remove(cs);
                        _context.SaveChanges();
                    }
                }
                var showcaseSolution = _context.IndustryShowcasePartnerSolution.Where(t => t.IndustryThemeId == industrytheme.IndustryThemeId).ToList();
                if (showcaseSolution?.Count > 0)
                {
                    foreach (var scSolution in showcaseSolution)
                    {
                        _context.Remove(scSolution);
                        _context.SaveChanges();
                    }
                }
                var themeSolution = _context.IndustryThemeBySolutionArea.Where(t => t.IndustryThemeId == industrytheme.IndustryThemeId).ToList();
                if (themeSolution?.Count > 0)
                {
                    foreach (var tSolution in themeSolution)
                    {
                        _context.Remove(tSolution);
                        _context.SaveChanges();
                    }
                }
                var resourceLink = _context.IndustryResourceLink.Where(t => t.IndustryThemeId == industrytheme.IndustryThemeId).ToList();
                if (resourceLink?.Count > 0)
                {
                    foreach (var rLink in resourceLink)
                    {
                        _context.Remove(rLink);
                        _context.SaveChanges();
                    }
                }

                industryRegistrationDTO.CustomerProspects?.ForEach(pros =>
                {
                    var industryTargetCustomerProspect = new IndustryTargetCustomerProspect
                    {
                        IndustryTargetCustomerProspectId = Guid.NewGuid(),
                        IndustryThemeId = industryRegistrationDTO.Theme.IndustryThemeId,
                        TargetPersonaTitle = pros.TargetPersonaTitle,
                        Status = "Created",
                        RowChangedDate = DateTime.UtcNow,
                    };
                    _context.IndustryTargetCustomerProspect.Add(industryTargetCustomerProspect);
                });

                industryRegistrationDTO.PartnersSolutions?.ForEach(solution =>
                {
                    var orgDetails = _context.Organization.Where(t => t.OrgName == solution.PartnerName).FirstOrDefault();
                    string slug = Regex.Replace(solution.PartnerName, @"<[^>]+>|&nbsp;", "").Trim();
                    slug = Regex.Replace(slug, @"\s{2,}", " ");
                    slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                    slug = Regex.Replace(slug, @" ", "-");
                    slug = slug.ToLower();

                    var industryShowcasePartnerSolution = new IndustryShowcasePartnerSolution
                    {
                        IndustryShowcasePartnerSolutionId = Guid.NewGuid(),
                        IndustryThemeId = industryRegistrationDTO.Theme.IndustryThemeId,
                        PartnerId = industryRegistrationDTO.Theme.PartnerId,
                        PartnerName = solution.PartnerName,
                        MarketPlaceLink = solution.MarketPlaceLink,
                        websiteLink = solution.websiteLink,
                        overviewDescription = solution.overviewDescription,
                        logoFileLink = solution.logoFileLink ?? orgDetails.logoFileLink,
                        Status = "Created",
                        RowChangedDate = DateTime.UtcNow,
                        PartnerNameSlug = slug,
                    };
                    _context.IndustryShowcasePartnerSolution.Add(industryShowcasePartnerSolution);
                    _context.SaveChanges();
                    /*var orgDetails = _context.Organization.Where(t => t.OrgId == solution.OrgId).FirstOrDefault();
                    if (orgDetails != null)
                    {
                        orgDetails.logoFileLink = solution.logoFileLink;
                        _context.Organization.Update(orgDetails);
                        _context.SaveChanges();
                    }*/
                });

                industryRegistrationDTO.ThemeBySolutionAreaModels?.ForEach(area =>
                {
                    var industryThemeBySolutionArea = new IndustryThemeBySolutionArea
                    {
                        IndustryThemeBySolutionAreaId = Guid.NewGuid(),
                        IndustryThemeId = industryRegistrationDTO.Theme.IndustryThemeId,
                        SolutionAreaId = area.SolutionAreaId,
                        SolutionDesc = area.SolutionDesc,
                        Status = "Created",
                        RowChangedDate = DateTime.UtcNow,
                    };
                    _context.IndustryThemeBySolutionArea.Add(industryThemeBySolutionArea);
                });

                industryRegistrationDTO.ResourceLinks?.ForEach(link =>
                {
                    var industryResourceLink = new IndustryResourceLink
                    {
                        IndustryResourceLinkId = Guid.NewGuid(),
                        IndustryThemeId = industryRegistrationDTO.Theme.IndustryThemeId,
                        ResourceLink = link.ResourceLink,
                        ResourceLinkId = link.ResourceLinkId,
                        Title = link.Title,
                        Status = "Created",
                        RowChangedDate = DateTime.UtcNow,
                    };
                    _context.IndustryResourceLink.Add(industryResourceLink);
                });

                using (var command = this._context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "DELETE FROM usecaseOrganization where usecaseId = @usecaseId";
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@usecaseId", industrytheme.IndustryThemeId));
                    this._context.Database.OpenConnection();
                    int result = command.ExecuteNonQuery();
                }

                if (industryRegistrationDTO.SelectedOrg?.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("usecaseOrgId");

                    foreach (var orgId in industryRegistrationDTO.SelectedOrg)
                    {
                        using (var command = this._context.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = "INSERT INTO usecaseOrganization (usecaseId,organizationId) VALUES (@usecaseId,@organizationId)";
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@usecaseId", industrytheme.IndustryThemeId));
                            command.Parameters.Add(new SqlParameter("@organizationId", orgId));
                            this._context.Database.OpenConnection();
                            int result = command.ExecuteNonQuery();
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return industrytheme.IndustryThemeId;
            }
            catch (Exception ex)
            {
                return default(Guid);
            }
        }
        [HttpPost]
        [Route("DeleteIndustryTheme")]
        public async Task<Guid> PostDeleteIndustryThemeAsync([FromBody] IndustryThemeIdDTO industryThemeIdDTO)
        {
            try
            {
                var industrytheme = _context.IndustryTheme.Where(t => t.IndustryThemeId == industryThemeIdDTO.IndustryThemeId).FirstOrDefault();
                if (industrytheme != null)
                {
                    var archiveIndustryTheme = new ArchiveIndustryTheme
                    {
                        IndustryThemeId = industrytheme.IndustryThemeId,
                        PartnerId = industrytheme.PartnerId,
                        IndustryId = industrytheme.IndustryId,
                        SubIndustryId = industrytheme.SubIndustryId,
                        Theme = industrytheme.Theme,
                        IndustryThemeDesc = industrytheme.IndustryThemeDesc,
                        Image_Thumb = industrytheme.Image_Thumb,
                        Image_Main = industrytheme.Image_Main,
                        Image_Mobile = industrytheme.Image_Mobile,
                        RowChangedDate = DateTime.UtcNow,
                        RowChangedBy = industrytheme.PartnerId,
                        SolutionStatusId = industrytheme.SolutionStatusId,
                        IsPublished = "0",
                        IndustryThemeSlug = industrytheme.IndustryThemeSlug
                    };
                    _context.ArchiveIndustryTheme.Add(archiveIndustryTheme);
                    await _context.SaveChangesAsync();
                    _context.Remove(industrytheme);
                    await _context.SaveChangesAsync();
                }
                var partnerSolutions = _context.PartnerSolution.Where(t => t.SubIndustryId == industrytheme.SubIndustryId).ToList();
                if(partnerSolutions.Count > 0)
                {
                    foreach (var soln in partnerSolutions)
                    {
                        soln.IsPublished = 5;
                        _context.PartnerSolution.Update(soln);
                        _context.SaveChanges();
                    }
                }                
                var subindustry = _context.SubIndustries.Where(t => t.SubIndustryId == industrytheme.SubIndustryId).FirstOrDefault();
                if (subindustry != null)
                {
                    var newSubInd = new ArchiveSubIndustry
                    {
                        SubIndustryId = subindustry.SubIndustryId,
                        IndustryId = subindustry.IndustryId,
                        SubIndustryName = subindustry.SubIndustryName,
                        SubIndustryDescription = subindustry.SubIndustryDescription,
                        Status = subindustry.Status,
                        RowChangedBy = subindustry.RowChangedBy,
                        RowChangedDate = DateTime.UtcNow
                    };
                    _context.ArchiveSubIndustries.Add(newSubInd);
                    await _context.SaveChangesAsync();
                    _context.Remove(subindustry);
                    await _context.SaveChangesAsync();
                }
                var customerProspect = _context.IndustryTargetCustomerProspect.Where(t => t.IndustryThemeId == industrytheme.IndustryThemeId).ToList();
                if (customerProspect.Count > 0)
                {
                    foreach (var cs in customerProspect)
                    {
                        _context.Remove(cs);
                        _context.SaveChanges();
                    }
                }
                var showcaseSolution = _context.IndustryShowcasePartnerSolution.Where(t => t.IndustryThemeId == industrytheme.IndustryThemeId).ToList();
                if (showcaseSolution?.Count > 0)
                {
                    foreach (var scSolution in showcaseSolution)
                    {
                        _context.Remove(scSolution);
                        _context.SaveChanges();
                    }
                }
                var themeSolution = _context.IndustryThemeBySolutionArea.Where(t => t.IndustryThemeId == industrytheme.IndustryThemeId).ToList();
                if (themeSolution?.Count > 0)
                {
                    foreach (var tSolution in themeSolution)
                    {
                        _context.Remove(tSolution);
                        _context.SaveChanges();
                    }
                }
                var resourceLink = _context.IndustryResourceLink.Where(t => t.IndustryThemeId == industrytheme.IndustryThemeId).ToList();
                if (resourceLink?.Count > 0)
                {
                    foreach (var rLink in resourceLink)
                    {
                        _context.Remove(rLink);
                        _context.SaveChanges();
                    }
                }
                await _context.SaveChangesAsync();
                return industrytheme.IndustryThemeId;
            }
            catch (Exception ex)
            {
                return default(Guid);
            }
        }
        [HttpGet]
        [Route("GetIndustryThemeById")]
        public IndustryRegistrationDTO GetIndustryThemeById(Guid ThemeId)
        {
            var ThemeData = new IndustryRegistrationDTO();

            var industrytheme = (from theme in _context.IndustryTheme
                                 where theme.IndustryThemeId == ThemeId
                                 select new IndustryThemeDTO
                                 {
                                     IndustryThemeId = theme.IndustryThemeId,
                                     IndustryId = theme.IndustryId,
                                     SubIndustryId = theme.SubIndustryId,
                                     PartnerId = theme.PartnerId,
                                     Theme = theme.Theme,
                                     Image_Thumb = theme.Image_Thumb,
                                     Image_Main = theme.Image_Main,
                                     Image_Mobile = theme.Image_Mobile,
                                     IndustryThemeDesc = theme.IndustryThemeDesc,
                                     SolutionStatusId = theme.SolutionStatusId
                                 }).FirstOrDefault();
            ThemeData.Theme = industrytheme;

            //sub industry
            var subIndustry = (from subind in _context.SubIndustries
                               where subind.SubIndustryId == industrytheme.SubIndustryId
                               select new SubIndustryRegistrationDTO
                               {
                                   SubIndustryName = subind.SubIndustryName,
                                   SubIndustryDescription = subind.SubIndustryDescription,
                                   SubIndustryId = subind.SubIndustryId

                               }).FirstOrDefault();
            ThemeData.SubIndustry = subIndustry;

            //sub industry

            var CustomerProspects = (from cs in _context.IndustryTargetCustomerProspect
                                     where cs.IndustryThemeId == ThemeId
                                     select new IndustryTargetCustomerProspectDTO
                                     {
                                         IndustryTargetCustomerProspectId = cs.IndustryTargetCustomerProspectId,
                                         IndustryThemeId = cs.IndustryThemeId,
                                         TargetPersonaTitle = cs.TargetPersonaTitle
                                     }).ToList();
            ThemeData.CustomerProspects = CustomerProspects;

            var PartnersSolutions = (from scSolution in _context.IndustryShowcasePartnerSolution
                                         //join org in _context.Organization
                                         //on scSolution.PartnerName equals org.OrgName
                                     where scSolution.IndustryThemeId == ThemeId
                                     select new IndustryShowcasePartnerSolutionDTO
                                     {
                                         IndustryShowcasePartnerSolutionId = scSolution.IndustryShowcasePartnerSolutionId,
                                         IndustryThemeId = scSolution.IndustryThemeId,
                                         PartnerId = scSolution.PartnerId,
                                         PartnerName = scSolution.PartnerName,
                                         MarketPlaceLink = scSolution.MarketPlaceLink,
                                         logoFileLink = scSolution.logoFileLink,
                                         websiteLink = scSolution.websiteLink,
                                         overviewDescription = scSolution.overviewDescription,
                                     }).ToList();
            ThemeData.PartnersSolutions = PartnersSolutions;

            var ThemeBySolutionAreaModels = (from themeSolution in _context.IndustryThemeBySolutionArea
                                             where themeSolution.IndustryThemeId == ThemeId
                                             select new IndustryThemeBySolutionAreaDTO
                                             {
                                                 IndustryThemeBySolutionAreaId = themeSolution.IndustryThemeBySolutionAreaId,
                                                 IndustryThemeId = themeSolution.IndustryThemeId,
                                                 SolutionAreaId = themeSolution.SolutionAreaId,
                                                 SolutionDesc = themeSolution.SolutionDesc,
                                             }).ToList();
            ThemeData.ThemeBySolutionAreaModels = ThemeBySolutionAreaModels;

            var ResourceLinks = (from rl in _context.IndustryResourceLink
                                 where rl.IndustryThemeId == ThemeId
                                 select new IndustryResourceLinkDTO
                                 {
                                     IndustryResourceLinkId = rl.IndustryResourceLinkId,
                                     IndustryThemeId = rl.IndustryThemeId,
                                     ResourceLinkId = rl.ResourceLinkId,
                                     Title = rl.Title,
                                     ResourceLink = rl.ResourceLink
                                 }).ToList();
            ThemeData.ResourceLinks = ResourceLinks;

            return ThemeData;
        }

        [HttpGet]
        [Route("CloneIndustryTheme")]
        public async Task<Guid> CloneIndustryTheme(Guid ThemeId)
        {
            try
            {
                var industrythemedata = _context.IndustryTheme.Where(t => t.IndustryThemeId == ThemeId).FirstOrDefault();
                var status = _context.SolutionStatusType.Where(t => t.SolutionStatus == "Draft/In progress").FirstOrDefault();
                var subIndustry = _context.SubIndustries.Where(t => t.SubIndustryId == industrythemedata.SubIndustryId).FirstOrDefault();
                var newSubInd = new SubIndustry
                {
                    SubIndustryId = Guid.NewGuid(),
                    IndustryId = subIndustry.IndustryId,
                    SubIndustryName = subIndustry.SubIndustryName,
                    SubIndustryDescription = subIndustry.SubIndustryDescription,
                    Status = subIndustry.Status,
                    RowChangedBy = subIndustry.RowChangedBy,
                    RowChangedDate = DateTime.UtcNow
                };
                _context.SubIndustries.Add(newSubInd);

                var industryTheme = new IndustryTheme
                {
                    IndustryThemeId = Guid.NewGuid(),
                    PartnerId = industrythemedata.PartnerId,
                    IndustryId = industrythemedata.IndustryId,
                    SubIndustryId = newSubInd.SubIndustryId,
                    Theme = industrythemedata.Theme,
                    Image_Thumb = industrythemedata.Image_Thumb,
                    Image_Mobile = industrythemedata.Image_Mobile,
                    Image_Main = industrythemedata.Image_Main,
                    IndustryThemeDesc = industrythemedata.IndustryThemeDesc,
                    RowChangedDate = DateTime.UtcNow,
                    //Status = "Draft/In progress",
                    SolutionStatusId = status.SolutionStatusId,
                    RowChangedBy = industrythemedata.PartnerId,
                };
                _context.IndustryTheme.Add(industryTheme);
                var customerProspect = _context.IndustryTargetCustomerProspect.Where(t => t.IndustryThemeId == ThemeId).ToList();
                if (customerProspect.Count > 0)
                {
                    foreach (var cs in customerProspect)
                    {
                        var industryTargetCustomerProspect = new IndustryTargetCustomerProspect
                        {
                            IndustryTargetCustomerProspectId = Guid.NewGuid(),
                            IndustryThemeId = industryTheme.IndustryThemeId,
                            TargetPersonaTitle = cs.TargetPersonaTitle,
                            Status = "Draft/In progress",
                            RowChangedDate = DateTime.UtcNow,
                        };
                        _context.IndustryTargetCustomerProspect.Add(industryTargetCustomerProspect);
                    }
                }
                var showcaseSolution = _context.IndustryShowcasePartnerSolution.Where(t => t.IndustryThemeId == ThemeId).ToList();
                if (showcaseSolution?.Count > 0)
                {
                    foreach (var scSolution in showcaseSolution)
                    {
                        var industryShowcasePartnerSolution = new IndustryShowcasePartnerSolution
                        {
                            IndustryShowcasePartnerSolutionId = Guid.NewGuid(),
                            IndustryThemeId = industryTheme.IndustryThemeId,
                            PartnerId = scSolution.PartnerId,
                            PartnerName = scSolution.PartnerName,
                            MarketPlaceLink = scSolution.MarketPlaceLink,
                            Status = "Draft/In progress",
                            RowChangedDate = DateTime.UtcNow,
                        };
                        _context.IndustryShowcasePartnerSolution.Add(industryShowcasePartnerSolution);
                    }
                }
                var themeSolution = _context.IndustryThemeBySolutionArea.Where(t => t.IndustryThemeId == ThemeId).ToList();
                if (themeSolution?.Count > 0)
                {
                    foreach (var tSolution in themeSolution)
                    {
                        var industryThemeBySolutionArea = new IndustryThemeBySolutionArea
                        {
                            IndustryThemeBySolutionAreaId = Guid.NewGuid(),
                            IndustryThemeId = industryTheme.IndustryThemeId,
                            SolutionAreaId = tSolution.SolutionAreaId,
                            SolutionDesc = tSolution.SolutionDesc,
                            Status = "Draft/In progress",
                            RowChangedDate = DateTime.UtcNow,
                        };
                        _context.IndustryThemeBySolutionArea.Add(industryThemeBySolutionArea);
                    }
                }
                var resourceLink = _context.IndustryResourceLink.Where(t => t.IndustryThemeId == ThemeId).ToList();
                if (resourceLink?.Count > 0)
                {
                    foreach (var rLink in resourceLink)
                    {
                        var industryResourceLink = new IndustryResourceLink
                        {
                            IndustryResourceLinkId = Guid.NewGuid(),
                            IndustryThemeId = industryTheme.IndustryThemeId,
                            ResourceLink = rLink.ResourceLink,
                            ResourceLinkId = rLink.ResourceLinkId,
                            Title = rLink.Title,
                            Status = "Created",
                            RowChangedDate = DateTime.UtcNow,
                        };
                        _context.IndustryResourceLink.Add(industryResourceLink);
                    }
                }

                await _context.SaveChangesAsync();
                return industryTheme.IndustryThemeId;
            }
            catch (Exception ex)
            {
                return default(Guid);
            }
        }

        [HttpGet]
        [Route("GetOrganizationWithPartnerUserByEmail")]
        public OrganizationWithPartnerUserDTO GetOrganizationWithPartnerUserByEmail(string emailAddress)
        {
            var utilities = new Utilities(configuration);
            string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
            string encEmail = utilities.EncryptString(encKey, emailAddress);
            System.Diagnostics.Debug.WriteLine(encEmail);
            var result = (from partner in _context.PartnerUser
                          where partner.PartnerEmail == encEmail
                          select new OrganizationWithPartnerUserDTO
                          {
                              PartnerUserId = partner.PartnerUserId.GetValueOrDefault(),
                              PartnerEmail = emailAddress,
                              LastName = partner.LastName,
                              FirstName = partner.FirstName,
                              UserType = partner.UserType,
                              OrgId = partner.OrgId.GetValueOrDefault()
                          }).FirstOrDefault();

            string domain = emailAddress.Split('@')[1];
            if (result == null)
            {
                var response = new OrganizationWithPartnerUserDTO();
                if (domain.ToLower() == "microsoft.com")
                {
                    response.UserType = "Microsoft";
                }
                else if (domain.ToLower() == "knackcollective.com")
                {
                    response.UserType = "Knack";
                }
                else
                {
                    response.UserType = "Partner";
                }
                response.UserExisting = false;
                return response;
            }
            result.UserExisting = true;
            if (result != null)
            {
                var firstName = utilities.DecryptString(encKey, result?.FirstName);
                var lastName = utilities.DecryptString(encKey, result?.LastName);
                result.FirstName = firstName;
                result.LastName = lastName;
                var Orgresult = (from org in _context.Organization
                                 where org.OrgId == result.OrgId
                                 select new OrganizationWithPartnerUserDTO
                                 {
                                     OrgName = org.OrgName,
                                     UserType = org.UserType
                                 }).FirstOrDefault();
                if (Orgresult == null)
                {
                    result.UserExisting = false;
                }
                else
                {
                    result.OrgName = Orgresult.OrgName;
                    result.UserType = Orgresult.UserType;
                }

            }

            return result;
        }

        [HttpGet]
        [Route("GetUserType")]
        public async Task<int> GetUserType()
        {

            return 1; //1 for souln 2 for industry
        }

        [HttpGet]
        [Route("GetDashBoard")]
        public PartnerUserDTO GetDashBoard(string emailAddress)
        {
            /*
             * Available Dashboards
             * Dashboard
             * Dashboard_UseCase
             * */
            var utilities = new Utilities(configuration);
            string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
            string encEmail = utilities.EncryptString(encKey, emailAddress);

            var pu = _context.PartnerUser.Where(t => t.PartnerEmail == encEmail).FirstOrDefault();

            if (pu != null)
            {
                var partnerUserDTO = new PartnerUserDTO
                {
                    OrgId = pu.OrgId.GetValueOrDefault(),
                    PartnerUserId = pu.PartnerUserId.GetValueOrDefault(),
                    PartnerEmail = emailAddress,
                    LastName = utilities.DecryptString(encKey, pu.LastName),
                    FirstName = utilities.DecryptString(encKey, pu.FirstName),
                    PartnerTitle = pu.PartnerTitle,
                    UserType = pu.UserType,
                };
                if (partnerUserDTO.UserType == "Microsoft")
                {
                    partnerUserDTO.Route_PostAuthentication = "Industry_Dashboard";
                }
                else
                {
                    partnerUserDTO.Route_PostAuthentication = "Dashboard";
                }

                return partnerUserDTO;
            }
            return default;
        }
        /*
        [HttpGet]
        [Route("GetPartnerSolutionDetails")]
        public GetPartnerSolutionDTO GetPartnerSolutionDetails(Guid partnerSolutionId)
        {
            var result = (from partnerSoln in _context.PartnerSolution
                          join solnArea in _context.PartnerSolutionByArea
                          on partnerSoln.PartnerSolutionId equals solnArea.PartnerSolutionId
                          where partnerSoln.PartnerSolutionId == partnerSolutionId
                          select new GetPartnerSolutionDTO
                          {
                              PartnerSolutionId = partnerSoln.PartnerSolutionId,
                              UserId = partnerSoln.UserId,
                              IndustryId = partnerSoln.IndustryId,
                              SubIndustryId = partnerSoln.SubIndustryId,
                              SolutionDescription = partnerSoln.SolutionDescription,
                              SolutionOrgWebsite = partnerSoln.SolutionOrgWebsite,
                              MarketplaceLink = partnerSoln.MarketplaceLink,
                              SpecialOfferLink = partnerSoln.SpecialOfferLink,
                              LogoFileLink = partnerSoln.LogoFileLink,
                              SolutionAreaID = solnArea.SolutionAreaId,
                              AreaSolutionDescription = solnArea.AreaSolutionDescription,
                              PartnerSolutionByAreaId = solnArea.PartnerSolutionByAreaId
                          }).FirstOrDefault();

            return result;
        }*/
        [HttpGet]
        [Route("GetPartnerSolutionAvailableGeo")]
        public async Task<IEnumerable<PartnerSolutionAvailableGeoDTO>> GetPartnerSolutionAvailableGeo(Guid partnerSolutionId)
        {
            var result = (from partnerGeo in _context.PartnerSolutionAvailableGeo
                          join geo in _context.Geos
                          on partnerGeo.GeoId equals geo.GeoId
                          where partnerGeo.PartnerSolutionId == partnerSolutionId
                          select new PartnerSolutionAvailableGeoDTO
                          {
                              GeoId = geo.GeoId,
                              Geoname = geo.Geoname
                          }).ToList();

            return result;
        }
        [HttpGet]
        [Route("GetPartnerSolutionResourceLink")]
        public async Task<IEnumerable<GetPartnerSolutionResourceLinkDTO>> GetPartnerSolutionResourceLink(Guid partnerSolutionAreaId)
        {
            var result = (from partnerResourceLink in _context.PartnerSolutionResourceLink
                          join resourceLinks in _context.ResourceLinks
                          on partnerResourceLink.ResourceLinkId equals resourceLinks.ResourceLinkId
                          where partnerResourceLink.PartnerSolutionByAreaId == partnerSolutionAreaId
                          select new GetPartnerSolutionResourceLinkDTO
                          {
                              ResourceLinkTitle = partnerResourceLink.ResourceLinkTitle,
                              ResourceLinkUrl = partnerResourceLink.ResourceLinkUrl,
                              //ResourceLinkOverview = partnerResourceLink.ResourceLinkOverview,
                              ResourceLinkName = resourceLinks.ResourceLinkName,
                              ResourceLinkDescription = resourceLinks.ResourceLinkDescription
                          }).ToList();

            return result;
        }
        [HttpGet]
        [Route("GetPartnerSolutionByIndustry")]
        public async Task<IEnumerable<GetPartnerSolutionDTO>> GetPartnerSolutionByIndustry(Guid subIndstryId)
        {
            var result = (from partnerSoln in _context.PartnerSolution
                          where partnerSoln.SubIndustryId == subIndstryId
                          select new GetPartnerSolutionDTO
                          {
                              PartnerSolutionId = partnerSoln.PartnerSolutionId,
                              IndustryId = partnerSoln.IndustryId,
                              SubIndustryId = partnerSoln.SubIndustryId,
                              SolutionDescription = partnerSoln.SolutionDescription,
                              LogoFileLink = partnerSoln.LogoFileLink,
                              SolutionName = partnerSoln.SolutionName
                          }).ToList();

            return result;
        }
        [HttpGet]
        [Route("GetPartnerSolutionByIndustryFilter")]
        public async Task<IEnumerable<GetPartnerSolutionFilterDTO>> GetPartnerSolutionByIndustryFilter(Guid subIndstryId)
        {
            List<GetPartnerSolutionFilterDTO> PSFs = new List<GetPartnerSolutionFilterDTO>();
            List<GetPartnerSolutionFilterDTO> PSFLists = new List<GetPartnerSolutionFilterDTO>();
            PSFs = (from ps in _context.PartnerSolution
                    join psa in _context.PartnerSolutionByArea
                    on ps.PartnerSolutionId equals psa.PartnerSolutionId
                    join sa in _context.SolutionAreas
                    on psa.SolutionAreaId equals sa.SolutionAreaId
                    join ss in _context.SolutionStatusType
                    on ps.SolutionStatusId equals ss.SolutionStatusId
                    where ss.SolutionStatus == "Approved"
                    where ps.IsPublished == 1
                    where ps.SubIndustryId == subIndstryId                    
                    group sa by new
                    {
                        sa.SolutionAreaId,
                        sa.SolutionAreaName
                    } into solnArea
                    orderby solnArea.Key.SolutionAreaName
                    select new GetPartnerSolutionFilterDTO
                    {
                        SolutionAreaId = solnArea.Key.SolutionAreaId,
                        SolutionAreaName = solnArea.Key.SolutionAreaName
                    }).ToList();
            foreach (GetPartnerSolutionFilterDTO ps in PSFs)
            {
                GetPartnerSolutionFilterDTO pSolution = new GetPartnerSolutionFilterDTO();
                pSolution.SolutionAreaId = ps.SolutionAreaId;
                pSolution.SolutionAreaName = ps.SolutionAreaName;

                var result = (from partnerSoln in _context.PartnerSolution
                              join partnerSolnArea in _context.PartnerSolutionByArea
                              on partnerSoln.PartnerSolutionId equals partnerSolnArea.PartnerSolutionId
                              join ss in _context.SolutionStatusType
                              on partnerSoln.SolutionStatusId equals ss.SolutionStatusId
                              join org in _context.Organization
                              on partnerSoln.OrganizationId equals org.OrgId
                              where ss.SolutionStatus == "Approved"
                              where partnerSoln.IsPublished == 1
                              where partnerSoln.SubIndustryId == subIndstryId
                              where partnerSolnArea.SolutionAreaId == ps.SolutionAreaId
                              select new GetPartnerSolutionDTO
                              {
                                  PartnerSolutionId = partnerSoln.PartnerSolutionId,
                                  IndustryId = partnerSoln.IndustryId,
                                  SubIndustryId = partnerSoln.SubIndustryId,
                                  SolutionDescription = partnerSoln.SolutionDescription,
                                  LogoFileLink = partnerSoln.LogoFileLink,
                                  SolutionName = partnerSoln.SolutionName,
                                  OrgName = org.OrgName,
                                  PartnerSolutionSlug = partnerSoln.PartnerSolutionSlug
                              }).ToList();
                pSolution.PartnerSolutions = result;
                PSFLists.Add(pSolution);
            }
            return PSFLists;
        }
        [HttpGet]
        [Route("GetPartnerSolutionById")]

        public PartnerSolutionSimplifiedDTO GetPartnerSolutionById(Guid PartnerSolutionId)
        {
            var pSolution = (from partnerSoln in _context.PartnerSolution
                             join org in _context.Organization
                             on partnerSoln.OrganizationId equals org.OrgId
                             where partnerSoln.PartnerSolutionId == PartnerSolutionId
                             select new PartnerSolutionSimplifiedDTO
                             {
                                 PartnerSolutionId = partnerSoln.PartnerSolutionId,
                                 IndustryId = partnerSoln.IndustryId,
                                 UserId = partnerSoln.UserId,
                                 OrganizationId = partnerSoln.OrganizationId,
                                 SubIndustryId = partnerSoln.SubIndustryId,
                                 SolutionName = partnerSoln.SolutionName,
                                 SolutionDescription = partnerSoln.SolutionDescription,
                                 SolutionOrgWebsite = partnerSoln.SolutionOrgWebsite,
                                 MarketplaceLink = partnerSoln.MarketplaceLink,
                                 SpecialOfferLink = partnerSoln.SpecialOfferLink,
                                 LogoFileLink = partnerSoln.LogoFileLink,
                                 SolutionStatusId = partnerSoln.SolutionStatusId,
                                 IsPublished = partnerSoln.IsPublished,
                                 RowChangedBy = partnerSoln.RowChangedBy,
                                 IndustryDesignation = partnerSoln.IndustryDesignation,
                             }).FirstOrDefault();

            pSolution.PartnerSolutionAvailableGeo = _context.PartnerSolutionAvailableGeo.FromSqlRaw
                   (
                       "select * from PartnerSolutionAvailableGeo where partnerSolutionId = @partnerSolutionId",
                       new SqlParameter("@partnerSolutionId", PartnerSolutionId)
                   ).ToList();

            var PartnerSolutionAreas = (from solnArea in _context.PartnerSolutionByArea
                                        where solnArea.PartnerSolutionId == PartnerSolutionId
                                        select new PartnerSolutionAreaDTO
                                        {
                                            partnersolutionByAreaId = solnArea.PartnerSolutionByAreaId,
                                            solutionAreaId = solnArea.SolutionAreaId,
                                            areaSolutionDescription = solnArea.AreaSolutionDescription
                                        }).ToList();



            var getDetailsAry = new List<PartnerSolutionAreaDTO>();
            for (int i = 0; i < PartnerSolutionAreas.Count(); i++)
            {

                var PartnerSolutionResourceLinks = (from resource in _context.PartnerSolutionResourceLink
                                                    where resource.PartnerSolutionByAreaId == PartnerSolutionAreas[i].partnersolutionByAreaId
                                                    select new PartnerSolutionResourceLinkDTO
                                                    {
                                                        resourceLinkId = resource.ResourceLinkId,
                                                        resourceLinkTitle = resource.ResourceLinkTitle,
                                                        resourceLinkUrl = resource.ResourceLinkUrl,
                                                        eventDateTime = resource.EventDateTime
                                                        //resourceLinkOverview = resource.ResourceLinkOverview,
                                                    }).ToList();
                PartnerSolutionAreas[i].PartnerSolutionResourceLinks = PartnerSolutionResourceLinks;
                getDetailsAry.Add(PartnerSolutionAreas[i]);
            }

            pSolution.PartnerSolutionAreas = getDetailsAry;

            var spotLight = _context.SpotLight.FirstOrDefault(s => s.PartnerSolutionId == PartnerSolutionId);
            if (spotLight != null)
            {
                pSolution.SpotLight = new SpotLightDTO
                {
                    OrganizationId = spotLight.OrganizationId.GetValueOrDefault(),
                    PartnerSolutionId = spotLight.PartnerSolutionId,
                    SpotlightId = spotLight.SpotlightId.GetValueOrDefault(),
                    SpotlightOverview = spotLight.SpotlightOverview,
                    UsecaseId = spotLight.UsecaseId,
                };
            }

            return pSolution;
        }

        [HttpPost]
        [Route("updatePartnersolution")]
        public async Task<IActionResult> PostupdatePartnersolution([FromBody] PartnerSolutionDTO partnerSolutionDTO)
        {
            try
            {
                if (partnerSolutionDTO == null)
                {
                    return BadRequest("PartnerSolution body cannot be empty");
                }

                var partnerSolutionId = await _industryBuilder.UpdatePartnerSolution(partnerSolutionDTO);
                return Ok(partnerSolutionId);

            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("deletePartnersolution")]
        public async Task<Guid> PostdeletePartnersolution([FromBody] PartnerSolutionIdDTO partnerSolutionIdDTO)
        {
            try
            {
                var solution = _context.PartnerSolution.Where(t => t.PartnerSolutionId == partnerSolutionIdDTO.PartnerSolutionId).FirstOrDefault();

                if (solution != null)
                {
                    _context.Remove(solution);
                    _context.SaveChanges();
                }

                var solutionAreas = _context.PartnerSolutionByArea.Where(t => t.PartnerSolutionId == partnerSolutionIdDTO.PartnerSolutionId).ToList();
                if (solutionAreas?.Count > 0)
                {
                    foreach (var area in solutionAreas)
                    {
                        var resourcess = _context.PartnerSolutionResourceLink.Where(t => t.PartnerSolutionByAreaId == area.PartnerSolutionByAreaId).ToList();
                        if (resourcess?.Count > 0)
                        {
                            foreach (var res in resourcess)
                            {
                                _context.Remove(res);
                                _context.SaveChanges();

                            }
                        }
                        _context.Remove(area);
                        _context.SaveChanges();
                    }
                }
                var geos = _context.PartnerSolutionAvailableGeo.Where(t => t.PartnerSolutionId == partnerSolutionIdDTO.PartnerSolutionId).ToList();
                if (geos?.Count > 0)
                {
                    foreach (var geo in geos)
                    {
                        _context.Remove(geo);
                        _context.SaveChanges();
                    }
                }
                await _context.SaveChangesAsync();
                return solution.PartnerSolutionId;
            }
            catch (Exception ex)
            {
                return default(Guid);
            }
        }
        [HttpPost]
        [Route("updatePartnerSolutionStatus")]
        public async Task<Guid> PostupdatePartnerSolutionStatus([FromBody] PartnerSolutionStatusDTO partnerSolutionStatusDTO)
        {
            try
            {

                var solution = _context.PartnerSolution.Where(t => t.PartnerSolutionId == partnerSolutionStatusDTO.PartnerSolutionId).FirstOrDefault();
                solution.SolutionStatusId = partnerSolutionStatusDTO.SolutionStatusId;
                _context.PartnerSolution.Update(solution);
                await _context.SaveChangesAsync();
                return solution.PartnerSolutionId;
            }
            catch (Exception ex)
            {
                return default(Guid);
            }
        }
        [HttpPost]
        [Route("updatePartnerSolutionPublish")]
        public async Task PostupdatePartnerSolutionPublish([FromBody] PartnerSolutionPublishDTO partnerSolutionPublishDTO)
        {
            try
            {
                var utilities = new Utilities(configuration);
                var solution = _context.PartnerSolution.Where(t => t.PartnerSolutionId == partnerSolutionPublishDTO.PartnerSolutionId).FirstOrDefault();
                solution.IsPublished = partnerSolutionPublishDTO.IsPublished;
                var resCheck = _context.PartnerSolution.Where(t => t.PartnerSolutionSlug == solution.PartnerSolutionSlug).Where(t => t.PartnerSolutionId != partnerSolutionPublishDTO.PartnerSolutionId).ToList();
                if (solution.PartnerSolutionSlug == null || resCheck.Count > 0)
                {
                    Random rnd = new Random();
                    int j = rnd.Next(1, 100003);
                    var slug = solution.PartnerSolutionSlug + "-" + j;
                    solution.PartnerSolutionSlug = slug;
                }
                _context.PartnerSolution.Update(solution);
                await _context.SaveChangesAsync();
                
                var org = _context.PartnerUser.Where(t => t.PartnerUserId == solution.UserId).FirstOrDefault();
                string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
                var partnerEmailAddress = utilities.DecryptString(encKey, org.PartnerEmail);

                if(partnerSolutionPublishDTO.IsPublished == 1)
                {
                    var filePath = _basefilePath + "\\Industry_Solution_Publish.html";
                    var emailBody = System.IO.File.ReadAllText(filePath);
                    var link = "https://partner.microsoftindustryinsights.com/industrySolution";
                    var landingHomelink = "https://solutions.microsoftindustryinsights.com/dashboard";

                    if (Request.Host.Host.Contains("dev"))
                    {
                        link = "https://mssoldir-app-dev.azurewebsites.net/industrySolution";
                        landingHomelink = "https://mssoldir-app-ui-land-dev.azurewebsites.net/dashboard";
                    }
                    else if (Request.Host.Host.Contains("qa"))
                    {
                        link = "https://mssoldir-app-qa.azurewebsites.net/industrySolution";
                        landingHomelink = "https://mssoldir-app-ui-land-qa.azurewebsites.net/dashboard";

                    }

                    emailBody = emailBody.Replace("#link#", link);
                    emailBody = emailBody.Replace("#landinghomelink#", landingHomelink);
                    utilities.SendMail(partnerEmailAddress, "Your Microsoft Solutions Directory Partner Profile is published", emailBody);
                }
                var result = await _industryBuilder.PartnerSolutionPublish(partnerSolutionPublishDTO);
            }
            catch (Exception ex)
            {
            }
        }

        [HttpPost]
        [Route("PublishUsecase")]
        public async Task PublishUsecase([FromBody] IndustryRegistrationPublishedDTO industryRegistrationDTO)
        {
            var industrytheme = _context.IndustryTheme.Where(t => t.IndustryThemeId == industryRegistrationDTO.IndustryThemeId).FirstOrDefault();

            industrytheme.IsPublished = industryRegistrationDTO.IsPublished;
            _context.IndustryTheme.Update(industrytheme);
            await _context.SaveChangesAsync();
        }



        [HttpPost]
        [Route("SpotLight")]
        public async Task<Guid> PostSpotLight([FromBody] SpotLightDTO spotLightDTO)
        {
            try
            {
                var spotLight = new SpotLight
                {
                    SpotlightId = Guid.NewGuid(),
                    OrganizationId = spotLightDTO.OrganizationId,
                    UsecaseId = spotLightDTO.UsecaseId,
                    PartnerSolutionId = spotLightDTO.PartnerSolutionId,
                    SpotlightOverview = spotLightDTO.SpotlightOverview,
                    RowChangedDate = DateTime.UtcNow,
                };
                _context.SpotLight.Add(spotLight);
                await _context.SaveChangesAsync();
                return spotLight.SpotlightId.Value;
            }
            catch (Exception ex)
            {
                return default(Guid);
            }
        }
        [HttpGet]
        [Route("ClonePartnersolution")]
        public async Task<Guid> ClonePartnersolution(Guid partnerSolutionId)
        {
            try
            {
                var status = _context.SolutionStatusType.Where(t => t.SolutionStatus == "Draft/In progress").FirstOrDefault();
                var solution = _context.PartnerSolution.Where(t => t.PartnerSolutionId == partnerSolutionId).FirstOrDefault();
                if (solution != null)
                {
                    solution.PartnerSolutionId = Guid.NewGuid();
                    solution.SolutionName = "Copy of " + solution.SolutionName.ToString();
                    solution.SolutionStatusId = status.SolutionStatusId;
                    //solution.RowChangedBy = status.RowChangedBy;
                    solution.RowChangedDate = DateTime.UtcNow;
                }
                _context.PartnerSolution.Add(solution);
                var goes = _context.PartnerSolutionAvailableGeo.Where(t => t.PartnerSolutionId == partnerSolutionId).ToList();
                if (goes?.Count > 0)
                {
                    foreach (var geo in goes)
                    {
                        geo.PartnerSolutionAvailableGeoId = Guid.NewGuid();
                        geo.PartnerSolutionId = solution.PartnerSolutionId;
                        geo.Status = "Created";
                        geo.RowChangedDate = DateTime.UtcNow;
                        geo.GeoId = geo.GeoId;
                        _context.PartnerSolutionAvailableGeo.Add(geo);
                    }
                }
                var solutionAreas = _context.PartnerSolutionByArea.Where(t => t.PartnerSolutionId == partnerSolutionId).ToList();
                if (solutionAreas?.Count > 0)
                {
                    foreach (var area in solutionAreas)
                    {
                        var resources = _context.PartnerSolutionResourceLink.Where(t => t.PartnerSolutionByAreaId == area.PartnerSolutionByAreaId).ToList();
                        area.PartnerSolutionByAreaId = Guid.NewGuid();
                        area.PartnerSolutionId = solution.PartnerSolutionId;
                        area.RowChangedDate = DateTime.UtcNow;
                        _context.PartnerSolutionByArea.Add(area);

                        if (resources.Count > 0)
                        {
                            foreach (var res in resources)
                            {

                                res.PartnerSolutionResourceLinkId = Guid.NewGuid();
                                res.PartnerSolutionByAreaId = area.PartnerSolutionByAreaId;
                                res.RowChangedDate = DateTime.UtcNow;
                                _context.PartnerSolutionResourceLink.Add(res);

                            }
                        }
                    }
                }

                var spotLight = _context.SpotLight.FirstOrDefault(s => s.PartnerSolutionId == partnerSolutionId);
                if (spotLight != null)
                {
                    spotLight.SpotlightId = Guid.NewGuid();
                    spotLight.PartnerSolutionId = solution.PartnerSolutionId;
                    spotLight.RowChangedDate = DateTime.UtcNow;

                    _context.SpotLight.Add(spotLight);
                }

                await _context.SaveChangesAsync();
                return solution.PartnerSolutionId;
            }
            catch (Exception ex)
            {
                return default(Guid);
            }
        }


        [HttpGet]
        [Route("GetSolutionStatusTypes")]
        public async Task<List<SolutionStatusType>> GetSolutionStatusTypesAsync()
        {
            return await _context.SolutionStatusType.OrderBy(i=>i.DisplayLabel).ToListAsync();
        }

        [HttpGet]
        [Route("GetOrganizations")]
        public async Task<IEnumerable<Organization>> GetOrganizationsList()
        {
            var organizations = await _context.Organization.Where(i => i.UserType != "Knack").OrderBy(i => i.OrgName).ToListAsync();

            return organizations;
        }
        [HttpGet]
        [Route("GetUsecaseOrganization")]
        public async Task<IEnumerable<OrganizationDTO>> GetUsecaseOrganizationList(Guid usecaseId)
        {
            var result = (from org in _context.Organization
                          join usecase in _context.UsecaseOrganization
                          on org.OrgId equals usecase.organizationId
                          where usecase.usecaseId == usecaseId
                          orderby org.OrgName
                          select new OrganizationDTO
                          {
                              OrgId = org.OrgId,
                              OrgName = org.OrgName
                          }).ToList();
            return result;
        }

        [HttpGet]
        [Route("GetOrganizationsUsecase")]
        public async Task<IEnumerable<OrganizationDTO>> GetOrganizationsUsecase()
        {
            var organizations = await _context.Organization.Where(i => i.UserType != "Knack").OrderBy(i => i.OrgName).ToListAsync();
            var showcases = await _context.IndustryShowcasePartnerSolution.ToListAsync();
            List<OrganizationDTO> Orgs = new List<OrganizationDTO>();
            foreach (var org in organizations)
            {
                OrganizationDTO os = new OrganizationDTO();
                //var showCase = _context.IndustryShowcasePartnerSolution.Where(t => t.PartnerName == org.OrgName).FirstOrDefault();
                os.OrgName = org.OrgName;
                os.OrgId = org.OrgId;
                var i = 0;
                foreach (var showcase in showcases)
                {
                    if (showcase.PartnerName == org.OrgName)
                    {
                        os.logoFileLink = showcase.logoFileLink;
                        i++;
                    }
                }
                if (i == 0)
                    os.logoFileLink = org.logoFileLink;
                Orgs.Add(os);
            }
            return Orgs;
        }
        [HttpGet]
        [Route("GetOrganizationPartnerSolution")]
        public async Task<IEnumerable<OrganizationPartnerSolutionDTO>> GetOrganizationPartnerSolution(Guid OrgId)
        {
            var result = (from org in _context.Organization
                          join ps in _context.PartnerSolution
                          on org.OrgId equals ps.OrganizationId
                          join ss in _context.SolutionStatusType
                          on ps.SolutionStatusId equals ss.SolutionStatusId
                          where org.OrgId == OrgId
                          orderby ps.SolutionName
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
                              ParentSolutionId = ps.ParentSolutionId
                          }).ToList();
            return result;
        }
        [HttpGet]
        [Route("GetPartnerSolutionFilter")]
        public async Task<IEnumerable<OrganizationPartnerSolutionFilterDTO>> GetPartnerSolutionFilter(Guid subIndId, Guid statusID, Guid orgId)
        {
            List<OrganizationPartnerSolutionFilterDTO> PSFs = new List<OrganizationPartnerSolutionFilterDTO>();

            var result = (from org in _context.Organization
                          join ps in _context.PartnerSolution
                          on org.OrgId equals ps.OrganizationId
                          join ss in _context.SolutionStatusType
                          on ps.SolutionStatusId equals ss.SolutionStatusId
                          join si in _context.SubIndustries
                          on ps.SubIndustryId equals si.SubIndustryId
                          join ind in _context.Industries
                          on si.IndustryId equals ind.IndustryId                          
                          orderby ps.SolutionName
                          select new OrganizationPartnerSolutionFilterDTO
                          {
                              PartnerSolutionId = ps.PartnerSolutionId,
                              IndustryId = ps.IndustryId,
                              SubIndustryId = ps.SubIndustryId,
                              OrganizationId = ps.OrganizationId,
                              OrganizationName = org.OrgName,
                              IndustryName = ind.IndustryName,
                              SubIndustryName = si.SubIndustryName,
                              SolutionStatusId = ps.SolutionStatusId,
                              SolutionStatus = ss.SolutionStatus,
                              DisplayLabel = ss.DisplayLabel,
                              SolutionName = ps.SolutionName,
                              IsPublished = ps.IsPublished,
                              PartnerSolutionSlug = ps.PartnerSolutionSlug
                          }).ToList();
           
            if (statusID != Guid.Empty)
            {
                result = result.Where(i => i.SolutionStatusId == statusID).ToList();
            }
            if (subIndId != Guid.Empty)
            {
                result = result.Where(i => i.SubIndustryId == subIndId).ToList();
            }
            if (orgId != Guid.Empty)
            {
                result = result.Where(i => i.OrganizationId == orgId).ToList();
            }
            
            /*using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                var sql = "select PartnerSolutionId, ps.IndustryId,ps.partnerSolutionSlug, ps.SubIndustryId, OrganizationId, OrgName, IndustryName, SubIndustryName, ps.SolutionStatusId, ss.SolutionStatus, ss.DisplayLabel, ps.SolutionName, ps.IsPublished, ps.ParentsolutionId  from organization as o join partnerSolution as ps on o.orgId = ps.OrganizationId join SolutionStatus as ss on ps.SolutionStatusId = ss.SolutionStatusId join SubIndustry as si on ps.SubIndustryId = si.SubIndustryId join Industry as ind on si.IndustryId = ind.IndustryId where ";
                var sqlstring = 0;
                if (statusID != Guid.Empty)
                {
                    sql += " ps.SolutionStatusId = '" + statusID + "'";
                    sqlstring++;
                }
                if (subIndId != Guid.Empty)
                {
                    if (sqlstring > 0)
                    {
                        sql += " AND ";
                    }
                    sql += " ps.SubIndustryId = '" + subIndId + "'";
                    sqlstring++;
                }
                if (orgId != Guid.Empty)
                {
                    if (sqlstring > 0)
                    {
                        sql += " AND ";
                    }
                    sql += " ps.OrganizationId = '" + orgId + "'";
                }
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                this._context.Database.OpenConnection();

                using (var results = command.ExecuteReader())
                {
                    while (results.Read())
                    {
                        OrganizationPartnerSolutionFilterDTO pSolution = new OrganizationPartnerSolutionFilterDTO();
                        pSolution.PartnerSolutionId = Guid.Parse(results["PartnerSolutionId"].ToString());
                        pSolution.SubIndustryId = Guid.Parse(results["SubIndustryId"].ToString());
                        pSolution.IndustryId = Guid.Parse(results["IndustryId"].ToString());
                        pSolution.OrganizationId = Guid.Parse(results["OrganizationId"].ToString());
                        pSolution.OrganizationName = results["OrgName"].ToString();
                        pSolution.IndustryName = results["IndustryName"].ToString();
                        pSolution.SubIndustryName = results["SubIndustryName"].ToString();
                        pSolution.SolutionStatusId = Guid.Parse(results["SolutionStatusId"].ToString());
                        pSolution.SolutionStatus = results["SolutionStatus"].ToString();
                        pSolution.DisplayLabel = results["DisplayLabel"].ToString();
                        pSolution.SolutionName = results["SolutionName"].ToString();
                        pSolution.IsPublished = results["IsPublished"].ToString();
                        pSolution.PartnerSolutionSlug = results["partnerSolutionSlug"].ToString();
                        pSolution.ParentSolutionId = results["ParentsolutionId"].ToString(); 
                        PSFs.Add(pSolution);
                    }
                }
            }*/
            return result;            
        }
        [HttpPost]
        [Route("uploadFileToBlob")]
        public async Task<string> PostUploadFileToBlob(IFormCollection formCollection)
        {
            try
            {
                Dictionary<string, IFormFile> formFiles = new Dictionary<string, IFormFile>();
                foreach (var file in formCollection.Files)
                {
                    formFiles.Add(file.Name, file);
                }
                //var filename = await UploadBlobAsync(formFiles);
                //return filename;
                var blobConnectionString = configuration.GetSection("AzureBlobConnectionString").GetSection("BlobStorage").Value;

                // Create a BlobServiceClient object which will be used to create a container client
                var blobServiceClient = new BlobServiceClient(blobConnectionString);

                // Get a reference to a blob
                //var blobClient = containerClient.GetBlobClient("blobName");

                // Upload the blob
                // await blobClient.UploadAsync(stream, true);
                var filename = "";
                var filekey = Guid.NewGuid().ToString();
                foreach (var file in formFiles)
                {
                    // Get a reference to a container
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("dev");

                    // Get a reference to a blob

                    BlobClient blobClient = containerClient.GetBlobClient(filekey + Path.GetExtension(file.Value.FileName));
                    var stream = file.Value.OpenReadStream();
                    await blobClient.UploadAsync(stream, true);
                    stream.Close();
                    filename = "https://mssoldirdevstorage1.blob.core.windows.net/dev/" + filekey + Path.GetExtension(file.Value.FileName);
                }
                return filename;
            }
            catch (Exception ex)
            {
                //log.Error(ex.Message, ex);
                return ex.Message;
            }
        }
        [HttpGet]
        [Route("GetPartnerSolutionByViewId")]

        public PartnerSolutionViewDTO GetPartnerSolutionByViewId(string slug)
        {
            var pSolution = (from partnerSoln in _context.PartnerSolution
                             join org in _context.Organization
                             on partnerSoln.OrganizationId equals org.OrgId
                             join ind in _context.Industries
                             on partnerSoln.IndustryId equals ind.IndustryId
                             join subInd in _context.SubIndustries
                             on partnerSoln.SubIndustryId equals subInd.SubIndustryId
                             where partnerSoln.PartnerSolutionSlug == slug
                             select new PartnerSolutionViewDTO
                             {
                                 PartnerSolutionId = partnerSoln.PartnerSolutionId,
                                 IndustryId = partnerSoln.IndustryId,
                                 SubIndustryId = partnerSoln.SubIndustryId,
                                 SolutionName = partnerSoln.SolutionName,
                                 SolutionDescription = partnerSoln.SolutionDescription,
                                 SolutionOrgWebsite = partnerSoln.SolutionOrgWebsite,
                                 MarketplaceLink = partnerSoln.MarketplaceLink,
                                 SpecialOfferLink = partnerSoln.SpecialOfferLink,
                                 LogoFileLink = partnerSoln.LogoFileLink,
                                 OrgName = org.OrgName,
                                 OrganizationId = org.OrgId,
                                 IndustrySlug = ind.IndustrySlug,
                                 SubIndustrySlug = subInd.SubIndustrySlug

                             }).FirstOrDefault();

            pSolution.Geos = (from pag in _context.PartnerSolutionAvailableGeo
                              join geo in _context.Geos
                              on pag.GeoId equals geo.GeoId
                              where pag.PartnerSolutionId == pSolution.PartnerSolutionId
                              select new PartnerSolutionAvailableGeoDTO
                              {
                                  GeoId = pag.GeoId,
                                  Geoname = geo.Geoname
                              }).ToList();


            var PartnerSolutionAreas = (from psa in _context.PartnerSolutionByArea
                                        join sa in _context.SolutionAreas
                                        on psa.SolutionAreaId equals sa.SolutionAreaId
                                        where psa.PartnerSolutionId == pSolution.PartnerSolutionId
                                        select new PartnerSolutionAreaViewDTO
                                        {
                                            partnersolutionByAreaId = psa.PartnerSolutionByAreaId,
                                            solutionAreaId = psa.SolutionAreaId,
                                            areaSolutionDescription = psa.AreaSolutionDescription,
                                            solutionAreaName = sa.SolutionAreaName
                                        }).ToList();



            var getDetailsAry = new List<PartnerSolutionAreaViewDTO>();
            for (int i = 0; i < PartnerSolutionAreas.Count(); i++)
            {

                var PartnerSolutionResourceLinks = (from psl in _context.PartnerSolutionResourceLink
                                                    join rl in _context.ResourceLinks
                                                    on psl.ResourceLinkId equals rl.ResourceLinkId
                                                    where psl.PartnerSolutionByAreaId == PartnerSolutionAreas[i].partnersolutionByAreaId
                                                    select new PartnerSolutionResourceLinkViewDTO
                                                    {
                                                        resourceLinkId = psl.ResourceLinkId,
                                                        resourceLinkTitle = psl.ResourceLinkTitle,
                                                        resourceLinkUrl = psl.ResourceLinkUrl,
                                                        //resourceLinkOverview = psl.ResourceLinkOverview,
                                                        resourceLinkName = rl.ResourceLinkName,
                                                        eventDateTime = psl.EventDateTime

                                                    }).ToList();
                PartnerSolutionAreas[i].PartnerSolutionResourceLinks = PartnerSolutionResourceLinks;
                getDetailsAry.Add(PartnerSolutionAreas[i]);
            }

            pSolution.PartnerSolutionAreas = getDetailsAry;

            /*var spotLight = _context.SpotLight.FirstOrDefault(s => s.PartnerSolutionId == PartnerSolutionId);
            if (spotLight != null)
            {
                pSolution.SpotLight = new SpotLightDTO
                {
                    OrganizationId = spotLight.OrganizationId.GetValueOrDefault(),
                    PartnerSolutionId = spotLight.PartnerSolutionId,
                    SpotlightId = spotLight.SpotlightId.GetValueOrDefault(),
                    SpotlightOverview = spotLight.SpotlightOverview,
                    UsecaseId = spotLight.UsecaseId,
                };
            }*/

            pSolution.hasMultipleTheme = false;
            using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "select SI.SubIndustryId from SubIndustry SI, IndustryTheme IT join SolutionStatus ss On IT.solutionStatusId = ss.SolutionStatusId where SI.SubIndustryId = IT.subIndustryId AND ss.SolutionStatus = 'Approved' AND IT.IsPublished = 1 AND  IT.IndustryId = @IndustryId";
                command.CommandType = CommandType.Text;
                SqlParameter param = new SqlParameter("@IndustryId", pSolution.IndustryId);
                command.Parameters.Add(param);

                this._context.Database.OpenConnection();
                int j = 0;
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        if (j > 0)
                        {
                            pSolution.hasMultipleTheme = true;
                        }
                        j++;
                    }
                }
            }
            using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "select Top (1) industryThemeId, industryThemeSlug from SubIndustry SI, IndustryTheme IT join SolutionStatus ss On IT.solutionStatusId = ss.SolutionStatusId where SI.SubIndustryId = IT.subIndustryId AND ss.SolutionStatus = 'Approved' AND IT.IsPublished = 1 AND  SI.SubIndustryId = @SubIndustryId";
                command.CommandType = CommandType.Text;
                SqlParameter param = new SqlParameter("@SubIndustryId", pSolution.SubIndustryId);
                command.Parameters.Add(param);

                this._context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        pSolution.IndustryThemeId = Guid.Parse(result["industryThemeId"].ToString());
                        pSolution.IndustryThemeSlug = result["industryThemeSlug"].ToString();
                    }
                }
            }
            return pSolution;
        }
        [HttpGet]
        [Route("GetSpotlightSolutionByViewId")]

        public GetIndustryShowcasePartnerSolutionDTO GetSpotlightSolutionByViewId(string industrySlug, string subIndustrySlug, string partnerNameSlug)
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
            showcasePartnerSolution.hasMultipleTheme = false;
            using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "select SI.SubIndustryId from SubIndustry SI, IndustryTheme IT join SolutionStatus ss On IT.solutionStatusId = ss.SolutionStatusId where SI.SubIndustryId = IT.subIndustryId AND ss.SolutionStatus = 'Approved' AND IT.IsPublished = 1 AND  IT.IndustryId = @IndustryId";
                command.CommandType = CommandType.Text;
                SqlParameter param = new SqlParameter("@IndustryId", showcasePartnerSolution.IndustryId);
                command.Parameters.Add(param);

                this._context.Database.OpenConnection();
                int j = 0;
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        if (j > 0)
                        {
                            showcasePartnerSolution.hasMultipleTheme = true;
                        }
                        j++;
                    }
                }
            }
            using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "select Top (1) industryThemeId from SubIndustry SI, IndustryTheme IT join SolutionStatus ss On IT.solutionStatusId = ss.SolutionStatusId where SI.SubIndustryId = IT.subIndustryId AND ss.SolutionStatus = 'Approved' AND IT.IsPublished = 1 AND  SI.SubIndustryId = @SubIndustryId";
                command.CommandType = CommandType.Text;
                SqlParameter param = new SqlParameter("@SubIndustryId", showcasePartnerSolution.SubIndustryId);
                command.Parameters.Add(param);

                this._context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        showcasePartnerSolution.IndustryThemeId = Guid.Parse(result["industryThemeId"].ToString());
                    }
                }
            }
            return showcasePartnerSolution;
        }
        [HttpGet]
        [Route("GetThemeDetalsByViewId")]
        public IndustryThemeDetailsDTO GetThemeDetalsByViewId(string slug)
        {
            var itDetails = new IndustryThemeDetailsDTO();
            Guid ThemeID = Guid.Empty;
            var IndThemeData = _context.IndustryTheme.Where(t => t.IndustryThemeSlug == slug).FirstOrDefault();
            if (IndThemeData != null)
            {
                ThemeID = IndThemeData.IndustryThemeId;
                itDetails.IndustryId = IndThemeData.IndustryId;
                itDetails.SubIndustryId = IndThemeData.SubIndustryId;
                itDetails.IndustryThemeId = IndThemeData.IndustryThemeId;
                itDetails.PartnerId = IndThemeData.PartnerId;
                itDetails.Theme = IndThemeData.Theme;
                itDetails.IndustryThemeSlug = IndThemeData.IndustryThemeSlug;
                itDetails.IndustryThemeDesc = IndThemeData.IndustryThemeDesc;
                itDetails.Image_Main = IndThemeData.Image_Main;
                itDetails.Image_Mobile = IndThemeData.Image_Mobile;
            }
            
        var subIndustryList = (from ind in _context.Industries
                                       join subInd in _context.SubIndustries
                                       on ind.IndustryId equals subInd.IndustryId
                                       join indTheme in _context.IndustryTheme
                                       on subInd.SubIndustryId equals indTheme.SubIndustryId
                                       where indTheme.IndustryThemeId == ThemeID
                                       select new
                                       {
                                           ind.IndustryId,
                                           subInd.SubIndustryId,
                                           indTheme.IndustryThemeId,
                                           indTheme.Theme,
                                           ind.IndustryName,
                                           subInd.SubIndustryName,
                                           subInd.SubIndustryDescription,
                                           ind.IndustrySlug,
                                           subInd.SubIndustrySlug
                                       }).ToList();
                foreach (var indData in subIndustryList)
                {
                    itDetails.IndustryId = indData.IndustryId;
                    itDetails.SubIndustryId = indData.SubIndustryId;

                    if (Guid.TryParse(indData.IndustryThemeId.ToString(), out var newGuid))
                    {
                        itDetails.IndustryThemeId = newGuid;
                    }

                    itDetails.Theme = indData.Theme;
                    itDetails.IndustryName = indData.IndustryName;
                    itDetails.SubIndustryName = indData.SubIndustryName;
                    itDetails.SubIndustryDescription = indData.SubIndustryDescription;
                    itDetails.IndustrySlug = indData.IndustrySlug;
                    itDetails.SubIndustrySlug = indData.SubIndustrySlug;
                }
                itDetails.ThemeSolutionAreas = (from theme in _context.IndustryTheme
                                            join soln in _context.IndustryThemeBySolutionArea
                                            on theme.IndustryThemeId equals soln.IndustryThemeId
                                            join solutions in _context.SolutionAreas
                                            on soln.SolutionAreaId equals solutions.SolutionAreaId
                                            where theme.IndustryThemeId == ThemeID
                                            orderby solutions.SolutionAreaName
                                            select new ThemeSolutionAreaDTO
                                            {
                                                IndustryThemeBySolutionAreaId = soln.IndustryThemeBySolutionAreaId,
                                                SolutionAreaId = soln.SolutionAreaId,
                                                SolutionDesc = soln.SolutionDesc,
                                                SolutionAreaName = solutions.SolutionAreaName
                                            }).ToList();

            //var getDetailsAry = new List<GetPartnerSolutionDTO>();
            for (int i = 0; i < itDetails.ThemeSolutionAreas.Count(); i++)
            {
                System.Diagnostics.Debug.WriteLine(itDetails.ThemeSolutionAreas[i].SolutionAreaId);
                System.Diagnostics.Debug.WriteLine(IndThemeData.SubIndustryId);

                var PartnerSolution = (from ps in _context.PartnerSolution
                                       join psl in _context.PartnerSolutionByArea
                                       on ps.PartnerSolutionId equals psl.PartnerSolutionId
                                       join ss in _context.SolutionStatusType
                                       on ps.SolutionStatusId equals ss.SolutionStatusId
                                       join org in _context.Organization
                                       on ps.OrganizationId equals org.OrgId
                                       where ss.SolutionStatus == "Approved"
                                       where ps.IsPublished == 1
                                       where ps.SubIndustryId == IndThemeData.SubIndustryId
                                       where psl.SolutionAreaId == itDetails.ThemeSolutionAreas[i].SolutionAreaId
                                       select new GetPartnerSolutionDTO
                                       {
                                           PartnerSolutionId = ps.PartnerSolutionId,
                                           IndustryId = ps.IndustryId,
                                           SubIndustryId = ps.SubIndustryId,
                                           SolutionDescription = ps.SolutionDescription,
                                           SolutionName = ps.SolutionName,
                                           LogoFileLink = ps.LogoFileLink,
                                           OrgName = org.OrgName,
                                           PartnerSolutionSlug = ps.PartnerSolutionSlug
                                       }).ToList();
                itDetails.ThemeSolutionAreas[i].PartnerSolutions = PartnerSolution;
            }

            var showcasePartnerSolution = (from ps in _context.IndustryShowcasePartnerSolution
                                           join it in _context.IndustryTheme
                                           on ps.IndustryThemeId equals it.IndustryThemeId
                                           join ind in _context.Industries
                                           on it.IndustryId equals ind.IndustryId
                                           join subInd in _context.SubIndustries
                                           on it.SubIndustryId equals subInd.SubIndustryId
                                           where ps.IndustryThemeId == ThemeID
                                           orderby ps.PartnerName
                                           select new IndustryShowcasePartnerSolutionDTO
                                           {
                                               PartnerName = ps.PartnerName,
                                               overviewDescription = ps.overviewDescription,
                                               websiteLink = ps.websiteLink,
                                               logoFileLink = ps.logoFileLink,
                                               IndustryShowcasePartnerSolutionId = ps.IndustryShowcasePartnerSolutionId,
                                               IndustrySlug = ind.IndustrySlug,
                                               SubIndustrySlug = subInd.SubIndustrySlug,
                                               PartnerNameSlug = ps.PartnerNameSlug,
                                           }).ToList();
            itDetails.SpotLightPartnerSolutions = showcasePartnerSolution;
            
            itDetails.hasMultipleTheme = false;
            
            var themeDetails = (from it in _context.IndustryTheme
                                    join ss in _context.SolutionStatusType
                                    on it.SolutionStatusId equals ss.SolutionStatusId
                                    where ss.SolutionStatus.Equals("Approved")
                                    where it.IsPublished == "1"
                                    where it.IndustryId.Equals(IndThemeData.IndustryId)
                                    select new
                                    {
                                        it.IndustryThemeId
                                    }).ToList();
            int j = 0;
            foreach (var itList in themeDetails)
            {
                if (j > 0)
                {
                    itDetails.hasMultipleTheme = true;
                }
                j++;
            }
            return itDetails;
        }

        [HttpGet]
        [Route("GetMenu")]
        public async Task<IEnumerable<MenuIndustryDTO>> GetMenu()
        {
            var ind = await _context.Industries.OrderBy(i => i.IndustryName).ToListAsync();

            List<MenuIndustryDTO> iDTOs = new List<MenuIndustryDTO>();

            foreach (Industry i in ind)
            {
                List<MenuSubIndustryDTO> iTDTO = new List<MenuSubIndustryDTO>();
                MenuIndustryDTO iDTO = new MenuIndustryDTO();

                iDTO.IndustryId = i.IndustryId;
                iDTO.IndustryName = i.IndustryName;
                iDTO.IndustrySlug = i.IndustrySlug;
                iDTO.hasMultipleThme = false;
                iDTO.hasSubMenu = false;
                var subInd = await _context.SubIndustries.Where(t => t.IndustryId == iDTO.IndustryId).OrderBy(i => i.SubIndustryName).ToListAsync();
                var j = 0;
                foreach (SubIndustry si in subInd)
                {
                    MenuSubIndustryDTO iTheme = new MenuSubIndustryDTO();
                    iTheme.SubIndustryId = si.SubIndustryId;
                    iTheme.SubIndustryName = si.SubIndustryName;
                    iTheme.SubIndustrySlug = si.SubIndustrySlug;
                    List<MenuSolutionAreaDTO> MSADTO = new List<MenuSolutionAreaDTO>();
                    
                    MenuSolutionAreaDTO indTheme = new MenuSolutionAreaDTO();
                    
                    var subIndustryList = (from it in _context.IndustryTheme
                                           join ss in _context.SolutionStatusType
                                           on it.SolutionStatusId equals ss.SolutionStatusId
                                           where it.SubIndustryId.Equals(si.SubIndustryId)
                                           where ss.SolutionStatus.Equals("Approved")
                                           where it.IsPublished == "1"
                                           select new
                                           {
                                               it.IndustryThemeId,
                                               it.IndustryThemeSlug
                                           }).ToList();
                    foreach (var mIDTO in subIndustryList)
                    {
                        indTheme.IndustryThemeId = mIDTO.IndustryThemeId;
                        iTheme.IndustryThemeId = mIDTO.IndustryThemeId;
                        iDTO.IndustryThemeId = mIDTO.IndustryThemeId;
                        indTheme.IndustryThemeSlug = mIDTO.IndustryThemeSlug;
                        iTheme.IndustryThemeSlug = mIDTO.IndustryThemeSlug;
                        iDTO.IndustryThemeSlug = mIDTO.IndustryThemeSlug;
                        iDTO.hasSubMenu = true;
                        if (j > 0)
                        {
                            iDTO.hasMultipleThme = true;
                            iDTO.IndustryThemeId = Guid.Empty;
                        }
                        j++;
                    }

                    if (indTheme.IndustryThemeId != Guid.Empty)
                    {
                        var solutionDetail = (from it in _context.IndustryTheme
                                              join itsa in _context.IndustryThemeBySolutionArea
                                              on it.IndustryThemeId equals itsa.IndustryThemeId
                                              join sa in _context.SolutionAreas
                                              on itsa.SolutionAreaId equals sa.SolutionAreaId
                                              where it.IndustryThemeId == indTheme.IndustryThemeId
                                              orderby sa.SolutionAreaName
                                              select new MenuSolutionAreaDTO
                                              {
                                                  solutionAreaName = sa.SolutionAreaName,
                                                  IndustryThemeId = it.IndustryThemeId,
                                                  IndustryThemeSlug = it.IndustryThemeSlug,
                                                  SolutionAreaId = sa.SolutionAreaId,
                                              }).ToList();
                        iTheme.SolutionAreas = solutionDetail;
                    }

                    

                    iTDTO.Add(iTheme);
                }
                iDTO.SubIndustries = iTDTO;

                iDTOs.Add(iDTO);
            }
            return iDTOs;
        }
        [HttpGet]
        [Route("GetOrganizationById")]
        public async Task<OrganizationDTO> GetOrganizationById(Guid orgId)
        {
            var org = _context.Organization.Where(t => t.OrgId == orgId).FirstOrDefault();
            OrganizationDTO orgDetails = new OrganizationDTO();
            if (org != null)
            {
                orgDetails.OrgId = org.OrgId;
                orgDetails.OrgName = org.OrgName;
                orgDetails.logoFileLink = org.logoFileLink;
            }
            return orgDetails;
        }
        [HttpGet]
        [Route("updateThemeIndustrySlug")]
        public async Task<IEnumerable<IndustryTheme>> updateThemeIndustrySlug()
        {
            var result = _context.IndustryTheme.ToList();
            foreach (var res in result)
            {
                string slug = Regex.Replace(res.Theme, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                string slugTmp = slug;
                Boolean slugCheck = true;
                int i = 1;
                while (slugCheck)
                {
                    var resCheck = _context.IndustryTheme.Where(t => t.IndustryThemeSlug == slug).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                        slug = slugTmp + "-" + i;
                    i++;
                }
                res.IndustryThemeSlug = slug;
                _context.IndustryTheme.Update(res);
                await _context.SaveChangesAsync();
            }
            return result;
        }
        [HttpGet]
        [Route("updatePartnerSolutionSlug")]
        public async Task<IEnumerable<PartnerSolution>> updatePartnerSolutionSlug()
        {
            var result = _context.PartnerSolution.Where(t => t.PartnerSolutionSlug == null).ToList();
            foreach (var res in result)
            {
                string slug = Regex.Replace(res.SolutionName, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                string slugTmp = slug;
                Boolean slugCheck = true;
                int i = 1;
                while (slugCheck)
                {
                    var resCheck = _context.PartnerSolution.Where(t => t.PartnerSolutionSlug == slug).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                    {
                        Random rnd = new Random();
                        int j = rnd.Next(1, 1003);
                        slug = slugTmp + "-" + j;
                    }

                }
                res.PartnerSolutionSlug = slug;
                _context.PartnerSolution.Update(res);
                await _context.SaveChangesAsync();
            }
            return result;
        }
        [HttpGet]
        [Route("updateIndustrySlug")]
        public async Task<IEnumerable<Industry>> updateIndustrySlug()
        {
            var result = _context.Industries.ToList();
            foreach (var res in result)
            {
                string slug = Regex.Replace(res.IndustryName, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                string slugTmp = slug;
                Boolean slugCheck = true;
                int i = 1;
                while (slugCheck)
                {
                    var resCheck = _context.Industries.Where(t => t.IndustrySlug == slug).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                        slug = slugTmp + "-" + i;
                    i++;
                }
                res.IndustrySlug = slug;
                _context.Industries.Update(res);
                await _context.SaveChangesAsync();
            }
            return result;
        }
        [HttpGet]
        [Route("updateSubIndustrySlug")]
        public async Task<IEnumerable<SubIndustry>> updateSubIndustrySlug()
        {
            var result = _context.SubIndustries.ToList();
            foreach (var res in result)
            {
                string slug = Regex.Replace(res.SubIndustryName, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                string slugTmp = slug;
                Boolean slugCheck = true;
                int i = 1;
                while (slugCheck)
                {
                    var resCheck = _context.SubIndustries.Where(t => t.SubIndustrySlug == slug).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                        slug = slugTmp + "-" + i;
                    i++;
                }
                res.SubIndustrySlug = slug;
                _context.SubIndustries.Update(res);
                await _context.SaveChangesAsync();
            }
            return result;
        }
        [HttpGet]
        [Route("updateIndustryShowCaseSlug")]
        public async Task<IEnumerable<IndustryShowcasePartnerSolution>> updateIndustryShowCaseSlug()
        {
            var result = _context.IndustryShowcasePartnerSolution.ToList();
            foreach (var res in result)
            {
                string slug = Regex.Replace(res.PartnerName, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                res.PartnerNameSlug = slug;
                _context.IndustryShowcasePartnerSolution.Update(res);
                await _context.SaveChangesAsync();
            }
            return result;
        }
        [HttpGet]
        [Route("GetSolutionArea")]
        public async Task<IEnumerable<SolutionArea>> GetSolutionArea(Guid SolutionPlayId)
        {
            var result = await _context.SolutionAreas.ToListAsync();
            return result;
        }
        [HttpGet]
        [Route("GetSolutionAreaForSolutionPlay")]
        public async Task<IEnumerable<SolutionArea>> GetSolutionAreaForSolutionPlay(Guid SolutionPlayId)
        {
            List<SolutionArea> solAreas = new List<SolutionArea>();
            using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT  sa.* FROM  solutionArea as sa CROSS APPLY (SELECT  TOP 1 solutionPlayName FROM solutionPlay as sp join SolutionStatus as ss on sp.solutionStatusId = ss.SolutionStatusId WHERE sp.solutionAreaId = sa.solutionAreaId and ss.SolutionStatus = 'Approved') solPlay Order by sa.SolutionAreaName Asc";
                command.CommandType = CommandType.Text;
                //SqlParameter param = new SqlParameter("@solutionAreaId", i.SolutionAreaId);
                //command.Parameters.Add(param);

                this._context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        SolutionArea sa = new SolutionArea();
                        sa.SolutionAreaId = Guid.Parse(result["solutionAreaId"].ToString());
                        sa.SolutionAreaName = result["solutionAreaName"].ToString();
                        solAreas.Add(sa);
                    }
                }
            }
            //var result = await _context.SolutionAreas.ToListAsync();
            return solAreas;
        }
        [HttpGet]
        [Route("GetSolutionPlay")]
        public async Task<IEnumerable<SolutionAreaPlayDTO>> GetSolutionPlay()
        {
            var solnAreas = await _context.SolutionAreas.OrderBy(i => i.SolutionAreaName).ToListAsync();

            List<SolutionAreaPlayDTO> sPlays = new List<SolutionAreaPlayDTO>();

            foreach (SolutionArea i in solnAreas)
            {
                SolutionAreaPlayDTO sPlay = new SolutionAreaPlayDTO();
                //List<SolutionPlayDTO> solnPlays = new List<SolutionPlayDTO>();

                sPlay.SolutionAreaId = i.SolutionAreaId;
                sPlay.SolutionAreaName = i.SolutionAreaName;

                var solnPlays = (from sp in _context.SolutionPlay
                              join sa in _context.SolutionAreas
                              on sp.SolutionAreaId equals sa.SolutionAreaId
                              join ss in _context.SolutionStatusType
                              on sp.SolutionStatusId equals ss.SolutionStatusId
                              where sp.SolutionAreaId == i.SolutionAreaId
                              orderby sp.SolutionPlayLabel
                                 select new SolutionPlayDTO
                              {
                                  SolutionPlayId = sp.SolutionPlayId,
                                  SolutionPlayName = sp.SolutionPlayName,
                                  SolutionPlayLabel = sp.SolutionPlayLabel,
                                  SolutionAreaId = sp.SolutionAreaId,
                                  SolutionAreaName = sa.SolutionAreaName,
                                  SolutionStatusId = sp.SolutionStatusId,
                                  SolutionStatus = ss.SolutionStatus,
                                  DisplayLabel = ss.DisplayLabel,
                                  IsPublished = sp.IsPublished
                              }).ToList();
                sPlay.SolutionPlays = solnPlays;
                sPlays.Add(sPlay);
                /*using (var command = this._context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "select sp.solutionPlayId,sp.IsPublished, sp.solutionPlayName,sp.solutionPlayLabel, sp.solutionStatusId, sp.solutionAreaId, sa.solutionAreaName,ss.SolutionStatus,ss.DisplayLabel from solutionPlay as sp join solutionArea as sa on sp.solutionAreaId = sa.solutionAreaId join SolutionStatus as ss on sp.solutionStatusId = ss.SolutionStatusId where sp.solutionAreaId = @solutionAreaId";
                    command.CommandType = CommandType.Text;
                    SqlParameter param = new SqlParameter("@solutionAreaId", i.SolutionAreaId);
                    command.Parameters.Add(param);

                    this._context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            SolutionPlayDTO play = new SolutionPlayDTO();
                            play.SolutionPlayId = Guid.Parse(result["solutionPlayId"].ToString());
                            play.SolutionPlayName = result["solutionPlayName"].ToString();
                            play.SolutionPlayLabel = result["solutionPlayLabel"].ToString();
                            play.SolutionAreaId = Guid.Parse(result["solutionAreaId"].ToString());
                            play.SolutionAreaName = result["solutionAreaName"].ToString();
                            play.SolutionStatusId = Guid.Parse(result["solutionStatusId"].ToString());
                            play.SolutionStatus = result["SolutionStatus"].ToString();
                            play.DisplayLabel = result["DisplayLabel"].ToString();
                            play.IsPublished = (int)result["IsPublished"];
                            solnPlays.Add(play);
                        }
                    }
                    sPlay.SolutionPlays = solnPlays;
                }*/

            }

            return sPlays;
        }
        [HttpGet]
        [Route("GetSolutionPlayId")]
        public async Task<SolutionPlayDTO> GetSolutionPlayId(Guid SolutionPlayId)
        {
            //var results = _context.SolutionPlay.Where(u => u.SolutionPlayId == SolutionPlayId).FirstOrDefault();
            SolutionPlayDTO solutionPlay = new SolutionPlayDTO();
            using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT ps.solutionPlayId, ps.solutionAreaId, ps.solutionPlayName,ps.image_main,ps.image_mobile,ps.image_thumb,ps.IsPublished,ps.solutionPlayDesc,ps.solutionPlayLabel,ps.solutionStatusId, Count(DISTINCT psPlay.partnerSolutionPlayByPlayId) AS totalSolutionPlay FROM solutionPlay as ps LEFT JOIN partnerSolutionPlayByPlay psPlay ON ps.solutionPlayId = psPlay.solutionPlayId where ps.SolutionPlayId = @SolutionPlayId GROUP BY ps.solutionPlayId, ps.solutionAreaId, ps.solutionPlayName,ps.image_main,ps.image_mobile,ps.image_thumb,ps.IsPublished,ps.solutionPlayDesc,ps.solutionPlayLabel,ps.solutionStatusId";
                command.CommandType = CommandType.Text;
                SqlParameter param = new SqlParameter("@solutionPlayId", SolutionPlayId);
                command.Parameters.Add(param);
                this._context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {

                        solutionPlay.SolutionPlayId = Guid.Parse(result["solutionPlayId"].ToString());
                        solutionPlay.SolutionAreaId = Guid.Parse(result["solutionAreaId"].ToString());
                        solutionPlay.SolutionStatusId = Guid.Parse(result["solutionStatusId"].ToString());
                        solutionPlay.SolutionPlayName = result["solutionPlayName"].ToString();
                        solutionPlay.ImageMain = result["image_main"].ToString();
                        solutionPlay.ImageMobile = result["image_mobile"].ToString();
                        solutionPlay.ImageThumb = result["image_thumb"].ToString();
                        solutionPlay.IsPublished = (int)result["IsPublished"];
                        solutionPlay.totalSolutionPlay = (int)result["totalSolutionPlay"];
                        solutionPlay.SolutionPlayDesc = result["solutionPlayDesc"].ToString();
                        solutionPlay.SolutionPlayLabel = result["solutionPlayLabel"].ToString();
                    }
                }
            }

            var PartnersSolutions = (from scSolution in _context.TechnologyShowcasePartnerSolution
                                         //join org in _context.Organization
                                         //on scSolution.PartnerName equals org.OrgName
                                     where scSolution.SolutionPlayId == SolutionPlayId
                                     select new TechnologyShowcasePartnerSolutionDTO
                                     {
                                         TechnologyShowcasePartnerSolutionId = scSolution.TechnologyShowcasePartnerSolutionId,
                                         SolutionPlayId = scSolution.SolutionPlayId,
                                         PartnerId = scSolution.PartnerId,
                                         PartnerName = scSolution.PartnerName,
                                         MarketPlaceLink = scSolution.MarketPlaceLink,
                                         logoFileLink = scSolution.logoFileLink,
                                         websiteLink = scSolution.websiteLink,
                                         overviewDescription = scSolution.overviewDescription,
                                     }).ToList();
            solutionPlay.PartnersSolutions = PartnersSolutions;

            return solutionPlay;
        }

        [HttpPost]
        [Route("UpdateSolutionPlay")]
        public async Task<ResponseDTO> UpdateSolutionPlay([FromBody] SolutionPlayViewDTO updatedDetails)
        {
            ResponseDTO res = new ResponseDTO();
            var result = _context.SolutionPlay.Where(u => u.SolutionPlayId == updatedDetails.SolutionPlayId).FirstOrDefault();
            if (result != null)
            {
                string slug = Regex.Replace(updatedDetails.SolutionPlayLabel, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                string slugTmp = slug;
                Boolean slugCheck = true;
                int i = 1;
                while (slugCheck)
                {
                    var resCheck = _context.SolutionPlay.Where(t => t.SolutionPlayThemeSlug == slug).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                        slug = slugTmp + "-" + i;
                    i++;
                }

                result.SolutionPlayThemeSlug = slug;
                result.SolutionAreaId = updatedDetails.SolutionAreaId;
                result.SolutionPlayName = updatedDetails.SolutionPlayName;
                result.SolutionPlayDesc = updatedDetails.SolutionPlayDesc;
                result.SolutionStatusId = updatedDetails.SolutionStatusId;
                result.RowChangedDate = DateTime.UtcNow;
                result.RowChangedBy = updatedDetails.RowChangedBy;
                result.SolutionPlayLabel = updatedDetails.SolutionPlayLabel;
                result.Image_Thumb = updatedDetails.ImageThumb;
                result.Image_Main = updatedDetails.Image_Main;
                result.Image_Mobile = updatedDetails.Image_Mobile;
                result.IsPublished = 0;
                _context.SolutionPlay.Update(result);
                await _context.SaveChangesAsync();

                var showcaseSolution = _context.TechnologyShowcasePartnerSolution.Where(t => t.SolutionPlayId == updatedDetails.SolutionPlayId).ToList();
                if (showcaseSolution?.Count > 0)
                {
                    foreach (var scSolution in showcaseSolution)
                    {
                        _context.Remove(scSolution);
                        _context.SaveChanges();
                    }
                }
                updatedDetails.PartnersSolutions?.ForEach(solution =>
                {
                    var orgDetails = _context.Organization.Where(t => t.OrgName == solution.PartnerName).FirstOrDefault();
                    string slug = Regex.Replace(solution.PartnerName, @"<[^>]+>|&nbsp;", "").Trim();
                    slug = Regex.Replace(slug, @"\s{2,}", " ");
                    slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                    slug = Regex.Replace(slug, @" ", "-");
                    slug = slug.ToLower();

                    var technologyShowcasePartnerSolution = new TechnologyShowcasePartnerSolution
                    {
                        TechnologyShowcasePartnerSolutionId = Guid.NewGuid(),
                        SolutionPlayId = updatedDetails.SolutionPlayId,
                        //PartnerId = updatedDetails.Theme.PartnerId,
                        PartnerName = solution.PartnerName,
                        MarketPlaceLink = solution.MarketPlaceLink,
                        websiteLink = solution.websiteLink,
                        overviewDescription = solution.overviewDescription,
                        logoFileLink = solution.logoFileLink ?? orgDetails.logoFileLink,
                        Status = "Created",
                        RowChangedDate = DateTime.UtcNow,
                        PartnerNameSlug = slug,
                    };
                    _context.TechnologyShowcasePartnerSolution.Add(technologyShowcasePartnerSolution);
                    _context.SaveChanges();
                    
                });
                res.Response = true;
                res.Message = "Updated Successfully";
                return res;
            }
            else
            {
                res.Response = false;
                res.Message = "Data not exists";
                return res;
            }
        }
        [HttpPost]
        [Route("AddSolutionPlay")]
        public async Task<ResponseDTO> AddSolutionPlay([FromBody] SolutionPlayViewDTO updatedDetails)
        {
            try
            {
                ResponseDTO res = new ResponseDTO();
                string slug = Regex.Replace(updatedDetails.SolutionPlayLabel, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                string slugTmp = slug;
                Boolean slugCheck = true;
                int i = 1;
                while (slugCheck)
                {
                    var resCheck = _context.SolutionPlay.Where(t => t.SolutionPlayThemeSlug == slug).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                        slug = slugTmp + "-" + i;
                    i++;
                }
                var solutionPlay = new SolutionPlay
                {
                    SolutionPlayId = Guid.NewGuid(),
                    SolutionAreaId = updatedDetails.SolutionAreaId,
                    SolutionPlayThemeSlug = slug,
                    SolutionPlayName = updatedDetails.SolutionPlayName,
                    SolutionPlayDesc = updatedDetails.SolutionPlayDesc,
                    SolutionStatusId = updatedDetails.SolutionStatusId,
                    RowChangedDate = DateTime.UtcNow,
                    RowChangedBy = updatedDetails.RowChangedBy,
                    SolutionPlayLabel = updatedDetails.SolutionPlayLabel,
                    Image_Thumb = updatedDetails.ImageThumb,
                    Image_Main = updatedDetails.Image_Main,
                    Image_Mobile = updatedDetails.Image_Mobile,
                    IsPublished = 0,
                };
                _context.SolutionPlay.Add(solutionPlay);
                await _context.SaveChangesAsync();

                updatedDetails.PartnersSolutions?.ForEach(solution =>
                {
                    var orgDetails = _context.Organization.Where(t => t.OrgName == solution.PartnerName).FirstOrDefault();
                    string slug = Regex.Replace(solution.PartnerName, @"<[^>]+>|&nbsp;", "").Trim();
                    slug = Regex.Replace(slug, @"\s{2,}", " ");
                    slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                    slug = Regex.Replace(slug, @" ", "-");
                    slug = slug.ToLower();

                    var technologyShowcasePartnerSolution = new TechnologyShowcasePartnerSolution
                    {
                        TechnologyShowcasePartnerSolutionId = Guid.NewGuid(),
                        SolutionPlayId = solutionPlay.SolutionPlayId,
                        //PartnerId = updatedDetails.Theme.PartnerId,
                        PartnerName = solution.PartnerName,
                        MarketPlaceLink = solution.MarketPlaceLink,
                        websiteLink = solution.websiteLink,
                        overviewDescription = solution.overviewDescription,
                        logoFileLink = solution.logoFileLink ?? orgDetails.logoFileLink,
                        Status = "Created",
                        RowChangedDate = DateTime.UtcNow,
                        PartnerNameSlug = slug,
                    };
                    _context.TechnologyShowcasePartnerSolution.Add(technologyShowcasePartnerSolution);
                    _context.SaveChanges();

                });

                res.Response = true;
                res.Message = "Added Successfully";
                return res;
            }
            catch
            {
                ResponseDTO res = new ResponseDTO();
                res.Response = false;
                res.Message = "Something went wrong. Please contact administrator";
                return res;
            }
        }
        [HttpPost]
        [Route("PublishedSolutionPlay")]
        public async Task<ResponseDTO> PublishedSolutionPlay(SolutionPlay updatedDetails)
        {
            ResponseDTO res = new ResponseDTO();
            var result = _context.SolutionPlay.Where(u => u.SolutionPlayId == updatedDetails.SolutionPlayId).FirstOrDefault();
            if (result != null)
            {
                result.IsPublished = updatedDetails.IsPublished;
                result.RowChangedBy = updatedDetails.RowChangedBy;
                result.RowChangedDate = DateTime.UtcNow;
                _context.SolutionPlay.Update(result);
                await _context.SaveChangesAsync();
                res.Response = true;
                res.Message = "Updated Successfully";
                return res;
            }
            else
            {
                res.Response = false;
                res.Message = "Data not exists";
                return res;
            }
        }
        [HttpGet]
        [Route("CloneSolutionPlay")]
        public async Task<ResponseDTO> CloneSolutionPlay(Guid SolutionPlayId)
        {
            ResponseDTO res = new ResponseDTO();
            var data = _context.SolutionPlay.Where(t => t.SolutionPlayId == SolutionPlayId).FirstOrDefault();
            var status = _context.SolutionStatusType.Where(t => t.SolutionStatus == "Draft/In progress").FirstOrDefault();
            if (data != null)
            {
                string slug = Regex.Replace(data.SolutionPlayName, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                string slugTmp = slug;
                Boolean slugCheck = true;
                int i = 1;
                while (slugCheck)
                {
                    var resCheck = _context.SolutionPlay.Where(t => t.SolutionPlayThemeSlug == slug).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                        slug = slugTmp + "-" + i;
                    i++;
                }
                var solutionPlay = new SolutionPlay
                {
                    SolutionPlayId = Guid.NewGuid(),
                    SolutionAreaId = data.SolutionAreaId,
                    SolutionPlayThemeSlug = slug,
                    SolutionPlayName = data.SolutionPlayName,
                    SolutionPlayDesc = data.SolutionPlayDesc,
                    SolutionPlayLabel = data.SolutionPlayLabel,
                    Image_Thumb = data.Image_Thumb,
                    Image_Main = data.Image_Main,
                    Image_Mobile = data.Image_Mobile,
                    SolutionStatusId = status.SolutionStatusId,
                    RowChangedDate = DateTime.UtcNow,
                    RowChangedBy = data.RowChangedBy,
                    IsPublished = 0,
                };
                _context.SolutionPlay.Add(solutionPlay);
                await _context.SaveChangesAsync();
                res.Response = true;
                res.Message = "Cloned Successfully";
                return res;
            }
            else
            {
                res.Response = false;
                res.Message = "Data not exists";
                return res;
            }
        }
        [HttpPost]
        [Route("DeleteSolutionPlay")]
        public async Task<ResponseDTO> DeleteSolutionPlay(SolutionPlay updatedDetails)
        {
            ResponseDTO res = new ResponseDTO();
            var result = _context.SolutionPlay.Where(u => u.SolutionPlayId == updatedDetails.SolutionPlayId).FirstOrDefault();
            if (result != null)
            {
                _context.Remove(result);
                _context.SaveChanges();
                res.Response = true;
                res.Message = "Deleted Successfully";
                return res;
            }
            else
            {
                res.Response = false;
                res.Message = "Data not exists";
                return res;
            }
        }
        [HttpPost]
        [Route("createpartnersolutionplay")]
        public async Task<ResponseDTO> PostPartnerSolutionPlay([FromBody] PartnerSolutionPlayDTO partnerSolutionPlayDTO)
        {
            try
            {
                var Status = _context.SolutionStatusType.Where(t => t.SolutionStatus == "Submitted for approval").FirstOrDefault();
                string slug = Regex.Replace(partnerSolutionPlayDTO.SolutionPlayName, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                Boolean slugCheck = true;
                string slugTmp = slug;
                while (slugCheck)
                {
                    var resCheck = _context.PartnerSolutionPlay.Where(t => t.PartnerSolutionPlaySlug == slug).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                    {
                        Random rnd = new Random();
                        int j = rnd.Next(1, 1003);
                        slug = slugTmp + "-" + j;
                    }

                }
                var solution = new PartnerSolutionPlay
                {
                    PartnerSolutionPlayId = Guid.NewGuid(),
                    SolutionAreaId = partnerSolutionPlayDTO.SolutionAreaId.GetValueOrDefault(),
                    OrgId = partnerSolutionPlayDTO.OrgId.GetValueOrDefault(),
                    LogoFileLink = partnerSolutionPlayDTO.LogoFileLink,
                    MarketplaceLink = partnerSolutionPlayDTO.MarketplaceLink,
                    SolutionPlayDescription = partnerSolutionPlayDTO.SolutionPlayDescription ?? "Test Description",
                    SpecialOfferLink = partnerSolutionPlayDTO.SpecialOfferLink,
                    SolutionPlayOrgWebsite = partnerSolutionPlayDTO.SolutionPlayOrgWebsite,
                    SolutionPlayName = partnerSolutionPlayDTO.SolutionPlayName,
                    SolutionStatusId = partnerSolutionPlayDTO.SolutionStatusId,
                    IsPublished = 0,
                    RowChangedBy = partnerSolutionPlayDTO.RowChangedBy.GetValueOrDefault(),
                    RowChangedDate = DateTime.UtcNow,
                    PartnerSolutionPlaySlug = slug,
                    Image_Thumb = partnerSolutionPlayDTO.Image_Thumb,
                    Image_Main = partnerSolutionPlayDTO.Image_Main,
                    Image_Mobile = partnerSolutionPlayDTO.Image_Mobile,
                    IndustryDesignation = partnerSolutionPlayDTO.IndustryDesignation
                };
                _context.PartnerSolutionPlay.Add(solution);

                /*var org = _context.Organization.Where(t => t.OrgId == partnerSolutionDTO.OrganizationId).FirstOrDefault();
                if (org != null)
                {
                    org.logoFileLink = partnerSolutionDTO.LogoFileLink;
                    org.orgWebsite = partnerSolutionDTO.SolutionOrgWebsite;
                    _context.Organization.Update(org);
                }*/

                if (partnerSolutionPlayDTO.SelectedGeos?.Count > 0)
                {
                    foreach (var geoId in partnerSolutionPlayDTO.SelectedGeos)
                    {
                        var partnerSolutionPlayAvailableGeo = new PartnerSolutionPlayAvailableGeo
                        {
                            PartnerSolutionPlayAvailableGeoId = Guid.NewGuid(),
                            PartnerSolutionPlayId = solution.PartnerSolutionPlayId,
                            Status = "Created",
                            RowChangedDate = DateTime.UtcNow,
                        };

                        if (Guid.TryParse(geoId, out var partnerSolutionPlayAvailableGeoId))
                        {
                            partnerSolutionPlayAvailableGeo.GeoId = partnerSolutionPlayAvailableGeoId;
                            _context.PartnerSolutionPlayAvailableGeo.Add(partnerSolutionPlayAvailableGeo);
                        }
                    }
                }
                if (partnerSolutionPlayDTO.PartnerSolutionPlays.Count > 0)
                {

                    foreach (var play in partnerSolutionPlayDTO.PartnerSolutionPlays)
                    {
                        var partnerSolutionPlayByPlay = new PartnerSolutionPlayByPlay
                        {
                            PartnerSolutionPlayByPlayId = Guid.NewGuid(),
                            PartnerSolutionPlayId = solution.PartnerSolutionPlayId,
                            SolutionPlayId = play.SolutionPlayId,
                            PlaySolutionDescription = play.PlaySolutionDescription,
                            Status = "Created",
                            RowChangedDate = DateTime.UtcNow,
                        };

                        _context.PartnerSolutionPlayByPlay.Add(partnerSolutionPlayByPlay);

                        if (play.PartnerSolutionPlayResourceLinks.Count > 0)
                        {
                            foreach (PartnerSolutionPlayResourceLinkDTO rl in play.PartnerSolutionPlayResourceLinks)
                            {
                                var partnerSolutionPlayResourceLink = new PartnerSolutionPlayResourceLink
                                {
                                    PartnerSolutionPlayResourceLinkId = Guid.NewGuid(),
                                    PartnerSolutionPlayByPlayId = partnerSolutionPlayByPlay.PartnerSolutionPlayByPlayId,
                                    ResourceLinkId = rl.resourceLinkId.Value,
                                    ResourceLinkTitle = rl.resourceLinkTitle,
                                    ResourceLinkUrl = rl.resourceLinkUrl,
                                    //ResourceLinkOverview = rl.resourceLinkOverview,
                                    Status = "Created",
                                    RowChangedDate = DateTime.UtcNow,

                                };

                                _context.PartnerSolutionPlayResourceLink.Add(partnerSolutionPlayResourceLink);

                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
                ResponseDTO response = new ResponseDTO();
                response.Response = true;
                response.Message = "Created Successfully";
                var utilities = new Utilities(configuration);
                var newUserRecipientEmail = configuration.GetSection("EmailSettings").
                GetSection("KnackAdmin").Value;
                var filePath = _basefilePath + "\\Technology_Solution_Submitted.html";
                var emailBody = System.IO.File.ReadAllText(filePath);
                emailBody = emailBody.Replace("#solutionname#", partnerSolutionPlayDTO.SolutionPlayName);
                emailBody = emailBody.Replace("#website#", partnerSolutionPlayDTO.SolutionPlayOrgWebsite);
                emailBody = emailBody.Replace("#status#", Status.SolutionStatus);
                emailBody = emailBody.Replace("#description#", partnerSolutionPlayDTO.SolutionPlayDescription);
                utilities.SendMail(newUserRecipientEmail, "New Solution Play Submitted", emailBody);
                return response;
            }
            catch (Exception ex)
            {
                ResponseDTO response = new ResponseDTO();
                response.Response = false;
                response.Message = "Api Error";
                return response;
            }
        }
        [HttpPost]
        [Route("updatePartnerSolutionPlay")]
        public async Task<ResponseDTO> PostupdatePartnerSolutionPlay([FromBody] PartnerSolutionPlayDTO partnerSolutionPlayDTO)
        {
            try
            {
                var utilities = new Utilities(configuration);

                string slug = Regex.Replace(partnerSolutionPlayDTO.SolutionPlayName, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                Boolean slugCheck = true;
                string slugTmp = slug;
                while (slugCheck)
                {
                    var resCheck = _context.PartnerSolutionPlay.Where(t => t.PartnerSolutionPlaySlug == slug).Where(t => t.PartnerSolutionPlayId != partnerSolutionPlayDTO.PartnerSolutionPlayId).FirstOrDefault();
                    if (resCheck == null)
                        slugCheck = false;
                    else
                    {
                        Random rnd = new Random();
                        int j = rnd.Next(1, 1003);
                        slug = slugTmp + "-" + j;
                    }

                }

                var solution = _context.PartnerSolutionPlay.Where(t => t.PartnerSolutionPlayId == partnerSolutionPlayDTO.PartnerSolutionPlayId).FirstOrDefault();

                solution.SolutionAreaId = partnerSolutionPlayDTO.SolutionAreaId.GetValueOrDefault();
                solution.LogoFileLink = partnerSolutionPlayDTO.LogoFileLink;
                solution.MarketplaceLink = partnerSolutionPlayDTO.MarketplaceLink;
                solution.SolutionPlayDescription = partnerSolutionPlayDTO.SolutionPlayDescription ?? "Test Description";
                solution.SpecialOfferLink = partnerSolutionPlayDTO.SpecialOfferLink;
                solution.SolutionPlayOrgWebsite = partnerSolutionPlayDTO.SolutionPlayOrgWebsite;
                solution.SolutionPlayName = partnerSolutionPlayDTO.SolutionPlayName;
                solution.SolutionStatusId = partnerSolutionPlayDTO.SolutionStatusId;
                solution.IsPublished = 0;
                solution.RowChangedBy = partnerSolutionPlayDTO.RowChangedBy;
                solution.RowChangedDate = DateTime.UtcNow;
                solution.PartnerSolutionPlaySlug = slug;
                solution.Image_Thumb = partnerSolutionPlayDTO.Image_Thumb;
                solution.Image_Main = partnerSolutionPlayDTO.Image_Main;
                solution.Image_Mobile = partnerSolutionPlayDTO.Image_Mobile;
                solution.IndustryDesignation = partnerSolutionPlayDTO.IndustryDesignation;

                _context.PartnerSolutionPlay.Update(solution);
                await _context.SaveChangesAsync();


                var solutionAreas = _context.PartnerSolutionPlayByPlay.Where(t => t.PartnerSolutionPlayId == partnerSolutionPlayDTO.PartnerSolutionPlayId).ToList();
                if (solutionAreas?.Count > 0)
                {
                    foreach (var area in solutionAreas)
                    {
                        var resourcess = _context.PartnerSolutionPlayResourceLink.Where(t => t.PartnerSolutionPlayByPlayId == area.PartnerSolutionPlayByPlayId).ToList();
                        if (resourcess?.Count > 0)
                        {
                            foreach (var res in resourcess)
                            {
                                _context.Remove(res);
                                _context.SaveChanges();

                            }
                        }
                        _context.Remove(area);
                        _context.SaveChanges();
                    }
                }
                var geos = _context.PartnerSolutionPlayAvailableGeo.Where(t => t.PartnerSolutionPlayId == partnerSolutionPlayDTO.PartnerSolutionPlayId).ToList();
                if (geos?.Count > 0)
                {
                    foreach (var geo in geos)
                    {
                        _context.Remove(geo);
                        _context.SaveChanges();
                    }
                }

                if (partnerSolutionPlayDTO.SelectedGeos?.Count > 0)
                {
                    foreach (var geoId in partnerSolutionPlayDTO.SelectedGeos)
                    {
                        var partnerSolutionPlayAvailableGeo = new PartnerSolutionPlayAvailableGeo
                        {
                            PartnerSolutionPlayAvailableGeoId = Guid.NewGuid(),
                            PartnerSolutionPlayId = solution.PartnerSolutionPlayId,
                            Status = "Created",
                            RowChangedDate = DateTime.UtcNow,
                        };

                        if (Guid.TryParse(geoId, out var partnerSolutionPlayAvailableGeoId))
                        {
                            partnerSolutionPlayAvailableGeo.GeoId = partnerSolutionPlayAvailableGeoId;
                            _context.PartnerSolutionPlayAvailableGeo.Add(partnerSolutionPlayAvailableGeo);
                        }
                    }
                }
                if (partnerSolutionPlayDTO.PartnerSolutionPlays.Count > 0)
                {

                    foreach (var play in partnerSolutionPlayDTO.PartnerSolutionPlays)
                    {
                        var partnerSolutionPlayByPlay = new PartnerSolutionPlayByPlay
                        {
                            PartnerSolutionPlayByPlayId = Guid.NewGuid(),
                            PartnerSolutionPlayId = solution.PartnerSolutionPlayId,
                            SolutionPlayId = play.SolutionPlayId,
                            PlaySolutionDescription = play.PlaySolutionDescription,
                            Status = "Created",
                            RowChangedDate = DateTime.UtcNow,
                        };

                        _context.PartnerSolutionPlayByPlay.Add(partnerSolutionPlayByPlay);

                        if (play.PartnerSolutionPlayResourceLinks.Count > 0)
                        {
                            foreach (PartnerSolutionPlayResourceLinkDTO rl in play.PartnerSolutionPlayResourceLinks)
                            {
                                var partnerSolutionPlayResourceLink = new PartnerSolutionPlayResourceLink
                                {
                                    PartnerSolutionPlayResourceLinkId = Guid.NewGuid(),
                                    PartnerSolutionPlayByPlayId = partnerSolutionPlayByPlay.PartnerSolutionPlayByPlayId,
                                    ResourceLinkId = rl.resourceLinkId.Value,
                                    ResourceLinkTitle = rl.resourceLinkTitle,
                                    ResourceLinkUrl = rl.resourceLinkUrl,
                                    //ResourceLinkOverview = rl.resourceLinkOverview,
                                    Status = "Created",
                                    RowChangedDate = DateTime.UtcNow,

                                };

                                _context.PartnerSolutionPlayResourceLink.Add(partnerSolutionPlayResourceLink);

                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                ResponseDTO response = new ResponseDTO();
                response.Response = true;
                response.Message = "Updated Successfully";
                var solutionStatus = _context.SolutionStatusType.Where(x => x.SolutionStatusId == partnerSolutionPlayDTO.SolutionStatusId).FirstOrDefault();

                if (solutionStatus.SolutionStatus.Equals("Approved", StringComparison.CurrentCultureIgnoreCase) ||
                    solutionStatus.SolutionStatus.Equals("Pending approval", StringComparison.CurrentCultureIgnoreCase) ||
                     solutionStatus.SolutionStatus.Equals("Correction required", StringComparison.CurrentCultureIgnoreCase))
                {
                    var newUserRecipientEmail = configuration.GetSection("EmailSettings").
                    GetSection("KnackAdmin").Value;
                    var filePath = _basefilePath + "\\Update_Partner_Solution_Play.html";
                    var emailBody = System.IO.File.ReadAllText(filePath);
                    emailBody = emailBody.Replace("#desc#", "Partner solution play has been updated.");
                    emailBody = emailBody.Replace("#website#", partnerSolutionPlayDTO.SolutionPlayOrgWebsite);
                    emailBody = emailBody.Replace("#status#", solutionStatus.SolutionStatus);
                    emailBody = emailBody.Replace("#description#", partnerSolutionPlayDTO.SolutionPlayDescription);
                    utilities.SendMail(newUserRecipientEmail, "Partner Solution Play Updated", emailBody);
                }
                return response;
            }
            catch (Exception ex)
            {
                ResponseDTO response = new ResponseDTO();
                response.Response = false;
                response.Message = "Api Error";
                return response;
            }
        }
        [HttpPost]
        [Route("deletePartnerSolutionbyPlay")]
        public async Task<ResponseDTO> PostdeletePartnerSolutionbyPlay([FromBody] PartnerSolutionPlayDTO partnerSolutionPlayDTO)
        {
            try
            {
                var solution = _context.PartnerSolutionPlay.Where(t => t.PartnerSolutionPlayId == partnerSolutionPlayDTO.PartnerSolutionPlayId).FirstOrDefault();
                ResponseDTO responseDto = new ResponseDTO();
                if (solution != null)
                {
                    _context.Remove(solution);
                    _context.SaveChanges();

                    var solutionAreas = _context.PartnerSolutionPlayByPlay.Where(t => t.PartnerSolutionPlayId == partnerSolutionPlayDTO.PartnerSolutionPlayId).ToList();
                    if (solutionAreas?.Count > 0)
                    {
                        foreach (var area in solutionAreas)
                        {
                            var resourcess = _context.PartnerSolutionPlayResourceLink.Where(t => t.PartnerSolutionPlayByPlayId == area.PartnerSolutionPlayByPlayId).ToList();
                            if (resourcess?.Count > 0)
                            {
                                foreach (var res in resourcess)
                                {
                                    _context.Remove(res);
                                    _context.SaveChanges();

                                }
                            }
                            _context.Remove(area);
                            _context.SaveChanges();
                        }
                    }
                    var geos = _context.PartnerSolutionPlayAvailableGeo.Where(t => t.PartnerSolutionPlayId == partnerSolutionPlayDTO.PartnerSolutionPlayId).ToList();
                    if (geos?.Count > 0)
                    {
                        foreach (var geo in geos)
                        {
                            _context.Remove(geo);
                            _context.SaveChanges();
                        }
                    }
                    await _context.SaveChangesAsync();

                    responseDto.Response = true;
                    responseDto.Message = "Deleted Successfully";
                    return responseDto;
                }
                else
                {
                    responseDto.Response = false;
                    responseDto.Message = "Data not exists";
                    return responseDto;
                }
            }
            catch (Exception ex)
            {
                ResponseDTO res = new ResponseDTO();
                res.Response = false;
                res.Message = "Please contact administrator. Delete api has an problem";
                return res;
            }
        }
        [HttpGet]
        [Route("GetOrganizationPartnerSolutionPlay")]
        public async Task<IEnumerable<OrganizationPartnerSolutionPlayDTO>> GetOrganizationPartnerSolutionPlay(Guid OrgId)
        {
            var result = (from org in _context.Organization
                          join ps in _context.PartnerSolutionPlay
                          on org.OrgId equals ps.OrgId
                          join ss in _context.SolutionStatusType
                          on ps.SolutionStatusId equals ss.SolutionStatusId
                          join sa in _context.SolutionAreas
                          on ps.SolutionAreaId equals sa.SolutionAreaId
                          join spbp in _context.PartnerSolutionPlayByPlay
                          on ps.PartnerSolutionPlayId equals spbp.PartnerSolutionPlayId
                          join sp in _context.SolutionPlay
                          on spbp.SolutionPlayId equals sp.SolutionPlayId
                          where org.OrgId == OrgId
                          orderby ps.SolutionPlayName
                          select new OrganizationPartnerSolutionPlayDTO
                          {
                              PartnerSolutionPlayId = ps.PartnerSolutionPlayId,
                              SolutionAreaId = ps.SolutionAreaId,
                              OrgId = ps.OrgId,
                              SolutionStatusId = ps.SolutionStatusId,
                              SolutionStatus = ss.SolutionStatus,
                              DisplayLabel = ss.DisplayLabel,
                              SolutionPlayName = ps.SolutionPlayName,
                              IsPublished = ps.IsPublished,
                              SolutionAreaName = sa.SolutionAreaName,
                              SolutionPlayId = sp.SolutionPlayId,
                              SolutionPlayLabel = sp.SolutionPlayLabel
                          }).ToList();
            return result;
        }
        [HttpGet]
        [Route("GetPartnerSolutionPlayFilter")]
        public async Task<IEnumerable<OrganizationPartnerSolutionPlayFilterDTO>> GetPartnerSolutionPlayFilter(Guid solutionAreaId, Guid statusID, Guid orgId)
        {
            List<OrganizationPartnerSolutionPlayFilterDTO> PSFs = new List<OrganizationPartnerSolutionPlayFilterDTO>();
            var result = (from org in _context.Organization
                          join ps in _context.PartnerSolutionPlay
                          on org.OrgId equals ps.OrgId
                          join ss in _context.SolutionStatusType
                          on ps.SolutionStatusId equals ss.SolutionStatusId
                          join sa in _context.SolutionAreas
                          on ps.SolutionAreaId equals sa.SolutionAreaId
                          join spbp in _context.PartnerSolutionPlayByPlay
                          on ps.PartnerSolutionPlayId equals spbp.PartnerSolutionPlayId
                          join sp in _context.SolutionPlay
                          on spbp.SolutionPlayId equals sp.SolutionPlayId
                          orderby ps.SolutionPlayName
                          select new OrganizationPartnerSolutionPlayFilterDTO
                          {
                              PartnerSolutionPlayId = ps.PartnerSolutionPlayId,
                              SolutionAreaId = ps.SolutionAreaId,
                              OrgId = ps.OrgId,
                              OrganizationName = org.OrgName,
                              SolutionStatusId = ps.SolutionStatusId,
                              SolutionStatus = ss.SolutionStatus,
                              DisplayLabel = ss.DisplayLabel,
                              SolutionPlayName = ps.SolutionPlayName,
                              IsPublished = ps.IsPublished,
                              SolutionAreaName = sa.SolutionAreaName,
                              SolutionPlayId = sp.SolutionPlayId
                          }).ToList();
            if (statusID != Guid.Empty)
            {
                result = result.Where(i => i.SolutionStatusId == statusID).ToList();
            }
            if (solutionAreaId != Guid.Empty)
            {
                result = result.Where(i => i.SolutionAreaId == solutionAreaId).ToList();               
            }
            if (orgId != Guid.Empty)
            {
                result = result.Where(i => i.OrgId == orgId).ToList();
            }
            return result;
            /*using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                var sql = "select ps.PartnerSolutionPlayId, ps.solutionAreaId, ps.OrgId, OrgName, ps.SolutionStatusId,sa.solutionAreaName, ss.SolutionStatus, ss.DisplayLabel, ps.SolutionPlayName, ps.IsPublished, psPlay.solutionPlayId  from organization as o join partnerSolutionPlay as ps on o.orgId = ps.OrgId join SolutionStatus as ss on ps.SolutionStatusId = ss.SolutionStatusId join solutionArea as sa on ps.solutionAreaId = sa.solutionAreaId join partnerSolutionPlayByPlay as psPlay on ps.partnerSolutionPlayId = psPlay.partnerSolutionPlayId where ";
                var sqlstring = 0;
                if (statusID != Guid.Empty)
                {
                    sql += " ps.SolutionStatusId = '" + statusID + "'";
                    sqlstring++;
                }
                if (solutionAreaId != Guid.Empty)
                {
                    if (sqlstring > 0)
                    {
                        sql += " AND ";
                    }
                    sql += " ps.solutionAreaId = '" + solutionAreaId + "'";
                    sqlstring++;
                }
                if (orgId != Guid.Empty)
                {
                    if (sqlstring > 0)
                    {
                        sql += " AND ";
                    }
                    sql += " ps.OrgId = '" + orgId + "'";
                }
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                this._context.Database.OpenConnection();

                using (var results = command.ExecuteReader())
                {
                    while (results.Read())
                    {
                        OrganizationPartnerSolutionPlayFilterDTO pSolution = new OrganizationPartnerSolutionPlayFilterDTO();
                        pSolution.PartnerSolutionPlayId = Guid.Parse(results["PartnerSolutionPlayId"].ToString());
                        pSolution.SolutionAreaId = Guid.Parse(results["solutionAreaId"].ToString());
                        pSolution.OrgId = Guid.Parse(results["OrgId"].ToString());
                        pSolution.OrganizationName = results["OrgName"].ToString();
                        pSolution.SolutionAreaName = results["solutionAreaName"].ToString();
                        pSolution.SolutionStatusId = Guid.Parse(results["SolutionStatusId"].ToString());
                        pSolution.SolutionStatus = results["SolutionStatus"].ToString();
                        pSolution.DisplayLabel = results["DisplayLabel"].ToString();
                        pSolution.SolutionPlayName = results["SolutionPlayName"].ToString();
                        pSolution.IsPublished = results["IsPublished"].ToString();
                        pSolution.SolutionPlayId = Guid.Parse(results["solutionPlayId"].ToString());
                        PSFs.Add(pSolution);
                    }
                }
            }
            return PSFs;*/
        }
        [HttpGet]
        [Route("ClonePartnerSolutionplay")]
        public async Task<ResponseDTO> ClonePartnerSolutionplay(Guid partnerSolutionPlayId)
        {
            try
            {
                ResponseDTO response = new ResponseDTO();
                var status = _context.SolutionStatusType.Where(t => t.SolutionStatus == "Draft/In progress").FirstOrDefault();
                var solution = _context.PartnerSolutionPlay.Where(t => t.PartnerSolutionPlayId == partnerSolutionPlayId).FirstOrDefault();
                var solutionPlay = _context.PartnerSolutionPlayByPlay.Where(t => t.PartnerSolutionPlayId == partnerSolutionPlayId).FirstOrDefault();
                if (solution != null && solutionPlay != null)
                {
                    string solutionname = solution.SolutionPlayName + " copy";
                    Boolean nameCheck = true;
                    string solutionnameTmp = solutionname;
                    int j = 1;
                    while (nameCheck)
                    {
                        var resCheck = (from play in _context.PartnerSolutionPlayByPlay
                                        join ps in _context.PartnerSolutionPlay
                                        on play.PartnerSolutionPlayId equals ps.PartnerSolutionPlayId
                                        where play.SolutionPlayId == solutionPlay.SolutionPlayId
                                        where play.PartnerSolutionPlayId != solution.PartnerSolutionPlayId
                                        where ps.SolutionPlayName.ToLower().Trim() == solutionname
                                        select new PartnerSolutionPlayByPlay
                                        {
                                            SolutionPlayId = play.SolutionPlayId
                                        }).FirstOrDefault();
                        if (resCheck == null)
                            nameCheck = false;
                        else
                        {
                            solutionname = solutionnameTmp + "-" + j;
                            j++;
                        }

                    }
                    solution.SolutionPlayName = solutionname;
                    solution.PartnerSolutionPlayId = Guid.NewGuid();
                    solution.SolutionStatusId = status.SolutionStatusId;
                    //solution.RowChangedBy = status.RowChangedBy;
                    solution.RowChangedDate = DateTime.UtcNow;
                }
                else
                {
                    response.Response = false;
                    response.Message = "Data not exists";
                    return response;
                }
                _context.PartnerSolutionPlay.Add(solution);
                var goes = _context.PartnerSolutionPlayAvailableGeo.Where(t => t.PartnerSolutionPlayId == partnerSolutionPlayId).ToList();
                if (goes?.Count > 0)
                {
                    foreach (var geo in goes)
                    {
                        geo.PartnerSolutionPlayAvailableGeoId = Guid.NewGuid();
                        geo.PartnerSolutionPlayId = solution.PartnerSolutionPlayId;
                        geo.Status = "Created";
                        geo.RowChangedDate = DateTime.UtcNow;
                        geo.GeoId = geo.GeoId;
                        _context.PartnerSolutionPlayAvailableGeo.Add(geo);
                    }
                }
                var solutionAreas = _context.PartnerSolutionPlayByPlay.Where(t => t.PartnerSolutionPlayId == partnerSolutionPlayId).ToList();
                if (solutionAreas?.Count > 0)
                {
                    foreach (var area in solutionAreas)
                    {
                        var resources = _context.PartnerSolutionPlayResourceLink.Where(t => t.PartnerSolutionPlayByPlayId == area.PartnerSolutionPlayByPlayId).ToList();
                        area.PartnerSolutionPlayByPlayId = Guid.NewGuid();
                        area.PartnerSolutionPlayId = solution.PartnerSolutionPlayId;
                        area.RowChangedDate = DateTime.UtcNow;
                        _context.PartnerSolutionPlayByPlay.Add(area);

                        if (resources.Count > 0)
                        {
                            foreach (var res in resources)
                            {

                                res.PartnerSolutionPlayResourceLinkId = Guid.NewGuid();
                                res.PartnerSolutionPlayByPlayId = area.PartnerSolutionPlayByPlayId;
                                res.RowChangedDate = DateTime.UtcNow;
                                _context.PartnerSolutionPlayResourceLink.Add(res);
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();

                response.Response = true;
                response.Message = "Cloned Successfully";
                return response;
            }
            catch (Exception ex)
            {
                ResponseDTO response = new ResponseDTO();
                response.Response = false;
                response.Message = "Api Error";
                return response;
            }
        }
        [HttpPost]
        [Route("updatePartnerSolutionPlayPublish")]
        public async Task<ResponseDTO> updatePartnerSolutionPlayPublish([FromBody] PartnerSolutionPlayPublishDTO partnerSolutionPlayPublishDTO)
        {
            try
            {
                var solution = _context.PartnerSolutionPlay.Where(t => t.PartnerSolutionPlayId == partnerSolutionPlayPublishDTO.PartnerSolutionPlayId).FirstOrDefault();
                solution.IsPublished = partnerSolutionPlayPublishDTO.IsPublished;
                _context.PartnerSolutionPlay.Update(solution);
                await _context.SaveChangesAsync();
                ResponseDTO res = new ResponseDTO();
                res.Response = true;
                res.Message = "Updated Successfully";
                return res;
            }
            catch (Exception ex)
            {
                ResponseDTO res = new ResponseDTO();
                res.Response = false;
                res.Message = "Api Error";
                return res;
            }
        }
        [HttpGet]
        [Route("GetPartnerSolutionPlayById")]

        public PartnerSolutionPlaySimplifiedDTO GetPartnerSolutionPlayById(Guid PartnerSolutionPlayId)
        {
            var pSolution = (from partnerSoln in _context.PartnerSolutionPlay
                             join org in _context.Organization
                             on partnerSoln.OrgId equals org.OrgId
                             join ss in _context.SolutionStatusType
                             on partnerSoln.SolutionStatusId equals ss.SolutionStatusId
                             where partnerSoln.PartnerSolutionPlayId == PartnerSolutionPlayId
                             select new PartnerSolutionPlaySimplifiedDTO
                             {
                                 PartnerSolutionPlayId = partnerSoln.PartnerSolutionPlayId,
                                 SolutionAreaId = partnerSoln.SolutionAreaId,
                                 OrgId = partnerSoln.OrgId,
                                 SolutionPlayName = partnerSoln.SolutionPlayName,
                                 SolutionPlayDescription = partnerSoln.SolutionPlayDescription,
                                 SolutionPlayOrgWebsite = partnerSoln.SolutionPlayOrgWebsite,
                                 MarketplaceLink = partnerSoln.MarketplaceLink,
                                 SpecialOfferLink = partnerSoln.SpecialOfferLink,
                                 LogoFileLink = partnerSoln.LogoFileLink,
                                 SolutionStatusId = partnerSoln.SolutionStatusId,
                                 IsPublished = partnerSoln.IsPublished,
                                 RowChangedBy = partnerSoln.RowChangedBy,
                                 DisplayLabel = ss.DisplayLabel,
                                 Image_Thumb = partnerSoln.Image_Thumb,
                                 Image_Main = partnerSoln.Image_Main,
                                 Image_Mobile = partnerSoln.Image_Mobile,
                                 IndustryDesignation = partnerSoln.IndustryDesignation
                             }).FirstOrDefault();
            if (pSolution != null)
            {
                pSolution.PartnerSolutionPlayAvailableGeo = _context.PartnerSolutionPlayAvailableGeo.FromSqlRaw
                       (
                           "select * from PartnerSolutionPlayAvailableGeo where partnerSolutionPlayId = @partnerSolutionPlayId",
                           new SqlParameter("@partnerSolutionPlayId", PartnerSolutionPlayId)
                       ).ToList();

                var PartnerSolutionPlays = (from solnArea in _context.PartnerSolutionPlayByPlay
                                            where solnArea.PartnerSolutionPlayId == PartnerSolutionPlayId
                                            select new PartnerSolutionPlayByPlayDTO
                                            {
                                                PartnerSolutionPlayByPlayId = solnArea.PartnerSolutionPlayByPlayId,
                                                SolutionPlayId = solnArea.SolutionPlayId,
                                                PlaySolutionDescription = solnArea.PlaySolutionDescription
                                            }).ToList();



                var getDetailsAry = new List<PartnerSolutionPlayByPlayDTO>();
                for (int i = 0; i < PartnerSolutionPlays.Count(); i++)
                {

                    var PartnerSolutionPlayResourceLinks = (from resource in _context.PartnerSolutionPlayResourceLink
                                                            where resource.PartnerSolutionPlayByPlayId == PartnerSolutionPlays[i].PartnerSolutionPlayByPlayId
                                                            select new PartnerSolutionPlayResourceLinkDTO
                                                            {
                                                                resourceLinkId = resource.ResourceLinkId,
                                                                resourceLinkTitle = resource.ResourceLinkTitle,
                                                                resourceLinkUrl = resource.ResourceLinkUrl,
                                                                //resourceLinkOverview = resource.ResourceLinkOverview,
                                                            }).ToList();
                    PartnerSolutionPlays[i].PartnerSolutionPlayResourceLinks = PartnerSolutionPlayResourceLinks;
                    getDetailsAry.Add(PartnerSolutionPlays[i]);
                }
                pSolution.PartnerSolutionPlays = getDetailsAry;
                return pSolution;
            }
            else
            {
                return pSolution;
            }
        }
        [HttpGet]
        [Route("GetSolutionPlayForPartnerProfile")]
        public async Task<IEnumerable<SolutionPlay>> GetSolutionPlayForPartnerProfile(Guid solutionAreaId)
        {
            var solutionPlay = (from play in _context.SolutionPlay
                                join ss in _context.SolutionStatusType
                                on play.SolutionStatusId equals ss.SolutionStatusId
                                where play.SolutionAreaId == solutionAreaId
                                where ss.SolutionStatus == "Approved"
                                select new SolutionPlay
                                {
                                    SolutionPlayId = play.SolutionPlayId,
                                    SolutionAreaId = play.SolutionAreaId,
                                    SolutionPlayName = play.SolutionPlayName,
                                    SolutionPlayLabel = play.SolutionPlayLabel
                                }).ToList();
            return solutionPlay;
        }
        [HttpGet]
        [Route("GetSolutionPlayIntake")]
        public async Task<IEnumerable<SolutionPlayDTO>> GetSolutionPlayIntake()
        {
            var solutionPlay = (from play in _context.SolutionPlay
                                join sa in _context.SolutionAreas
                                on play.SolutionAreaId equals sa.SolutionAreaId
                                join ss in _context.SolutionStatusType
                                on play.SolutionStatusId equals ss.SolutionStatusId
                                where ss.SolutionStatus == "Approved"
                                orderby play.SolutionPlayName
                                select new SolutionPlayDTO
                                {
                                    SolutionPlayId = play.SolutionPlayId,
                                    SolutionAreaId = play.SolutionAreaId,
                                    SolutionPlayName = play.SolutionPlayName,
                                    SolutionPlayLabel = play.SolutionPlayLabel,
                                    SolutionAreaName = sa.SolutionAreaName
                                }).ToList();
            return solutionPlay;
        }
        [HttpGet]
        [Route("CheckSolutionPlayAdd")]
        public async Task<ResponseDTO> CheckSolutionPlayAdd(Guid SolutionPlayId, String SolutionName)
        {
            ResponseDTO res = new ResponseDTO();
            var solutionNameCheck = SolutionName.Trim().ToLower();
            var solution = (from play in _context.PartnerSolutionPlayByPlay
                            join ps in _context.PartnerSolutionPlay
                            on play.PartnerSolutionPlayId equals ps.PartnerSolutionPlayId
                            where play.SolutionPlayId == SolutionPlayId
                            where ps.SolutionPlayName.ToLower().Trim() == solutionNameCheck
                            select new PartnerSolutionPlayByPlay
                            {
                                SolutionPlayId = play.SolutionPlayId
                            }).FirstOrDefault();
            //var solution = _context.PartnerSolutionPlayByPlay.Where(t => t.SolutionPlayId == SolutionPlayId).FirstOrDefault();
            if (solution == null)
            {
                res.Response = true;
                res.Message = "Valid";
            }
            else
            {
                res.Response = false;
                res.Message = "Solution name already exists. Please enter different solution name";
            }
            return res;
        }
        [HttpGet]
        [Route("CheckSolutionPlayEdit")]
        public async Task<ResponseDTO> CheckSolutionPlayEdit(Guid SolutionPlayId, Guid PartnerSolutionPlayId, String SolutionName)
        {
            ResponseDTO res = new ResponseDTO();
            var solutionNameCheck = SolutionName.Trim().ToLower();
            var solution = (from play in _context.PartnerSolutionPlayByPlay
                            join ps in _context.PartnerSolutionPlay
                            on play.PartnerSolutionPlayId equals ps.PartnerSolutionPlayId
                            where play.SolutionPlayId == SolutionPlayId
                            where play.PartnerSolutionPlayId != PartnerSolutionPlayId
                            where ps.SolutionPlayName.ToLower().Trim() == solutionNameCheck
                            select new PartnerSolutionPlayByPlay
                            {
                                SolutionPlayId = play.SolutionPlayId
                            }).FirstOrDefault();
            //var solution = _context.PartnerSolutionPlayByPlay.Where(t => t.PartnerSolutionPlayId != PartnerSolutionPlayId).Where(t => t.SolutionPlayId == SolutionPlayId).FirstOrDefault();
            if (solution == null)
            {
                res.Response = true;
                res.Message = "Valid";
            }
            else
            {
                res.Response = false;
                res.Message = "Solution name already exists. Please enter different solution name";
            }
            return res;
        }
        [HttpGet]
        [Route("GetUserOtp")]
        public async Task<ResponseDTO> GetUserOtp(string emailAddress)
        {
            var utilities = new Utilities(configuration);
            var response = new ResponseDTO();
            string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
            string encEmail = utilities.EncryptString(encKey, emailAddress);
            string domain = emailAddress.Split('@')[1];
            //if (domain.ToLower() == "microsoft.com" || domain.ToLower() == "knackcollective.com")
            if (domain.ToLower() == "microsoft.com"
                 || domain.ToLower() == "meekscs.com"
                 || domain.ToLower() == "knackcollective.com")
            {
                response.Response = false;
                response.Message = "Invalid user type.";
                return response;
            }
            else
            {
                var data = _context.userOtps.Where(t => t.UserEmail == encEmail).OrderByDescending(i => i.RowChangedDate).FirstOrDefault();
                if (data != null)
                {
                    DateTime startTime = (DateTime)data.RowChangedDate;
                    TimeSpan breakDuration = TimeSpan.FromMinutes(5);
                    if ((DateTime.UtcNow - startTime) < breakDuration)
                    {
                        response.Response = true;
                        response.Message = "Email has been sent already.";
                        return response;
                    }
                }
                string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                string sOTP = String.Empty;
                string sTempChars = String.Empty;
                Random rand = new Random();
                for (int i = 0; i < 5; i++)
                {
                    int p = rand.Next(0, saAllowedCharacters.Length);
                    sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];
                    sOTP += sTempChars;
                }
                var otpRecord = _context.userOtps.Where(t => t.UserEmail == encEmail).ToList();
                if (otpRecord.Count() > 0)
                {
                    foreach (var otp in otpRecord)
                    {
                        _context.Remove(otp);
                        _context.SaveChanges();
                    }
                }
                var res = new UserOtp
                {
                    UserOtpId = Guid.NewGuid(),
                    UserEmail = encEmail,
                    OtpNumber = sOTP,
                    RowChangedDate = DateTime.UtcNow
                };
                _context.userOtps.Add(res);
                await _context.SaveChangesAsync();
                var recepientDetails = new RecipientOTPMailDTO
                {
                    RecipientEmail = emailAddress,
                    RecipientName = emailAddress,
                    Subject = "Industry Solutions Directory Verification Email",
                    Body = $"<div style=\"font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2\">\r\n  <div style=\"margin:50px auto;width:70%;padding:20px 0\">\r\n    <div style=\"border-bottom:1px solid #eee;text-align: center;padding-bottom: 20px;\" >\r\n      <img src=\"https://partner.microsoftindustryinsights.com/assets/images/Microsoft_Logo_Color@2x.png\" alt=\"Microsoft Color Logo\" style=\"width: 150px;\">\r\n    </div>\r\n    <p style=\"font-size:1.1em\">Hi,</p>\r\n    <p>Thank You for choosing the  Industry Solutions Directory. Please use the following OTP to complete your Sign In.  This OTP code will expire in 10 minutes.</p>\r\n    <h2 style=\"background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;\">{sOTP}</h2>\r\n    <p style=\"font-size:0.9em;\">Regards,<br />Industry Solutions Directory</p>\r\n    <hr style=\"border:none;border-top:1px solid #eee\" />    \r\n  </div>\r\n</div>",
                    OTP = sOTP,
                    smtpServer = configuration.GetSection("KnackSettings").GetSection("SMTPServer").Value,
                    smtpUsername = configuration.GetSection("KnackSettings").GetSection("SMTPUsername").Value,
                    smtpPassword = configuration.GetSection("KnackSettings").GetSection("SMTPPassword").Value

                };
                //var mailresponse = new ResponseDTO();

                bool mailresponse = utilities.SendMail(recepientDetails.RecipientEmail, recepientDetails.Subject, recepientDetails.Body);
                if (mailresponse)
                {
                    response.Response = true;
                    //response.Val = sOTP;
                    response.Message = "Successfully Otp sent to user email address";
                }
                else
                {
                    response.Response = false;
                    response.Message = "Mail not send. Please contact administrator";
                }

                return response;
            }
        }
        [HttpGet]
        [Route("CheckUserOtp")]
        public async Task<ResponseDTO> CheckUserOtp(string emailAddress, string OtpNumber)
        {
            var utilities = new Utilities(configuration);
            var response = new ResponseDTO();
            string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
            string encEmail = utilities.EncryptString(encKey, emailAddress);
            var data = _context.userOtps.Where(t => t.UserEmail == encEmail).Where(t => t.OtpNumber == OtpNumber).FirstOrDefault();
            if (data != null)
            {
                DateTime startTime = (DateTime)data.RowChangedDate;
                TimeSpan breakDuration = TimeSpan.FromMinutes(10);
                var otpRecord = _context.userOtps.Where(t => t.UserEmail == encEmail).ToList();
                if ((DateTime.UtcNow - startTime) < breakDuration)
                {
                    response.Response = true;
                    response.Message = "Valid Otp";
                    foreach (var otp in otpRecord)
                    {
                        _context.Remove(otp);
                        _context.SaveChanges();
                    }
                    return response;
                }
                else
                {
                    response.Response = false;
                    response.Message = "OTP is expired. Please click Resend OTP to get the new OTP.";
                    return response;
                }
            }
            else
            {
                response.Response = false;
                response.Message = "Invalid Otp";
                return response;
            }

        }
        [HttpGet]
        [Route("GetMenuSolutionPlay")]
        public async Task<IEnumerable<MenuSolutionAreaListDTO>> GetMenuSolutionPlay()
        {
            var solutionAreas = await _context.SolutionAreas.OrderBy(i => i.SolutionAreaName).ToListAsync();

            List<MenuSolutionAreaListDTO> iDTOs = new List<MenuSolutionAreaListDTO>();

            foreach (SolutionArea i in solutionAreas)
            {
                MenuSolutionAreaListDTO iDTO = new MenuSolutionAreaListDTO();

                iDTO.SolutionAreaId = i.SolutionAreaId;
                iDTO.SolutionAreaName = i.SolutionAreaName;
                iDTO.SolutionAreaSlug = i.SolutionAreaSlug;
                iDTO.hasSubMenu = false;

                List<MenuSolutionPlayListDTO> solnPlays = new List<MenuSolutionPlayListDTO>();

                
                solnPlays = (from sp in _context.SolutionPlay
                             join sa in _context.SolutionAreas
                             on sp.SolutionAreaId equals sa.SolutionAreaId
                             join ss in _context.SolutionStatusType
                             on sp.SolutionStatusId equals ss.SolutionStatusId
                             where ss.SolutionStatus == "Approved"
                             where sp.IsPublished == 1
                             where sp.SolutionAreaId == i.SolutionAreaId
                             orderby sp.SolutionPlayName
                             select new MenuSolutionPlayListDTO
                             {
                                 SolutionPlayId = sp.SolutionPlayId,
                                 SolutionAreaId = sp.SolutionAreaId,
                                 SolutionPlayLabel = sp.SolutionPlayLabel,
                                 SolutionPlayThemeSlug = sp.SolutionPlayThemeSlug,
                                 SolutionPlayName = sp.SolutionPlayName
                             }).ToList();
                if (solnPlays.Count() > 0)
                {
                    foreach (var solnplay in solnPlays)
                    {
                        var pSolution = (from partnerSoln in _context.PartnerSolutionPlay
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
                                         where solutionPlay.SolutionPlayId == solnplay.SolutionPlayId
                                         where ss.SolutionStatus == "Approved"
                                         where partnerSoln.IsPublished == 1
                                         select new PartnerSolutionPlayViewDTO
                                         {
                                             PartnerSolutionPlayId = partnerSoln.PartnerSolutionPlayId,
                                         }).FirstOrDefault();
                        if (pSolution != null)
                            iDTO.hasSubMenu = true;
                    }
                }

                iDTO.SolutionPlays = solnPlays;
                iDTOs.Add(iDTO);
            }
            return iDTOs;
        }
        [HttpGet]
        [Route("GetSolutionPlayBySolutionAreaViewId")]
        public async Task<IEnumerable<SolutionPlayViewDTO>> GetSolutionPlayBySolutionAreaViewId(String SolutionAreaId)
        {
            List<SolutionPlayViewDTO> solnPlays = new List<SolutionPlayViewDTO>();

            using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "select distinct(sp.solutionPlayId), sp.solutionAreaId, sp.solutionPlayThemeSlug, sp.solutionPlayName,sp.solutionPlayLabel,sp.image_thumb,sa.solutionAreaName, sa.solutionAreaSlug from solutionPlay as sp join solutionArea sa on sp.solutionAreaId = sa.solutionAreaId  join SolutionStatus as ss on sp.solutionStatusId = ss.solutionStatusId left join partnerSolutionPlayByPlay as psPlayByPlay on sp.solutionPlayId = psPlayByPlay.solutionPlayId join partnerSolutionPlay as psPlay on psPlayByPlay.PartnerSolutionPlayId = psPlay.PartnerSolutionPlayId where ss.SolutionStatus = 'Approved' and sp.IsPublished = 1 and psPlay.IsPublished = 1 and solutionAreaSlug = @solutionAreaId";
                command.CommandType = CommandType.Text;
                SqlParameter param = new SqlParameter("@solutionAreaId", SolutionAreaId);
                command.Parameters.Add(param);
                this._context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        SolutionPlayViewDTO solnPlay = new SolutionPlayViewDTO();
                        solnPlay.SolutionPlayId = Guid.Parse(result["SolutionPlayId"].ToString());
                        solnPlay.SolutionAreaId = Guid.Parse(result["solutionAreaId"].ToString());
                        solnPlay.SolutionPlayLabel = result["solutionPlayLabel"].ToString();
                        solnPlay.SolutionPlayThemeSlug = result["solutionPlayThemeSlug"].ToString();
                        solnPlay.SolutionPlayName = result["solutionPlayName"].ToString();
                        solnPlay.ImageThumb = result["image_thumb"].ToString();
                        solnPlay.SolutionAreaName = result["solutionAreaName"].ToString();
                        solnPlay.SolutionAreaSlug = result["solutionAreaSlug"].ToString();
                        solnPlays.Add(solnPlay);
                    }
                }
            }
            /*var pSolution = (from partnerSoln in _context.PartnerSolutionPlay
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
                             where solArea.SolutionAreaSlug == SolutionAreaId
                             where ss.SolutionStatus == "Approved"
                             where partnerSoln.IsPublished == 1
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
                                 PartnerSolutionPlaySlug = partnerSoln.PartnerSolutionPlaySlug
                             }).ToList();*/
            return solnPlays;
        }
        [HttpGet]
        [Route("GetSolutionPlayBySolutionPlayViewId")]
        public async Task<IEnumerable<PartnerSolutionPlayViewDTO>> GetSolutionPlayBySolutionPlayViewId(String SolutionPlayThemeSlug)
        {
            var pSolution = (from partnerSoln in _context.PartnerSolutionPlay
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
                             where solutionPlay.SolutionPlayThemeSlug == SolutionPlayThemeSlug
                             where ss.SolutionStatus == "Approved"
                             where partnerSoln.IsPublished == 1
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
                                 SolutionPlayTitle = solutionPlay.SolutionPlayLabel
                             }).ToList();
            return pSolution;
        }
        [HttpGet]
        [Route("GetSolutionPlayViewId")]
        public async Task<IEnumerable<PartnerSolutionPlayViewDTO>> GetSolutionPlayViewId(Guid SolutionPlayId)
        {
            var pSolution = (from partnerSoln in _context.PartnerSolutionPlay
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
                             where solutionPlay.SolutionPlayId == SolutionPlayId
                             where ss.SolutionStatus == "Approved"
                             where partnerSoln.IsPublished == 1
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
                                 PartnerSolutionPlaySlug = partnerSoln.PartnerSolutionPlaySlug
                             }).ToList();
            return pSolution;
        }
        [HttpGet]
        [Route("GetPartnerSolutionPlayByViewId")]
        public PartnerSolutionPlaySimplifiedDTO GetPartnerSolutionPlayByViewId(string slug)
        {
            var pSolution = (from partnerSoln in _context.PartnerSolutionPlay
                             join org in _context.Organization
                             on partnerSoln.OrgId equals org.OrgId
                             join ss in _context.SolutionStatusType
                             on partnerSoln.SolutionStatusId equals ss.SolutionStatusId
                             where partnerSoln.PartnerSolutionPlaySlug == slug
                             select new PartnerSolutionPlaySimplifiedDTO
                             {
                                 PartnerSolutionPlayId = partnerSoln.PartnerSolutionPlayId,
                                 SolutionAreaId = partnerSoln.SolutionAreaId,
                                 OrgId = partnerSoln.OrgId,
                                 OrgName = org.OrgName,
                                 SolutionPlayName = partnerSoln.SolutionPlayName,
                                 SolutionPlayDescription = partnerSoln.SolutionPlayDescription,
                                 SolutionPlayOrgWebsite = partnerSoln.SolutionPlayOrgWebsite,
                                 MarketplaceLink = partnerSoln.MarketplaceLink,
                                 SpecialOfferLink = partnerSoln.SpecialOfferLink,
                                 LogoFileLink = org.logoFileLink,
                                 SolutionStatusId = partnerSoln.SolutionStatusId,
                                 IsPublished = partnerSoln.IsPublished,
                                 RowChangedBy = partnerSoln.RowChangedBy,
                                 DisplayLabel = ss.DisplayLabel,
                                 Image_Thumb = partnerSoln.Image_Thumb,
                                 Image_Main = partnerSoln.Image_Main,
                                 Image_Mobile = partnerSoln.Image_Mobile,
                             }).FirstOrDefault();
            if (pSolution != null)
            {
                pSolution.SolutionAreaWithSolutionPlay = (from partnerSoln in _context.PartnerSolutionPlay
                                                          join solArea in _context.SolutionAreas
                                                          on partnerSoln.SolutionAreaId equals solArea.SolutionAreaId
                                                          join playByPlay in _context.PartnerSolutionPlayByPlay
                                                          on partnerSoln.PartnerSolutionPlayId equals playByPlay.PartnerSolutionPlayId
                                                          join solutionPlay in _context.SolutionPlay
                                                          on playByPlay.SolutionPlayId equals solutionPlay.SolutionPlayId
                                                          where partnerSoln.PartnerSolutionPlaySlug == slug
                                                          select new SolutionAreaWithSolutionPlay
                                                          {
                                                              SolutionAreaId = solArea.SolutionAreaId,
                                                              SolutionAreaName = solArea.SolutionAreaName,
                                                              SolutionAreaDescription = solArea.SolAreaDescription,
                                                              SolutionPlayId = solutionPlay.SolutionPlayId,
                                                              SolutionPlayLabel = solutionPlay.SolutionPlayLabel,
                                                              SolutionPlayName = solutionPlay.SolutionPlayName,
                                                              SolutionAreaSlug = solArea.SolutionAreaSlug,
                                                              SolutionPlayThemeSlug = solutionPlay.SolutionPlayThemeSlug,
                                                              PartnerSolutionPlayName = partnerSoln.SolutionPlayName,
                                                          }).FirstOrDefault();
                /*pSolution.PartnerSolutionPlayAvailableGeo = _context.PartnerSolutionPlayAvailableGeo.FromSqlRaw
                       (
                           "select * from PartnerSolutionPlayAvailableGeo where partnerSolutionPlayId = @partnerSolutionPlayId",
                           new SqlParameter("@partnerSolutionPlayId", pSolution.PartnerSolutionPlayId)
                       ).ToList();*/
                pSolution.Geos = (from pag in _context.PartnerSolutionPlayAvailableGeo
                                  join geo in _context.Geos
                                  on pag.GeoId equals geo.GeoId
                                  where pag.PartnerSolutionPlayId == pSolution.PartnerSolutionPlayId
                                  select new PartnerSolutionPlayGeos
                                  {
                                      GeoId = pag.GeoId,
                                      Geoname = geo.Geoname
                                  }).ToList();

                var PartnerSolutionPlays = (from solnArea in _context.PartnerSolutionPlayByPlay
                                            where solnArea.PartnerSolutionPlayId == pSolution.PartnerSolutionPlayId
                                            select new PartnerSolutionPlayByPlayDTO
                                            {
                                                PartnerSolutionPlayByPlayId = solnArea.PartnerSolutionPlayByPlayId,
                                                SolutionPlayId = solnArea.SolutionPlayId,
                                                PlaySolutionDescription = solnArea.PlaySolutionDescription
                                            }).ToList();



                var getDetailsAry = new List<PartnerSolutionPlayByPlayDTO>();
                for (int i = 0; i < PartnerSolutionPlays.Count(); i++)
                {
                    var PartnerSolutionPlayResourceLinks = (from psl in _context.PartnerSolutionPlayResourceLink
                                                            join rl in _context.ResourceLinks
                                                        on psl.ResourceLinkId equals rl.ResourceLinkId
                                                            where psl.PartnerSolutionPlayByPlayId == PartnerSolutionPlays[i].PartnerSolutionPlayByPlayId
                                                            select new PartnerSolutionPlayResourceLinkDTO
                                                            {
                                                                resourceLinkId = psl.ResourceLinkId,
                                                                resourceLinkTitle = psl.ResourceLinkTitle,
                                                                resourceLinkUrl = psl.ResourceLinkUrl,
                                                                //resourceLinkOverview = psl.ResourceLinkOverview,
                                                                resourceLinkName = rl.ResourceLinkName
                                                            }).ToList();

                    PartnerSolutionPlays[i].PartnerSolutionPlayResourceLinks = PartnerSolutionPlayResourceLinks;
                    getDetailsAry.Add(PartnerSolutionPlays[i]);
                }
                pSolution.PartnerSolutionPlays = getDetailsAry;
                return pSolution;
            }
            else
            {
                return pSolution;
            }
        }
        [HttpGet]
        [Route("GetSolutionPlayOrgViewId")]
        public async Task<IEnumerable<PartnerSolutionPlayViewDTO>> GetSolutionPlayOrgViewId(Guid OrgId)
        {

            var pSolution = (from partnerSoln in _context.PartnerSolutionPlay
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
                             where partnerSoln.OrgId == OrgId
                             where ss.SolutionStatus == "Approved"
                             where partnerSoln.IsPublished == 1
                             orderby partnerSoln.SolutionPlayName
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
                                 show = false,
                                 SolutionPlayId = playByPlay.SolutionPlayId,
                                 SolutionPlayDescription = partnerSoln.SolutionPlayDescription
                             }).ToList();
            foreach (var soln in pSolution)
            {
                var solnPlay = (from solnPlays in _context.SolutionPlay
                                join status in _context.SolutionStatusType
                                on solnPlays.SolutionStatusId equals status.SolutionStatusId
                                where status.SolutionStatus == "Approved"
                                where solnPlays.IsPublished == 1
                                select new SolutionPlay
                                {
                                    SolutionPlayId = solnPlays.SolutionPlayId
                                }).FirstOrDefault();
                if (solnPlay != null)
                {
                    soln.show = true;
                }
            }
            return pSolution;
        }
        [HttpGet]
        [Route("GetPartnerSolutionOrgViewId")]
        public async Task<IEnumerable<GetPartnerSolutionDTO>> GetPartnerSolutionOrgViewId(Guid OrgId)
        {
            var pSolution = (from partnerSoln in _context.PartnerSolution
                             join partnerSolnArea in _context.PartnerSolutionByArea
                             on partnerSoln.PartnerSolutionId equals partnerSolnArea.PartnerSolutionId
                             join ss in _context.SolutionStatusType
                             on partnerSoln.SolutionStatusId equals ss.SolutionStatusId
                             join org in _context.Organization
                             on partnerSoln.OrganizationId equals org.OrgId
                             where ss.SolutionStatus == "Approved"
                             where partnerSoln.IsPublished == 1
                             where partnerSoln.OrganizationId == OrgId
                             select new GetPartnerSolutionDTO
                             {
                                 PartnerSolutionId = partnerSoln.PartnerSolutionId,
                                 IndustryId = partnerSoln.IndustryId,
                                 SubIndustryId = partnerSoln.SubIndustryId,
                                 SolutionDescription = partnerSoln.SolutionDescription,
                                 LogoFileLink = partnerSoln.LogoFileLink,
                                 SolutionName = partnerSoln.SolutionName,
                                 OrgName = org.OrgName,
                                 PartnerSolutionSlug = partnerSoln.PartnerSolutionSlug,
                                 show = false
                             }).ToList();
            foreach (var soln in pSolution)
            {
                var theme = (from themes in _context.IndustryTheme
                             join status in _context.SolutionStatusType
                             on themes.SolutionStatusId equals status.SolutionStatusId
                             where status.SolutionStatus == "Approved"
                             where themes.IsPublished == "1"
                             select new IndustryTheme
                             {
                                 IndustryThemeId = themes.IndustryThemeId
                             }).FirstOrDefault();
                if (theme != null)
                    soln.show = true;
            }
            return pSolution;
        }
        [HttpGet]
        [Route("ThemeViewId")]
        public IndustryTheme GetThemeViewId(string industryId, string subIndustryId)
        {
            var theme = (from ind in _context.Industries
                         join subInd in _context.SubIndustries
                         on ind.IndustryId equals subInd.IndustryId
                         join themes in _context.IndustryTheme
                         on subInd.SubIndustryId equals themes.SubIndustryId
                         join status in _context.SolutionStatusType
                         on themes.SolutionStatusId equals status.SolutionStatusId
                         where status.SolutionStatus == "Approved"
                         where themes.IsPublished == "1"
                         where ind.IndustrySlug == industryId
                         where subInd.SubIndustrySlug == subIndustryId
                         select new IndustryTheme
                         {
                             IndustryThemeId = themes.IndustryThemeId,
                             IndustryThemeSlug = themes.IndustryThemeSlug,
                             SubIndustryId = themes.SubIndustryId,
                             IndustryId = themes.IndustryId
                         }).FirstOrDefault();
            return theme;
        }
        [HttpGet]
        [Route("GetUserEmailEncryptValue")]
        public PartnerUser GetUserEmailEncryptValue(string emailAddress)
        {
            var utilities = new Utilities(configuration);
            string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
            string encEmail = utilities.EncryptString(encKey, emailAddress);
            var userid = _context.PartnerUser.Where(t => t.PartnerEmail == encEmail).FirstOrDefault();
            return userid;
        }

        [HttpGet]
        [Route("GetPartnerSolutions")]
        public async Task<IEnumerable<string>> GetPartnerSolutions(Guid industryId, Guid subIndustryId)
        {
           var result =await _industryBuilder.GetPartnerSolution(industryId, subIndustryId);
            return result;
        }
        [HttpGet]
        [Route("DeleteSubIndustrySolutionsActionForPublished")]
        public async Task<ResponseDTO> DeleteSubIndustrySolutionsActionForPublished()
        {
            var result = _context.PartnerSolution.ToList();
            var response = new ResponseDTO();
            int cnt = 0;
            foreach (var res in result)
            {
                var subIndCheck = _context.SubIndustries.Where(t => t.SubIndustryId == res.SubIndustryId).FirstOrDefault();
                if(subIndCheck == null)
                {

                    res.IsPublished = 5;
                    res.RowChangedDate = DateTime.UtcNow;
                    _context.PartnerSolution.Update(res);
                    _context.SaveChanges();
                    cnt++;
                }
            }
            if(cnt > 0)
            {
                response.Response = true;
                response.Message = "Deleted Successfully";
            }
            else
            {
                response.Message = "Not Deleted";
                response.Response = false;
            }
            return response;
               
        }
        [HttpGet]
        [Route("UpdateCreatedDateForParterSolution")]
        public async Task<ResponseDTO> UpdateCreatedDateForParterSolution()
        {
            var result = _context.PartnerSolution.ToList();
            var response = new ResponseDTO();
            int cnt = 0;
            foreach (var res in result)
            {   
                res.RowCreatedDate = res.RowChangedDate;
                _context.PartnerSolution.Update(res);
                _context.SaveChanges();
                cnt++;
                
            }
            if (cnt > 0)
            {
                response.Response = true;
                response.Message = "Updated Successfully";
            }
            else
            {
                response.Message = "Not Updated";
                response.Response = false;
            }
            return response;

        }
    }
}
