using Azure;
using Azure.Storage.Blobs;
using Knack.API.Common;
using Knack.API.Data;
using Knack.API.Interfaces;
using Knack.API.Models;
using Knack.DBEntities;
using Knack.Entities.DBEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Data;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Resources;

namespace Knack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserBuilder _userBuilder;
        private readonly KnackContext _context;
        private readonly IConfiguration configuration;
        public AdminController(KnackContext context, IConfiguration iConfig, IUserBuilder userBuilder)
        {
            _context = context;
            configuration = iConfig;
            _userBuilder = userBuilder;
        }
        [HttpGet]
        [Route("GetAllOrgs")]
        public async Task<IEnumerable<AdminOrganizationDTO>> GetAllOrganizations()
        {            
            var Orgs = (from org in _context.Organization
                          join ps in _context.PartnerSolution
                          on org.OrgId equals ps.OrganizationId into solutions
                          orderby org.OrgName
                          select new AdminOrganizationDTO
                          {
                              OrgId = org.OrgId,
                              OrgName = org.OrgName,
                              OrgDescription = org.OrgDescription,
                              OrgWebsite = org.orgWebsite,
                              LogoFileLink = org.logoFileLink,
                              RowChangedBy = org.RowChangedBy,
                              RowChangedDate = org.RowChangedDate,
                              Status = org.Status,
                              TotalSolution = solutions.Count().ToString()
                          }).ToList();
            /*
            List<AdminOrganizationDTO> Orgs = new List<AdminOrganizationDTO>();
            using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT O.orgId,O.orgName,O.orgDescription,O.orgWebsite,O.logoFileLink,O.rowChangedBy,O.rowChangedDate,O.status,Count(DISTINCT ps.partnerSolutionId) AS totalSolution FROM organization O LEFT JOIN partnerSolution ps ON O.orgId = ps.OrganizationId GROUP BY O.orgId,O.orgName,O.orgWebsite,O.logoFileLink,O.rowChangedBy,O.rowChangedDate,O.status,O.orgDescription Order by o.OrgName";
                command.CommandType = CommandType.Text;
                this._context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        AdminOrganizationDTO org = new AdminOrganizationDTO();
                        org.OrgId = Guid.Parse(result["orgId"].ToString());
                        org.OrgName = result["orgName"].ToString();
                        org.OrgDescription = result["orgDescription"].ToString();
                        org.OrgWebsite = result["orgWebsite"].ToString();
                        org.LogoFileLink = result["logoFileLink"].ToString(); ;
                        org.RowChangedBy = result["rowChangedBy"].ToString();
                        org.RowChangedDate = (DateTime?)result["rowChangedDate"];
                        org.Status = result["status"].ToString();
                        org.TotalSolution = result["totalSolution"].ToString();
                        Orgs.Add(org);
                    }
                }
            }*/
            return Orgs;
        }

        [HttpPost]
        [Route("UpdateOrganization")]
        public async Task<ResponseDTO> UpdateOrganization(Organization updatedOrgDetails)
        {
            var orgchck = _context.Organization.Where(org => org.OrgName == updatedOrgDetails.OrgName).Where(org => org.OrgId != updatedOrgDetails.OrgId).FirstOrDefault();
            var response = new ResponseDTO();
            if (orgchck == null)
            {
                var org = _context.Organization.Where(org => org.OrgId == updatedOrgDetails.OrgId).FirstOrDefault();
                org.OrgName = updatedOrgDetails.OrgName;
                org.OrgDescription = updatedOrgDetails.OrgDescription;
                org.RowChangedDate = DateTime.UtcNow;
                org.RowChangedBy = updatedOrgDetails.RowChangedBy;
                org.orgWebsite = updatedOrgDetails.orgWebsite;
                org.logoFileLink = updatedOrgDetails.logoFileLink;

                _context.Organization.Update(org);
                await _context.SaveChangesAsync();
                response.Response = true;
                response.Message = "Updated Successfully";
            }
            else
            {
                response.Response = false;
                response.Message = "Organization name already exists";
            }
            return response;
        }
        [HttpPost]
        [Route("DeleteOrganization")]
        public async Task<ResponseDTO> DeleteOrganization(Organization updatedOrgDetails)
        {
            var org = _context.Organization.Where(org => org.OrgId == updatedOrgDetails.OrgId).FirstOrDefault();
            var response = new ResponseDTO();
            if (org != null)
            {
                var pSolution = _context.PartnerSolution.Where(u => u.OrganizationId == updatedOrgDetails.OrgId).FirstOrDefault();
                if (pSolution != null)
                {
                    response.Response = false;
                    response.Message = "We cant delete! Solutions already added in these organization";
                    return response;
                }
                else
                {
                    _context.Remove(org);
                    await _context.SaveChangesAsync();
                    var users = _context.PartnerUser.Where(org => org.OrgId == updatedOrgDetails.OrgId).ToList();
                    if (users?.Count > 0)
                    {
                        foreach (var user in users)
                        {
                            _context.Remove(user);
                            _context.SaveChanges();
                        }
                    }
                    response.Response = true;
                    response.Message = "Deleted Successfully";
                    return response;
                }
            }
            else
            {
                response.Response = false;
                response.Message = "Data not exists.";
                return response;
            }
        }
        [HttpPost]
        [Route("AddOrganization")]
        public async Task<ResponseDTO> AddOrganization(Organization updatedDetails)
        {
            var org = _context.Organization.Where(org => org.OrgName == updatedDetails.OrgName).FirstOrDefault();
            var response = new ResponseDTO();
            if (org == null)
            {
                var res = new Organization
                {
                    OrgId = Guid.NewGuid(),
                    OrgName = updatedDetails.OrgName,
                    OrgDescription = updatedDetails.OrgDescription,
                    RowChangedDate = DateTime.UtcNow,
                    orgWebsite = updatedDetails.orgWebsite,
                    logoFileLink = updatedDetails.logoFileLink,
                    Status = "Created"
                };
                _context.Organization.Add(res);
                await _context.SaveChangesAsync();

                response.Response = true;
                response.Message = "Added Successfully";
            }
            else
            {
                response.Response = false;
                response.Message = "Organization name already exists";
            }
            return response;
        }
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IEnumerable<PartnerUserAdminDTO>> GetAllUsers()
        {
            string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
            var utilities = new Utilities(configuration);
            var pUsers = (from user in _context.PartnerUser
                        join org in _context.Organization
                        on user.OrgId equals org.OrgId into orgs
                        from orgLists in orgs.DefaultIfEmpty()
                        where user.LastName != "Deleted User"
                          orderby orgLists.OrgName
                        select new PartnerUserAdminDTO
                        {
                           PartnerUserId = user.PartnerUserId,
                           OrgId = orgLists.OrgId,
                           OrgName = orgLists.OrgName,
                           FirstName = utilities.DecryptString(encKey, user.FirstName),
                           LastName = utilities.DecryptString(encKey, user.LastName),
                           PartnerEmail = utilities.DecryptString(encKey, user.PartnerEmail),
                           PartnerTitle = user.PartnerTitle,
                           UserType = user.UserType,
                           Status = user.Status
                        }).ToList();
            /*List<PartnerUserAdminDTO> pUsers = new List<PartnerUserAdminDTO>();
            using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                var utilities = new Utilities(configuration);
                command.CommandText = "select distinct(pu.partnerUserId), pu.status, pu.partnerEmail, " +
                    "pu.UserType, pu.orgId, pu.firstName, pu.lastName, " +
                    "pu.partnerTitle, pu.UserType, o.orgName from partnerUser as pu " +
                    "left join organization as o ON pu.orgId = o.orgId where lastname <>'Deleted User'";
                command.CommandType = CommandType.Text;
                this._context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        PartnerUserAdminDTO user = new PartnerUserAdminDTO();
                        string decEmail = utilities.DecryptString(encKey, result["partnerEmail"].ToString());
                        user.PartnerUserId = Guid.Parse(result["partnerUserId"].ToString());
                        user.OrgId = Guid.Parse(result["orgId"].ToString());
                        user.FirstName = utilities.DecryptString(encKey, result["firstName"].ToString());
                        user.LastName = utilities.DecryptString(encKey, result["lastName"].ToString());
                        user.PartnerEmail = decEmail;
                        user.PartnerTitle = result["partnerTitle"].ToString();
                        user.UserType = result["UserType"].ToString();
                        user.OrgName = result["orgName"].ToString();
                        user.Status = result["status"].ToString();
                        pUsers.Add(user);
                    }
                }
            }*/
            return pUsers;
        }
        [HttpPost]
        [Route("UpdateUser")]
        public async Task<IEnumerable<PartnerUser>> UpdateUser(PartnerUser updatedDetails)
        {
            var user = _context.PartnerUser.Where(u => u.PartnerUserId == updatedDetails.PartnerUserId).FirstOrDefault();
            var utilities = new Utilities(configuration);
            string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
            user.FirstName = utilities.EncryptString(encKey, updatedDetails.FirstName);
            user.LastName = utilities.EncryptString(encKey, updatedDetails.LastName);
            user.OrgId = updatedDetails.OrgId;
            user.RowChangedDate = DateTime.UtcNow;
            //user.RowChangedBy = updatedDetails.RowChangedBy;
            user.PartnerTitle = updatedDetails.PartnerTitle;
            user.UserType = updatedDetails.UserType;
            _context.PartnerUser.Update(user);
            await _context.SaveChangesAsync();

            return await _context.PartnerUser.ToListAsync();
        }
        [HttpPost]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(PartnerUserDTO partnerUser)
        {
            var response = await _userBuilder.DeletePartnerUser(partnerUser.PartnerUserId);
            if (response == null)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity,
                "Error occured while deleting the user. Please contact the admin for more details");
            }

            return Ok(new ResponseDTO { Message = response, Response = true });
        }
        [HttpPost]
        [Route("InviteUser")]
        public async Task<ResponseDTO> InviteUser(UserInviteDTO updatedDetails)
        {
            var utilities = new Utilities(configuration);
            var response = new ResponseDTO();
            string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
            string encEmail = utilities.EncryptString(encKey, updatedDetails.UserInviteEmail);
            var partneruser = _context.PartnerUser.Where(u => u.PartnerEmail == encEmail).FirstOrDefault();
            var user = _context.UserInvite.Where(u => u.UserInviteEmail == encEmail).FirstOrDefault();
            if (user == null && partneruser == null)
            {
                var res = new UserInvite
                {
                    UserInviteId = Guid.NewGuid(),
                    FirstName = utilities.EncryptString(encKey, updatedDetails.FirstName),
                    LastName = utilities.EncryptString(encKey, updatedDetails.LastName),
                    RowChangedDate = DateTime.UtcNow,
                    UserInviteEmail = encEmail,
                    Status = "Invited"
                };
                _context.UserInvite.Add(res);
                await _context.SaveChangesAsync();
                var recepientDetails = new RecipientOTPMailDTO
                {
                    RecipientEmail = updatedDetails.UserInviteEmail,
                    RecipientName = updatedDetails.UserInviteEmail,
                    Subject = "Industry Solutions Directory Invitation Email",
                    Body = $"<div style=\"font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2\">\r\n  <div style=\"margin:50px auto;width:70%;padding:20px 0\">\r\n    <div style=\"border-bottom:1px solid #eee;text-align: center;padding-bottom: 20px;\" >\r\n      <img src=\"https://partner.microsoftindustryinsights.com/assets/images/Microsoft_Logo_Color@2x.png\" alt=\"Microsoft Color Logo\" style=\"width: 150px;\">\r\n    </div>\r\n    <p style=\"font-size:1.1em\">Hi,</p>\r\n    <p>You were invited to our Industry Solutions Directory. Use the following Link to complete your Sign In procedures.</p>\r\n    <h2 style=\"background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;\"><a href=\"{updatedDetails.LoginUrl}\" style=\"color:#fff;\">Register Now</a></h2>\r\n    <p style=\"font-size:0.9em;\">Regards,<br />Industry Solutions Directory</p>\r\n    <hr style=\"border:none;border-top:1px solid #eee\" />    \r\n  </div>\r\n</div>",
                    smtpServer = configuration.GetSection("EmailSettings").GetSection("SmtpServer").Value,
                    smtpUsername = configuration.GetSection("EmailSettings").GetSection("UserName").Value,
                    smtpPassword = configuration.GetSection("EmailSettings").GetSection("Pwd").Value
                };
                //var mailresponse = new ResponseDTO();

                bool mailresponse = utilities.SendMail(recepientDetails.RecipientEmail,
                    recepientDetails.Subject, recepientDetails.Body);
                if (mailresponse)
                {
                    response.Response = true;
                    response.Message = "User Invited Successfully";
                }
                else
                {
                    response.Response = false;
                    response.Message = "Mail not send. Please contact administrator";
                }
            }
            else if (partneruser != null)
            {
                response.Response = false;
                response.Message = "User already registered";
            }
            else
            {
                response.Response = false;
                response.Message = "User already invited";
            }
            return response;
        }
        [HttpGet]
        [Route("GetAllSolutions")]
        public async Task<IEnumerable<AdminPartnerSolutionDTO>> GetAllSolutions()
        {
            var inds = (from ind in _context.Industries
                        join subInd in _context.SubIndustries
                        on ind.IndustryId equals subInd.IndustryId
                        select new AdminPartnerSolutionDTO
                        {
                            IndustryId = subInd.IndustryId,
                            SubIndustryId = subInd.SubIndustryId,
                            IndustryName = ind.IndustryName,
                            SubIndustryName = subInd.SubIndustryName,
                        }).ToList();
            foreach (var ind in inds)
            {
                using (var command = this._context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "select Count(partnerSolutionId) as NoofSolutions  from partnerSolution where SubIndustryId = @subIndustryId";
                    command.CommandType = CommandType.Text;
                    SqlParameter param = new SqlParameter("@subIndustryId", ind.SubIndustryId);
                    command.Parameters.Add(param);
                    this._context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            int NoofSolutions = (int)result["NoofSolutions"];
                            ind.NoofSolutions = NoofSolutions;
                        }
                    }
                }
                using (var command = this._context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "select Count(partnerSolutionId) as NoofApproved  from partnerSolution as ps join SolutionStatus as ss on ps.SolutionStatusId = ss.SolutionStatusId where ss.SolutionStatus = 'Approved' and ps.SubIndustryId = @subIndustryId";
                    command.CommandType = CommandType.Text;
                    SqlParameter param = new SqlParameter("@subIndustryId", ind.SubIndustryId);
                    command.Parameters.Add(param);
                    this._context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            int NoofApproved = (int)result["NoofApproved"];
                            ind.NoofApproved = NoofApproved;
                        }
                    }
                }
                using (var command = this._context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "select Count(partnerSolutionId) as NoofPublish  from partnerSolution as ps join SolutionStatus as ss on ps.SolutionStatusId = ss.SolutionStatusId where ss.SolutionStatus = 'Approved' and ps.IsPublished = 1 and ps.SubIndustryId = @subIndustryId";
                    command.CommandType = CommandType.Text;
                    SqlParameter param = new SqlParameter("@subIndustryId", ind.SubIndustryId);
                    command.Parameters.Add(param);
                    this._context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            int NoofPublish = (int)result["NoofPublish"];
                            ind.NoofPublish = NoofPublish;
                        }
                    }
                }
            }
            return inds;
        }
        [HttpGet]
        [Route("GetAllGeos")]
        public async Task<IEnumerable<Geo>> GetAllGeos()
        {
            var Geos = await _context.Geos.OrderBy(g=>g.Geoname).ToListAsync();
            return Geos;
        }
        [HttpPost]
        [Route("UpdateGeo")]
        public async Task<IEnumerable<Geo>> UpdateGeo(Geo updatedDetails)
        {
            var geo = _context.Geos.Where(u => u.GeoId == updatedDetails.GeoId).FirstOrDefault();
            geo.Geoname = updatedDetails.Geoname;
            geo.Locale = updatedDetails.Locale;
            geo.RowChangedDate = DateTime.UtcNow;
            //user.RowChangedBy = updatedDetails.RowChangedBy;
            geo.Geodescription = updatedDetails.Geodescription;
            geo.DisplayOrder = updatedDetails.DisplayOrder;
            _context.Geos.Update(geo);
            await _context.SaveChangesAsync();
            return await _context.Geos.ToListAsync();
        }
        [HttpPost]
        [Route("DeleteGeo")]
        public async Task<ResponseDTO> DeleteGeo(Geo updatedDetails)
        {
            var geo = _context.Geos.Where(u => u.GeoId == updatedDetails.GeoId).FirstOrDefault();
            var response = new ResponseDTO();
            if (geo != null)
            {
                var partnerGeo = _context.PartnerSolutionAvailableGeo.Where(u => u.GeoId == updatedDetails.GeoId).FirstOrDefault();
                var solutionPlayGeo = _context.PartnerSolutionPlayAvailableGeo.Where(u => u.GeoId == updatedDetails.GeoId).FirstOrDefault();
                if (partnerGeo != null || solutionPlayGeo != null)
                {
                    response.Response = false;
                    response.Message = "We cant delete! Geo already added in solutions";
                    return response;
                }
                else
                {
                    _context.Remove(geo);
                    await _context.SaveChangesAsync();
                    response.Response = true;
                    response.Message = "Deleted Successfully";
                    return response;
                }
            }
            else
            {
                response.Response = false;
                response.Message = "Data not exists.";
                return response;
            }
        }
        [HttpPost]
        [Route("AddGeo")]
        public async Task<IEnumerable<Geo>> AddGeo(Geo updatedDetails)
        {
            var geo = new Geo
            {
                GeoId = Guid.NewGuid(),
                Geoname = updatedDetails.Geoname,
                Locale = updatedDetails.Locale,
                RowChangedDate = DateTime.UtcNow,
                Geodescription = updatedDetails.Geodescription,
                DisplayOrder = updatedDetails.DisplayOrder,
                Status = "Active",
            };
            _context.Geos.Add(geo);
            await _context.SaveChangesAsync();
            return await _context.Geos.ToListAsync();
        }
        [HttpGet]
        [Route("GetAllSolutionArea")]
        public async Task<IEnumerable<SolutionArea>> GetAllSolutionArea()
        {
            var solutionAreas = await _context.SolutionAreas.OrderBy(s=>s.SolutionAreaName).ToListAsync();
            return solutionAreas;
        }
        [HttpPost]
        [Route("UpdateSolutionArea")]
        public async Task<IEnumerable<SolutionArea>> UpdateSolutionArea(SolutionArea updatedDetails)
        {
            string slug = Regex.Replace(updatedDetails.SolutionAreaName, @"<[^>]+>|&nbsp;", "").Trim();
            slug = Regex.Replace(slug, @"\s{2,}", " ");
            slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
            slug = Regex.Replace(slug, @" ", "-");
            slug = slug.ToLower();
            Boolean slugCheck = true;
            string slugTmp = slug;
            int j = 1;
            while (slugCheck)
            {
                var resCheck = _context.SolutionAreas.Where(t => t.SolutionAreaSlug == slug).Where(u => u.SolutionAreaId != updatedDetails.SolutionAreaId).FirstOrDefault();
                if (resCheck == null)
                    slugCheck = false;
                else
                {
                    slug = slugTmp + "-" + j;
                    j++;
                }

            }
            var solutionArea = _context.SolutionAreas.Where(u => u.SolutionAreaId == updatedDetails.SolutionAreaId).FirstOrDefault();
            solutionArea.SolutionAreaName = updatedDetails.SolutionAreaName;
            solutionArea.SolAreaDescription = updatedDetails.SolAreaDescription;
            solutionArea.RowChangedDate = DateTime.UtcNow;
            solutionArea.Status = updatedDetails.Status;
            //user.RowChangedBy = updatedDetails.RowChangedBy;
            solutionArea.DisplayOrder = updatedDetails.DisplayOrder;
            solutionArea.Image_Thumb = updatedDetails.Image_Thumb;
            solutionArea.Image_Main = updatedDetails.Image_Main;
            solutionArea.Image_Mobile = updatedDetails.Image_Mobile;
            solutionArea.SolutionAreaSlug = slug;
            _context.SolutionAreas.Update(solutionArea);
            await _context.SaveChangesAsync();
            return await _context.SolutionAreas.ToListAsync();
        }
        [HttpPost]
        [Route("DeleteSolutionArea")]
        public async Task<ResponseDTO> DeleteSolutionArea(SolutionArea updatedDetails)
        {
            var solutionArea = _context.SolutionAreas.Where(u => u.SolutionAreaId == updatedDetails.SolutionAreaId).FirstOrDefault();
            var response = new ResponseDTO();
            if (solutionArea != null)
            {
                var themeSolutionArea = _context.IndustryThemeBySolutionArea.Where(u => u.SolutionAreaId == updatedDetails.SolutionAreaId).FirstOrDefault();
                var solutionPlayArea = _context.PartnerSolutionPlay.Where(u => u.SolutionAreaId == updatedDetails.SolutionAreaId).FirstOrDefault();
                if (themeSolutionArea != null || solutionPlayArea != null)
                {
                    response.Response = false;
                    response.Message = "We cant delete! Solutions already added in industry theme and solutions";
                    return response;
                }
                else
                {
                    _context.Remove(solutionArea);
                    await _context.SaveChangesAsync();
                    response.Response = true;
                    response.Message = "Deleted Successfully";
                    return response;
                }
            }
            else
            {
                response.Response = false;
                response.Message = "Data not exists.";
                return response;
            }

        }
        [HttpPost]
        [Route("AddSolutionArea")]
        public async Task<IEnumerable<SolutionArea>> AddSolutionArea(SolutionArea updatedDetails)
        {
            string slug = Regex.Replace(updatedDetails.SolutionAreaName, @"<[^>]+>|&nbsp;", "").Trim();
            slug = Regex.Replace(slug, @"\s{2,}", " ");
            slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
            slug = Regex.Replace(slug, @" ", "-");
            slug = slug.ToLower();
            Boolean slugCheck = true;
            string slugTmp = slug;
            int j = 1;
            while (slugCheck)
            {
                var resCheck = _context.SolutionAreas.Where(t => t.SolutionAreaSlug == slug).FirstOrDefault();
                if (resCheck == null)
                    slugCheck = false;
                else
                {
                    slug = slugTmp + "-" + j;
                    j++;
                }

            }

            var solutionArea = new SolutionArea
            {
                SolutionAreaId = Guid.NewGuid(),
                SolutionAreaName = updatedDetails.SolutionAreaName,
                SolAreaDescription = updatedDetails.SolAreaDescription,
                RowChangedDate = DateTime.UtcNow,
                DisplayOrder = updatedDetails.DisplayOrder,
                Image_Thumb = updatedDetails.Image_Thumb,
                Image_Main = updatedDetails.Image_Main,
                Image_Mobile = updatedDetails.Image_Mobile,
                Status = "Active",
                SolutionAreaSlug = slug
            };
            _context.SolutionAreas.Add(solutionArea);
            await _context.SaveChangesAsync();
            return await _context.SolutionAreas.ToListAsync();
        }
        [HttpGet]
        [Route("GetAllResourceLink")]
        public async Task<IEnumerable<ResourceLink>> GetAllResourceLink()
        {
            var ResourceLinks = await _context.ResourceLinks.OrderBy(r=>r.ResourceLinkName).ToListAsync();
            return ResourceLinks;
        }
        [HttpPost]
        [Route("UpdateResourceLink")]
        public async Task<IEnumerable<ResourceLink>> UpdateResourceLink(ResourceLink updatedDetails)
        {
            var resourcelink = _context.ResourceLinks.Where(u => u.ResourceLinkId == updatedDetails.ResourceLinkId).FirstOrDefault();
            resourcelink.ResourceLinkName = updatedDetails.ResourceLinkName;
            resourcelink.ResourceLinkDescription = updatedDetails.ResourceLinkDescription;
            resourcelink.RowChangedDate = DateTime.UtcNow;
            //user.RowChangedBy = updatedDetails.RowChangedBy;
            resourcelink.DisplayOrder = updatedDetails.DisplayOrder;
            _context.ResourceLinks.Update(resourcelink);
            await _context.SaveChangesAsync();
            return await _context.ResourceLinks.ToListAsync();
        }
        [HttpPost]
        [Route("DeleteResourceLink")]
        public async Task<ResponseDTO> DeleteResourceLink(ResourceLink updatedDetails)
        {
            var resourcelink = _context.ResourceLinks.Where(u => u.ResourceLinkId == updatedDetails.ResourceLinkId).FirstOrDefault();
            var response = new ResponseDTO();
            if (resourcelink != null)
            {
                var indResource = _context.IndustryResourceLink.Where(u => u.ResourceLinkId == updatedDetails.ResourceLinkId).FirstOrDefault();
                var partnerSolutionResource = _context.PartnerSolutionResourceLink.Where(u => u.ResourceLinkId == updatedDetails.ResourceLinkId).FirstOrDefault();
                var partnerSolutionPlayResource = _context.PartnerSolutionPlayResourceLink.Where(u => u.ResourceLinkId == updatedDetails.ResourceLinkId).FirstOrDefault();
                if (indResource != null || partnerSolutionResource != null || partnerSolutionPlayResource != null)
                {
                    response.Response = false;
                    response.Message = "We cant delete! Solutions already added in industry theme and solutions";
                    return response;
                }
                else
                {
                    _context.Remove(resourcelink);
                    await _context.SaveChangesAsync();
                    response.Response = true;
                    response.Message = "Deleted Successfully";
                    return response;
                }
            }
            else
            {
                response.Response = false;
                response.Message = "Data not exists.";
                return response;
            }
        }
        [HttpPost]
        [Route("AddResourceLink")]
        public async Task<IEnumerable<ResourceLink>> AddResourceLink(ResourceLink updatedDetails)
        {
            var resourcelink = new ResourceLink
            {
                ResourceLinkId = Guid.NewGuid(),
                ResourceLinkName = updatedDetails.ResourceLinkName,
                ResourceLinkDescription = updatedDetails.ResourceLinkDescription,
                RowChangedDate = DateTime.UtcNow,
                DisplayOrder = updatedDetails.DisplayOrder,
                Status = "Active",
            };
            _context.ResourceLinks.Add(resourcelink);
            await _context.SaveChangesAsync();
            return await _context.ResourceLinks.ToListAsync();
        }
        [HttpGet]
        [Route("GetAllSolutionStatus")]
        public async Task<IEnumerable<SolutionStatusType>> GetAllSolutionStatus()
        {
            var res = await _context.SolutionStatusType.ToListAsync();
            return res;
        }
        [HttpPost]
        [Route("UpdateSolutionStatus")]
        public async Task<IEnumerable<SolutionStatusType>> UpdateSolutionStatus(SolutionStatusType updatedDetails)
        {
            var res = _context.SolutionStatusType.Where(u => u.SolutionStatusId == updatedDetails.SolutionStatusId).FirstOrDefault();
            res.SolutionStatus = updatedDetails.SolutionStatus;
            res.DisplayLabel = updatedDetails.DisplayLabel;
            _context.SolutionStatusType.Update(res);
            await _context.SaveChangesAsync();
            return await _context.SolutionStatusType.ToListAsync();
        }
        [HttpPost]
        [Route("AddSolutionStatus")]
        public async Task<IEnumerable<SolutionStatusType>> AddSolutionStatus(SolutionStatusType updatedDetails)
        {
            var res = new SolutionStatusType
            {
                SolutionStatusId = Guid.NewGuid(),
                SolutionStatus = updatedDetails.SolutionStatus,
                DisplayLabel = updatedDetails.DisplayLabel
            };
            _context.SolutionStatusType.Add(res);
            await _context.SaveChangesAsync();
            return await _context.SolutionStatusType.ToListAsync();
        }
        [HttpPost]
        [Route("DeleteSolutionStatus")]
        public async Task<ResponseDTO> DeleteSolutionStatus(SolutionStatusType updatedDetails)
        {
            var data = _context.SolutionStatusType.Where(u => u.SolutionStatusId == updatedDetails.SolutionStatusId).FirstOrDefault();
            var response = new ResponseDTO();
            if (data != null)
            {
                var ind = _context.IndustryTheme.Where(u => u.SolutionStatusId == updatedDetails.SolutionStatusId).FirstOrDefault();
                var partnerSolution = _context.PartnerSolution.Where(u => u.SolutionStatusId == updatedDetails.SolutionStatusId).FirstOrDefault();
                var partnerSolutionPlay = _context.PartnerSolutionPlay.Where(u => u.SolutionStatusId == updatedDetails.SolutionStatusId).FirstOrDefault();
                var solutionPlay = _context.SolutionPlay.Where(u => u.SolutionStatusId == updatedDetails.SolutionStatusId).FirstOrDefault();
                if (ind != null || partnerSolution != null || partnerSolutionPlay != null || solutionPlay != null)
                {
                    response.Response = false;
                    response.Message = "We cant delete! Status already updated in industry theme and solutions";
                    return response;
                }
                else
                {
                    _context.Remove(data);
                    await _context.SaveChangesAsync();
                    response.Response = true;
                    response.Message = "Deleted Successfully";
                    return response;
                }
            }
            else
            {
                response.Response = false;
                response.Message = "Data not exists.";
                return response;
            }
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
                var blobConnectionString = configuration.GetSection("AzureBlobConnectionString").GetSection("BlobStorage").Value;
                var blobServiceClient = new BlobServiceClient(blobConnectionString);
                // Create a BlobServiceClient object which will be used to create a container client
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
        [Route("GetPartnerSolutionReport")]
        public async Task<ActionResult<IEnumerable<PartnerSolutionReport>>> GetPartnerSolutionReport()
        {
            try
            {
                var query = "select o.orgname,o.orgId,i.industryname,si.SubIndustryName,si.SubIndustryId,ps.solutionName,st.SolutionStatus,st.SolutionStatusId,ps.rowChangedDate from organization o " +
                "left join partnersolution ps on o.orgid=ps.OrganizationId " +
                "inner join industry i on i.industryid=ps.industryid " +
                "inner join subindustry si on si.SubIndustryId=ps.SubIndustryId " +
                "left join solutionstatus st on st.SolutionStatusId=ps.SolutionStatusId " +
                "order by ps.rowChangedDate desc";
                return await _context.Database.SqlQueryRaw<PartnerSolutionReport>(query).ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the reports. Please try again later.", Details = ex.Message });

            }

        }

        /*[HttpGet]
        [Route("DownloadPartnerSolutionReport")]
        public IActionResult DownloadPartnerSolutionReport(String? SubIndustryId, String? OrgId, String? SolutionStatusId)
        {
            try
            {
                var where = " ";
                int andCondition = 0;
                if (SubIndustryId != "" && SubIndustryId != null)
                {
                    Guid SubIndustryIdVal = new Guid(SubIndustryId);
                    if (andCondition == 0)
                        where = " where ";
                    where += "si.SubIndustryId = '" + SubIndustryIdVal + "'";
                    andCondition = 1;
                }
                if (OrgId != "" && OrgId != "null")
                {
                    Guid OrgIdVal = new Guid(OrgId);
                    if (andCondition == 0)
                        where = " where ";
                    if (andCondition == 1)
                        where += " and ";
                    where += "o.orgId = '" + OrgIdVal + "'";
                    andCondition = 1;
                }
                if (SolutionStatusId != "" && SolutionStatusId != "null")
                {
                    Guid SolutionStatusIdVal = new Guid(SolutionStatusId);
                    if (andCondition == 0)
                        where = " where ";
                    if (andCondition == 1)
                        where += " and ";
                    where += "st.SolutionStatusId = '" + SolutionStatusIdVal + "'";
                    andCondition = 1;
                }
                var query = "select o.orgname,o.orgId,i.industryname,si.SubIndustryName,si.SubIndustryId,ps.solutionName,ps.solutionOrgWebsite,ps.marketplaceLink,ps.specialOfferLink,ps.logoFileLink,st.SolutionStatus,st.SolutionStatusId,ps.rowChangedDate, GeoName = STUFF((  SELECT ',' + g.geoname FROM Geo g join partnersolutionavailablegeo pg on g.geoid = pg.geoId WHERE pg.partnerSolutionId = ps.partnerSolutionId      FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') , SolutionAreaName = STUFF((  SELECT ',' + sa.solutionAreaName FROM solutionArea sa join partnerSolutionByArea psa on sa.solutionAreaId = psa.solutionAreaId WHERE psa.partnerSolutionId = ps.partnerSolutionId      FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') from organization o left join partnersolution ps on o.orgid=ps.OrganizationId inner join industry i on i.industryid=ps.industryid inner join subindustry si on si.SubIndustryId=ps.SubIndustryId left join solutionstatus st on st.SolutionStatusId=ps.SolutionStatusId " +
                 where +
                 " order by ps.rowChangedDate desc";
                List<PartnerSolutionReportCSV> lst = _context.Database.SqlQueryRaw<PartnerSolutionReportCSV>(query).ToList();
                var properties = typeof(PartnerSolutionReportCSV).GetProperties();
                var csvContent = new StringBuilder();

                // Write the header row
                foreach (var prop in properties)
                {
                    csvContent.Append(prop.Name).Append(",");
                }
                csvContent.Length--; // Remove the last comma
                csvContent.AppendLine();

                // Write the data rows
                foreach (var item in lst)
                {
                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(item, null);
                        if (value != null)
                        {
                            csvContent.Append(value.ToString().Replace(",", " - ")).Append(",");
                        }
                        else
                        {
                            csvContent.Append(",");
                        }
                    }
                    csvContent.Length--; // Remove the last comma
                    csvContent.AppendLine();
                }

                // Return as byte array
                //Encoding.UTF8.GetBytes(csvContent.ToString());
                return File(Encoding.UTF8.GetBytes(csvContent.ToString()), "text/csv", "Report");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while downloading the reports. Please try again later.", Details = ex.Message });

            }
        }*/
        [HttpGet]
        [Route("DownloadTechnologySolutionReport")]
        public IActionResult DownloadTechnologySolutionReport(String? SolutionAreaId, String? OrgId, String? SolutionStatusId)
        {
            try
            {
                var where = " ";
                int andCondition = 0;
                if (SolutionAreaId != "" && SolutionAreaId != null)
                {
                    Guid SolutionAreaIdVal = new Guid(SolutionAreaId);
                    if (andCondition == 0)
                        where = " where ";
                    where += "sa.SolutionAreaId = '" + SolutionAreaIdVal + "'";
                    andCondition = 1;
                }
                if (OrgId != "" && OrgId != "null")
                {
                    Guid OrgIdVal = new Guid(OrgId);
                    if (andCondition == 0)
                        where = " where ";
                    if (andCondition == 1)
                        where += " and ";
                    where += "o.orgId = '" + OrgIdVal + "'";
                    andCondition = 1;
                }
                if (SolutionStatusId != "" && SolutionStatusId != "null")
                {
                    Guid SolutionStatusIdVal = new Guid(SolutionStatusId);
                    if (andCondition == 0)
                        where = " where ";
                    if (andCondition == 1)
                        where += " and ";
                    where += "st.SolutionStatusId = '" + SolutionStatusIdVal + "'";
                    andCondition = 1;
                }
                var query = "select o.orgname,o.orgId,sa.SolutionAreaName,sa.SolutionAreaId,ps.solutionPlayName,ps.solutionPlayOrgWebsite,ps.marketplaceLink,ps.specialOfferLink,ps.logoFileLink,st.SolutionStatus,st.SolutionStatusId,ps.rowChangedDate, GeoName = STUFF((  SELECT ',' + g.geoname FROM Geo g join PartnerSolutionPlayAvailableGeo pg on g.geoid = pg.geoId WHERE pg.partnerSolutionPlayId = ps.partnerSolutionPlayId      FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') from organization o left join partnersolutionplay ps on o.orgid=ps.orgId inner join solutionArea sa on sa.SolutionAreaId=ps.SolutionAreaId left join solutionstatus st on st.SolutionStatusId=ps.SolutionStatusId  " +
                 where +
                 " order by ps.rowChangedDate desc";
                List<TechnologySolutionReportCSV> lst = _context.Database.SqlQueryRaw<TechnologySolutionReportCSV>(query).ToList();
                var properties = typeof(TechnologySolutionReportCSV).GetProperties();
                var csvContent = new StringBuilder();

                // Write the header row
                foreach (var prop in properties)
                {
                    csvContent.Append(prop.Name).Append(",");
                }
                csvContent.Length--; // Remove the last comma
                csvContent.AppendLine();

                // Write the data rows
                foreach (var item in lst)
                {
                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(item, null);
                        if (value != null)
                        {
                            csvContent.Append(value.ToString().Replace(",", " - ")).Append(",");
                        }
                        else
                        {
                            csvContent.Append(",");
                        }
                    }
                    csvContent.Length--; // Remove the last comma
                    csvContent.AppendLine();
                }

                // Return as byte array
                //Encoding.UTF8.GetBytes(csvContent.ToString());
                return File(Encoding.UTF8.GetBytes(csvContent.ToString()), "text/csv", "Report");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while downloading the reports. Please try again later.", Details = ex.Message });

            }
        }
        [HttpGet]
        [Route("GetTechnologySolutionReport")]
        public async Task<ActionResult<IEnumerable<TechnologySolutionReport>>> GetTechnologySolutionReport()
        {
            try
            {
                var query = "select o.orgname,o.orgId,sa.solutionAreaName,sa.solutionAreaId,ps.solutionPlayName,st.SolutionStatus,st.SolutionStatusId,ps.rowChangedDate from organization o " +
                "left join partnersolutionplay ps on o.orgid=ps.orgId " +
                "inner join solutionArea sa on sa.solutionAreaId=ps.solutionAreaId " +
                "left join solutionstatus st on st.SolutionStatusId=ps.SolutionStatusId " +
                "order by o.orgname desc";
                return await _context.Database.SqlQueryRaw<TechnologySolutionReport>(query).ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the reports. Please try again later.", Details = ex.Message });

            }

        }
        [HttpGet]
        [Route("GetAllUsersInvite")]
        public async Task<IEnumerable<UserInvite>> GetAllUsersInvite()
        {
            string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
            var utilities = new Utilities(configuration);
            var pUsers = (from user in _context.UserInvite
                          select new UserInvite
                          {
                              UserInviteId = user.UserInviteId,
                              UserInviteEmail = utilities.DecryptString(encKey, user.UserInviteEmail),
                              FirstName = utilities.DecryptString(encKey, user.FirstName),
                              LastName = utilities.DecryptString(encKey, user.LastName),
                              Status = user.Status,
                              RowChangedDate = user.RowChangedDate
                          }).ToList().OrderBy(t => t.UserInviteEmail);
            /*List<UserInvite> pUsers = new List<UserInvite>();
            var utilities = new Utilities(configuration);
            using (var command = this._context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "select * from UserInvite";
                command.CommandType = CommandType.Text;
                this._context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        UserInvite user = new UserInvite();
                        string decEmail = utilities.DecryptString(encKey, result["UserInviteEmail"].ToString());
                        user.UserInviteId = Guid.Parse(result["UserInviteId"].ToString());
                        user.UserInviteEmail = decEmail;
                        user.FirstName = utilities.DecryptString(encKey, result["FirstName"].ToString());
                        user.LastName = utilities.DecryptString(encKey, result["LastName"].ToString());
                        user.Status = result["Status"].ToString();
                        user.RowChangedDate = (DateTime)result["RowChangedDate"];
                        pUsers.Add(user);
                    }
                }
            }*/
            return pUsers;
        }

        [HttpGet]
        [Route("UserProfileDetails")]
        public async Task<PartnerUserProfileDTO> GetUserProfileDetails(Guid UserId, Guid OrgId)
        {
            string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
            var utilities = new Utilities(configuration);
            var data = (from pUser in _context.PartnerUser
                        where pUser.OrgId == OrgId
                        where pUser.PartnerUserId != UserId
                        where pUser.Status != "Deleted"
                        select new PartnerUser
                        {
                            FirstName = utilities.DecryptString(encKey, pUser.FirstName).ToString(),
                            LastName = utilities.DecryptString(encKey, pUser.LastName).ToString(),
                            PartnerEmail = utilities.DecryptString(encKey, pUser.PartnerEmail).ToString(),
                            PartnerUserId = pUser.PartnerUserId
                        }).ToList();
            var response = new PartnerUserProfileDTO();
            response.PartnerUserDTOs = data;
            response.PartnerUserSolutionDTOs = (from partnerSoln in _context.PartnerSolution
                             join ind in _context.Industries
                             on partnerSoln.IndustryId equals ind.IndustryId
                             join subInd in _context.SubIndustries
                             on partnerSoln.SubIndustryId equals subInd.SubIndustryId
                             join ss in _context.SolutionStatusType
                             on partnerSoln.SolutionStatusId equals ss.SolutionStatusId
                             where partnerSoln.UserId == UserId
                             select new PartnerUserSolutionDTO
                             {
                                 PartnerSolutionId = partnerSoln.PartnerSolutionId,
                                 IndustryName = ind.IndustryName,
                                 SubIndustryName = subInd.SubIndustryName,
                                 IndustryId = ind.IndustryId,
                                 SubIndustryId = subInd.SubIndustryId,
                                 UserId = partnerSoln.UserId,
                                 SolutionName = partnerSoln.SolutionName,
                                 Status = ss.SolutionStatus
                             }).ToList();
            return response;
        }

        [HttpPost]
        [Route("TransferOwnership")]
        public async Task<ResponseDTO> TransferOwnership(PartnerUserChangeProfileDTO updatedDetails)
        {
            var response = new ResponseDTO();
            if (updatedDetails?.PartnerUserSolutionDTOs.Count > 0)
            {
                foreach (var slnDetails in updatedDetails.PartnerUserSolutionDTOs)
                {
                    var soln = _context.PartnerSolution.Where(u => u.PartnerSolutionId == slnDetails.PartnerSolutionId).FirstOrDefault();
                    soln.UserId = updatedDetails.PartnerUserId;
                    _context.PartnerSolution.Update(soln);
                    await _context.SaveChangesAsync();
                }
                response.Response = true;
                response.Message = "Updated Successfully";
            }
            else
            {
                response.Response = false;
                response.Message = "Partner Solntion not found";
            }
            return response;
        }

        [HttpPost]
        [Route("MergeOrganization")]
        public async Task<ResponseDTO> MergeOrganization(MergeOrganization updatedDetails)
        {
            try
            {
                var response = new ResponseDTO();
                var users = _context.PartnerUser.Where(u => u.OrgId == updatedDetails.FromOrgId).ToList();
                if (users.Count > 0)
                {
                    foreach (var user in users)
                    {
                        user.OrgId = updatedDetails.ToOrgId;
                        _context.PartnerUser.Update(user);
                    }
                    await _context.SaveChangesAsync();
                }
                var partnerSolution = _context.PartnerSolution.Where(u => u.OrganizationId == updatedDetails.FromOrgId).ToList();
                if (partnerSolution.Count > 0)
                {
                    foreach (var pSoln in partnerSolution)
                    {
                        pSoln.OrganizationId = updatedDetails.ToOrgId;
                        _context.PartnerSolution.Update(pSoln);
                    }
                    await _context.SaveChangesAsync();
                }
                var partnerSolutionPlay = _context.PartnerSolutionPlay.Where(u => u.OrgId == updatedDetails.FromOrgId).ToList();
                if (partnerSolutionPlay.Count > 0)
                {
                    foreach (var pSolnPlay in partnerSolutionPlay)
                    {
                        pSolnPlay.OrgId = updatedDetails.ToOrgId;
                        _context.PartnerSolutionPlay.Update(pSolnPlay);
                    }
                    await _context.SaveChangesAsync();
                }
                /*var org = _context.Organization.Where(org => org.OrgId == updatedDetails.FromOrgId).FirstOrDefault();
                if (org != null)
                {              
                    _context.Remove(org);
                    await _context.SaveChangesAsync();
                }*/
                response.Response = true;
                response.Message = "Updated Successfully";
                return response;
            }
            catch (Exception ex)
            {
                var response = new ResponseDTO();
                response.Response = false;
                response.Message = "Technical issue";
                return response;
            }            
        }
        [HttpGet]
        [Route("DownloadPartnerSolutionFullReport")]
        public IActionResult DownloadPartnerSolutionFullReport()
        {
            try
            {
                string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
                var utilities = new Utilities(configuration);
                var lst = (from ps in _context.PartnerSolution
                              join org in _context.Organization
                              on ps.OrganizationId equals org.OrgId
                              join user in _context.PartnerUser
                              on ps.UserId equals user.PartnerUserId
                              join ind in _context.Industries
                              on ps.IndustryId equals ind.IndustryId
                              join subInd in _context.SubIndustries
                              on ps.SubIndustryId equals subInd.SubIndustryId
                              join ss in _context.SolutionStatusType
                              on ps.SolutionStatusId equals ss.SolutionStatusId
                              join pGeo in _context.PartnerSolutionAvailableGeo
                              on ps.PartnerSolutionId equals pGeo.PartnerSolutionId
                              join geo in _context.Geos
                              on pGeo.GeoId equals geo.GeoId
                              join pArea in _context.PartnerSolutionByArea
                              on ps.PartnerSolutionId equals pArea.PartnerSolutionId
                              join area in _context.SolutionAreas
                              on pArea.SolutionAreaId equals area.SolutionAreaId
                              join pRLink in _context.PartnerSolutionResourceLink
                              on pArea.PartnerSolutionByAreaId equals pRLink.PartnerSolutionByAreaId
                              join rLink in _context.ResourceLinks 
                              on pRLink.ResourceLinkId equals rLink.ResourceLinkId
                              select new PartnerSolutionFullReport
                              {
                                  OrgName = org.OrgName,
                                  PartnerFirstName = utilities.DecryptString(encKey, user.FirstName.ToString()),
                                  PartnerLastName = utilities.DecryptString(encKey, user.LastName.ToString()),
                                  PartnerEmail = utilities.DecryptString(encKey, user.PartnerEmail.ToString()),
                                  IndustryName = ind.IndustryName,
                                  SubIndustryName = subInd.SubIndustryName,
                                  SolutionName = ps.SolutionName,
                                  SolutionStatus = ss.SolutionStatus,
                                  RowChangedDate = ps.RowChangedDate,
                                  MarketPlaceLink = ps.MarketplaceLink,
                                  SolutionDescription = ps.SolutionDescription,
                                  SolutionOrgWebsite = ps.SolutionOrgWebsite,
                                  SpecialOfferLink = ps.SpecialOfferLink,
                                  GeoName = geo.Geoname,
                                  SolutionAreaName = area.SolutionAreaName,
                                  SolutionAreaDescription = pArea.AreaSolutionDescription,
                                  ResourceLinkName = rLink.ResourceLinkName,
                                  ResourceLinkTitle = pRLink.ResourceLinkTitle,
                                  ResourceLinkUrl = pRLink.ResourceLinkUrl,
                                  EventDateTime = pRLink.EventDateTime
                              }).ToList();
                
                var properties = typeof(PartnerSolutionFullReport).GetProperties();
                var csvContent = new StringBuilder();

                // Write the header row
                foreach (var prop in properties)
                {
                    csvContent.Append(prop.Name).Append(",");
                }
                csvContent.Length--; // Remove the last comma
                csvContent.AppendLine();

                // Write the data rows
                foreach (var item in lst)
                {
                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(item, null);                        
                        if (value != null)
                        {
                            value = RemoveSpecialCharacter(value.ToString());
                            csvContent.Append(value.ToString().Replace(",", "-").Replace("  ", " ")).Append(",");
                        }
                        else
                        {
                            csvContent.Append(",");
                        }
                    }
                    csvContent.Length--; // Remove the last comma
                    csvContent.AppendLine();
                }

                // Return as byte array
                //Encoding.UTF8.GetBytes(csvContent.ToString());
                return File(Encoding.UTF8.GetBytes(csvContent.ToString()), "text/csv", "Report");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while downloading the reports. Please try again later.", Details = ex.Message });

            }
        }
        public static string RemoveSpecialCharacter(string text)
        {
            // Replace any character that is not a letter, digit, space, underscore, or hyphen with an empty string
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '=' || c == '.' || c == '_' || c == ':' || c == '/' || c == ' ' || c == '&' || c == '-' || c == '@' || c == ';' || c == '<' || c == '>' || c == '(' || c == ')' || c == '"' || c == ',' || c == '$' || c == '!' || c == '#' || c == '*' || c == '?' || c == '{' || c == '}' || c == '[' || c == ']' || c == '/' || c == '?' || c == '%')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
            //return Regex.Replace(text, @"[^\w \-]", "");
        }
        /*[HttpGet]
        [Route("DownloadIndustrySolutionReport")]
        public IActionResult DownloadIndustrySolutionReport(Guid IndustryId, Guid IndustryThemeId, Guid SolutionAreaId, Boolean ExcludeSolutionArea)
        {
            try
            {
                string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
                
                var utilities = new Utilities(configuration);
                var lst = (from ps in _context.PartnerSolution
                           join org in _context.Organization
                           on ps.OrganizationId equals org.OrgId
                           join user in _context.PartnerUser
                           on ps.UserId equals user.PartnerUserId
                           join ind in _context.Industries
                           on ps.IndustryId equals ind.IndustryId
                           join subInd in _context.SubIndustries
                           on ps.SubIndustryId equals subInd.SubIndustryId
                           join theme in _context.IndustryTheme
                           on subInd.SubIndustryId equals theme.SubIndustryId
                           join ss in _context.SolutionStatusType
                           on ps.SolutionStatusId equals ss.SolutionStatusId
                           join pGeo in _context.PartnerSolutionAvailableGeo
                           on ps.PartnerSolutionId equals pGeo.PartnerSolutionId
                           join geo in _context.Geos
                           on pGeo.GeoId equals geo.GeoId
                           join pArea in _context.PartnerSolutionByArea
                           on ps.PartnerSolutionId equals pArea.PartnerSolutionId
                           join area in _context.SolutionAreas
                           on pArea.SolutionAreaId equals area.SolutionAreaId
                           join pRLink in _context.PartnerSolutionResourceLink
                           on pArea.PartnerSolutionByAreaId equals pRLink.PartnerSolutionByAreaId
                           join rLink in _context.ResourceLinks
                           on pRLink.ResourceLinkId equals rLink.ResourceLinkId
                           where ss.SolutionStatus  == "Approved"
                           select new IndustrySolutionFullReport
                           {
                               OrgName = org.OrgName,                               
                               IndustryName = ind.IndustryName,
                               UseCase = subInd.SubIndustryName,
                               SolutionName = ps.SolutionName,
                               SolutionStatus = ss.SolutionStatus,
                               MarketPlaceLink = ps.MarketplaceLink,
                               SolutionDescription = StripHTML(ps.SolutionDescription),
                               SolutionOrgWebsite = ps.SolutionOrgWebsite,
                               SpecialOfferLink = ps.SpecialOfferLink,
                               GeoName = geo.Geoname,
                               SolutionAreaName = area.SolutionAreaName,
                               SolutionAreaDescription = StripHTML(pArea.AreaSolutionDescription),
                               ResourceLinkName = rLink.ResourceLinkName,
                               ResourceLinkTitle = pRLink.ResourceLinkTitle,
                               ResourceLinkUrl = pRLink.ResourceLinkUrl,
                               IndustryId = ind.IndustryId,
                               IndustryThemeId = theme.IndustryThemeId,
                               SolutionAreaId = area.SolutionAreaId
                           }).ToList();
                
                if (IndustryId != Guid.Empty)
                {
                    lst = lst.Where(t => t.IndustryId == IndustryId).ToList();
                }
                if (IndustryThemeId != Guid.Empty)
                {
                    lst = lst.Where(t => t.IndustryThemeId == IndustryThemeId).ToList();
                }
                var csvContent = new StringBuilder();
                if (SolutionAreaId != Guid.Empty)
                {
                    lst = lst.Where(t => t.SolutionAreaId == SolutionAreaId).ToList();                    
                }
                var properties = typeof(IndustrySolutionFullReport).GetProperties();
                foreach (var prop in properties)
                {
                    if(prop.Name != "IndustryId" && prop.Name != "IndustryThemeId" && prop.Name != "SolutionAreaId" && prop.Name != "SolutionAreaName" && prop.Name != "SolutionAreaDescription")
                        csvContent.Append(prop.Name).Append(",");
                    else if((prop.Name == "SolutionAreaName" || prop.Name == "SolutionAreaDescription") && ExcludeSolutionArea == false)
                        csvContent.Append(prop.Name).Append(",");
                }
                // Write the header row

                csvContent.Length--; // Remove the last comma
                csvContent.AppendLine();
                // Write the data rows
                foreach (var item in lst)
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Name != "IndustryId" && prop.Name != "IndustryThemeId" && prop.Name != "SolutionAreaId" && prop.Name != "SolutionAreaName" && prop.Name != "SolutionAreaDescription")
                        {
                            var value = prop.GetValue(item, null);
                            if (value != null)
                            {
                                value = RemoveSpecialCharacter(value.ToString());
                                csvContent.Append(value.ToString().Replace(",", "-").Replace("  ", " ")).Append(",");
                            }
                            else
                            {
                                csvContent.Append(",");
                            }
                        }
                        else if ((prop.Name == "SolutionAreaName" || prop.Name == "SolutionAreaDescription") && ExcludeSolutionArea == false)
                        {
                            var value = prop.GetValue(item, null);
                            if (value != null)
                            {
                                value = RemoveSpecialCharacter(value.ToString());
                                csvContent.Append(value.ToString().Replace(",", "-").Replace("  ", " ")).Append(",");
                            }
                            else
                            {
                                csvContent.Append(",");
                            }
                        }
                    }
                    csvContent.Length--; // Remove the last comma
                    csvContent.AppendLine();
                }
                // Return as byte array
                //Encoding.UTF8.GetBytes(csvContent.ToString());
                return File(Encoding.UTF8.GetBytes(csvContent.ToString()), "text/csv", "Report");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while downloading the reports. Please try again later.", Details = ex.Message });

            }
        }*/
        /*[HttpPost]
        [Route("DownloadIndustrySolutionReport")]
        public IActionResult DownloadIndustrySolutionReport(IndustryReportDropDownDTO SelectedValues)
        {
            try
            {
                IEnumerable<Guid> IndustryIds = SelectedValues.IndustryId.Select(x => x.IndustryId).Distinct();
                IEnumerable<Guid> IndustryThemeIds = SelectedValues.IndustryThemeId.Select(x => x.IndustryThemeId).Distinct();
                IEnumerable<Guid> SolutionAreaIds = SelectedValues.SolutionAreaId.Select(x => x.SolutionAreaId).Distinct();
                string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;

                var utilities = new Utilities(configuration);
                var lst =  (from ps in _context.PartnerSolution
                           join org in _context.Organization
                           on ps.OrganizationId equals org.OrgId
                           join user in _context.PartnerUser
                           on ps.UserId equals user.PartnerUserId
                           join ind in _context.Industries
                           on ps.IndustryId equals ind.IndustryId
                           join subInd in _context.SubIndustries
                           on ps.SubIndustryId equals subInd.SubIndustryId
                           join theme in _context.IndustryTheme
                           on subInd.SubIndustryId equals theme.SubIndustryId
                           join ss in _context.SolutionStatusType
                           on ps.SolutionStatusId equals ss.SolutionStatusId
                           join pArea in _context.PartnerSolutionByArea
                           on ps.PartnerSolutionId equals pArea.PartnerSolutionId
                           join area in _context.SolutionAreas
                           on pArea.SolutionAreaId equals area.SolutionAreaId
                            join pRLink in _context.PartnerSolutionResourceLink
                            on pArea.PartnerSolutionByAreaId equals pRLink.PartnerSolutionByAreaId into resources
                            from pRLink in resources.DefaultIfEmpty()
                            join rLink in _context.ResourceLinks
                            on pRLink.ResourceLinkId equals rLink.ResourceLinkId into resourceLink
                            from rLink in resourceLink.DefaultIfEmpty()
                            where ss.SolutionStatus == "Approved"
                           where IndustryIds.Count() > 0 ? IndustryIds.Contains(ind.IndustryId) : true
                           where IndustryThemeIds.Count() > 0 ? IndustryThemeIds.Contains(theme.IndustryThemeId) : true
                           where SolutionAreaIds.Count() > 0 ? SolutionAreaIds.Contains(area.SolutionAreaId) : true
                            select new IndustrySolutionFullReport
                           {
                               OrgName = org.OrgName,
                               IndustryName = ind.IndustryName,
                               UseCase = subInd.SubIndustryName,
                               SolutionName = ps.SolutionName,
                               SolutionStatus = ss.SolutionStatus,
                               MarketPlaceLink = ps.MarketplaceLink,
                               SolutionDescription = StripHTML(ps.SolutionDescription),
                               SolutionOrgWebsite = ps.SolutionOrgWebsite,
                               SpecialOfferLink = ps.SpecialOfferLink,
                               Canada = "No",
                               LatinAmerica = "No",
                               UnitedStates = "No",
                               SolutionAreaName = area.SolutionAreaName,
                               SolutionAreaDescription = StripHTML(pArea.AreaSolutionDescription),
                                ResourceLinkName = rLink.ResourceLinkName,
                                ResourceLinkTitle = pRLink.ResourceLinkTitle,
                                ResourceLinkUrl = pRLink.ResourceLinkUrl,
                                PartnerSolutionId = ps.PartnerSolutionId,
                                //RowChangedDate = ps.RowChangedDate,
                                CreatedDate = ps.RowCreatedDate
                           }).ToList();

                foreach(var data in lst)
                {
                    var geos = (from geoId in _context.PartnerSolutionAvailableGeo
                                join geo in _context.Geos
                                on geoId.GeoId equals geo.GeoId
                                where geoId.PartnerSolutionId == data.PartnerSolutionId
                                select new IndustrySolutionGeoReport
                                {
                                    GeoName = geo.Geoname
                                }).ToList();
                    
                    foreach (var geo in geos)
                    {
                        if(geo.GeoName == "Canada")
                        {
                            data.Canada = "Yes";
                        }
                        if (geo.GeoName == "Latin America")
                        {
                            data.LatinAmerica = "Yes";
                        }
                        if (geo.GeoName == "United States")
                        {
                            data.UnitedStates = "Yes";
                        }
                    }
                }
                var csvContent = new StringBuilder();
                
                var properties = typeof(IndustrySolutionFullReport).GetProperties();
                foreach (var prop in properties)
                {
                    if (prop.Name != "SolutionAreaName" && prop.Name != "SolutionAreaDescription" && prop.Name != "PartnerSolutionId")
                        csvContent.Append(prop.Name).Append(",");
                    else if ((prop.Name == "SolutionAreaName" || prop.Name == "SolutionAreaDescription") && SelectedValues.ExcludeSolutionArea == false)
                        csvContent.Append(prop.Name).Append(",");
                }
                // Write the header row

                csvContent.Length--; // Remove the last comma
                csvContent.AppendLine();
                // Write the data rows
                foreach (var item in lst)
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Name != "SolutionAreaName" && prop.Name != "SolutionAreaDescription" && prop.Name != "PartnerSolutionId")
                        {
                            var value = prop.GetValue(item, null);
                            if (value != null)
                            {
                                value = RemoveSpecialCharacter(value.ToString());
                                csvContent.Append(value.ToString().Replace(",", "-").Replace("  ", " ")).Append(",");
                            }
                            else
                            {
                                csvContent.Append(",");
                            }
                        }
                        else if ((prop.Name == "SolutionAreaName" || prop.Name == "SolutionAreaDescription") && SelectedValues.ExcludeSolutionArea == false)
                        {
                            var value = prop.GetValue(item, null);
                            if (value != null)
                            {
                                value = RemoveSpecialCharacter(value.ToString());
                                csvContent.Append(value.ToString().Replace(",", "-").Replace("  ", " ")).Append(",");
                            }
                            else
                            {
                                csvContent.Append(",");
                            }
                        }

                    }
                    csvContent.Length--; // Remove the last comma
                    csvContent.AppendLine();
                }
                // Return as byte array
                //Encoding.UTF8.GetBytes(csvContent.ToString());
                return File(Encoding.UTF8.GetBytes(csvContent.ToString()), "text/csv", "Report");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while downloading the reports. Please try again later.", Details = ex.Message });

            }
        }*/
        [HttpPost]
        [Route("DownloadArchiveIndustrySolutionReport")]
        public IActionResult DownloadArchiveIndustrySolutionReport()
        {
            try
            {
                //IEnumerable<Guid> IndustryThemeIds = SelectedValues.IndustryThemeId.Select(x => x.IndustryThemeId).Distinct();
                //IEnumerable<Guid> SolutionAreaIds = SelectedValues.SolutionAreaId.Select(x => x.SolutionAreaId).Distinct();
                string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;

                var utilities = new Utilities(configuration);
                var lst = (from ps in _context.PartnerSolution
                           join org in _context.Organization
                           on ps.OrganizationId equals org.OrgId
                           join user in _context.PartnerUser
                           on ps.UserId equals user.PartnerUserId
                           join ind in _context.Industries
                           on ps.IndustryId equals ind.IndustryId
                           //join subInd in _context.ArchiveSubIndustries
                           //on ps.SubIndustryId equals subInd.SubIndustryId
                           //join theme in _context.ArchiveIndustryTheme
                           //on subInd.SubIndustryId equals theme.SubIndustryId
                           join ss in _context.SolutionStatusType
                           on ps.SolutionStatusId equals ss.SolutionStatusId
                           join pArea in _context.PartnerSolutionByArea
                           on ps.PartnerSolutionId equals pArea.PartnerSolutionId
                           join area in _context.SolutionAreas
                           on pArea.SolutionAreaId equals area.SolutionAreaId
                           join pRLink in _context.PartnerSolutionResourceLink
                           on pArea.PartnerSolutionByAreaId equals pRLink.PartnerSolutionByAreaId into resources
                           from pRLink in resources.DefaultIfEmpty()
                           join rLink in _context.ResourceLinks
                           on pRLink.ResourceLinkId equals rLink.ResourceLinkId into resourceLink
                           from rLink in resourceLink.DefaultIfEmpty()
                               //where IndustryThemeIds.Count() > 0 ? IndustryThemeIds.Contains(theme.IndustryThemeId) : true
                               //where SolutionAreaIds.Count() > 0 ? SolutionAreaIds.Contains(area.SolutionAreaId) : true
                           where ps.IsPublished == 5
                           select new IndustrySolutionFullReport
                           {
                               OrgName = org.OrgName,
                               IndustryName = ind.IndustryName,
                               UseCase = "Archive Usecase",//subInd.SubIndustryName,
                               SolutionName = ps.SolutionName,
                               SolutionStatus = ss.SolutionStatus,
                               MarketPlaceLink = ps.MarketplaceLink,
                               SolutionDescription = StripHTML(ps.SolutionDescription),
                               SolutionOrgWebsite = ps.SolutionOrgWebsite,
                               SpecialOfferLink = ps.SpecialOfferLink,
                               Canada = "No",
                               LatinAmerica = "No",
                               UnitedStates = "No",
                               SolutionAreaName = area.SolutionAreaName,
                               SolutionAreaDescription = StripHTML(pArea.AreaSolutionDescription),
                               //ResourceLinkName = rLink.ResourceLinkName,
                               //ResourceLinkTitle = pRLink.ResourceLinkTitle,
                               //ResourceLinkUrl = pRLink.ResourceLinkUrl,
                               PartnerSolutionId = ps.PartnerSolutionId,
                               //RowChangedDate = ps.RowChangedDate,
                               CreatedDate = ps.RowCreatedDate
                           }).ToList();

                foreach (var data in lst)
                {
                    var geos = (from geoId in _context.PartnerSolutionAvailableGeo
                                join geo in _context.Geos
                                on geoId.GeoId equals geo.GeoId
                                where geoId.PartnerSolutionId == data.PartnerSolutionId
                                select new IndustrySolutionGeoReport
                                {
                                    GeoName = geo.Geoname
                                }).ToList();

                    foreach (var geo in geos)
                    {
                        if (geo.GeoName == "Canada")
                        {
                            data.Canada = "Yes";
                        }
                        if (geo.GeoName == "Latin America")
                        {
                            data.LatinAmerica = "Yes";
                        }
                        if (geo.GeoName == "United States")
                        {
                            data.UnitedStates = "Yes";
                        }
                    }
                }
                var csvContent = new StringBuilder();

                var properties = typeof(IndustrySolutionFullReport).GetProperties();
                foreach (var prop in properties)
                {
                    if (prop.Name != "SolutionAreaName" && prop.Name != "SolutionAreaDescription" && prop.Name != "PartnerSolutionId")
                        csvContent.Append(prop.Name).Append(",");
                    else if ((prop.Name == "SolutionAreaName" || prop.Name == "SolutionAreaDescription") )
                        csvContent.Append(prop.Name).Append(",");
                }
                // Write the header row

                csvContent.Length--; // Remove the last comma
                csvContent.AppendLine();
                // Write the data rows
                foreach (var item in lst)
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Name != "SolutionAreaName" && prop.Name != "SolutionAreaDescription" && prop.Name != "PartnerSolutionId")
                        {
                            var value = prop.GetValue(item, null);
                            if (value != null)
                            {
                                value = RemoveSpecialCharacter(value.ToString());
                                csvContent.Append(value.ToString().Replace(",", "-").Replace("  ", " ")).Append(",");
                            }
                            else
                            {
                                csvContent.Append(",");
                            }
                        }
                        else if ((prop.Name == "SolutionAreaName" || prop.Name == "SolutionAreaDescription") )
                        {
                            var value = prop.GetValue(item, null);
                            if (value != null)
                            {
                                value = RemoveSpecialCharacter(value.ToString());
                                csvContent.Append(value.ToString().Replace(",", "-").Replace("  ", " ")).Append(",");
                            }
                            else
                            {
                                csvContent.Append(",");
                            }
                        }

                    }
                    csvContent.Length--; // Remove the last comma
                    csvContent.AppendLine();
                }
                // Return as byte array
                //Encoding.UTF8.GetBytes(csvContent.ToString());
                return File(Encoding.UTF8.GetBytes(csvContent.ToString()), "text/csv", "Report");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while downloading the reports. Please try again later.", Details = ex.Message });

            }
        }
        /*[HttpGet]
        [Route("CheckCountForIndustrySolutionReport")]
        public async Task<ResponseDTO> CheckCountForIndustrySolutionReport(Guid IndustryId, Guid IndustryThemeId, Guid SolutionAreaId)
        {
            var lst = (from ps in _context.PartnerSolution
                       join org in _context.Organization
                       on ps.OrganizationId equals org.OrgId
                       join user in _context.PartnerUser
                       on ps.UserId equals user.PartnerUserId
                       join ind in _context.Industries
                       on ps.IndustryId equals ind.IndustryId
                       join subInd in _context.SubIndustries
                       on ps.SubIndustryId equals subInd.SubIndustryId
                       join theme in _context.IndustryTheme
                       on subInd.SubIndustryId equals theme.SubIndustryId
                       join ss in _context.SolutionStatusType
                       on ps.SolutionStatusId equals ss.SolutionStatusId
                       join pGeo in _context.PartnerSolutionAvailableGeo
                       on ps.PartnerSolutionId equals pGeo.PartnerSolutionId
                       join geo in _context.Geos
                       on pGeo.GeoId equals geo.GeoId
                       join pArea in _context.PartnerSolutionByArea
                       on ps.PartnerSolutionId equals pArea.PartnerSolutionId
                       join area in _context.SolutionAreas
                       on pArea.SolutionAreaId equals area.SolutionAreaId
                       join pRLink in _context.PartnerSolutionResourceLink
                       on pArea.PartnerSolutionByAreaId equals pRLink.PartnerSolutionByAreaId
                       join rLink in _context.ResourceLinks
                       on pRLink.ResourceLinkId equals rLink.ResourceLinkId
                       where ss.SolutionStatus == "Approved"
                       select new IndustrySolutionFullReport
                       {                           
                           IndustryId = ind.IndustryId,
                           IndustryThemeId = theme.IndustryThemeId,
                           SolutionAreaId = area.SolutionAreaId
                       }).ToList();

            if (IndustryId != Guid.Empty)
            {
                lst = lst.Where(t => t.IndustryId == IndustryId).ToList();
            }
            if (IndustryThemeId != Guid.Empty)
            {
                lst = lst.Where(t => t.IndustryThemeId == IndustryThemeId).ToList();
            }
            if (SolutionAreaId != Guid.Empty)
            {
                lst = lst.Where(t => t.SolutionAreaId == SolutionAreaId).ToList();
            }
            var response = new ResponseDTO();
            if (lst.Count()>0)
            {
                response.Response = true;
            }
            else
            {
                response.Response = false;
            }
            return response;
        }*/
        [HttpPost]
        [Route("CheckCountForIndustrySolutionReport")]
        public async Task<ResponseDTO> CheckCountForIndustrySolutionReport(IndustryReportDropDownDTO SelectedValues)
        {
            IEnumerable<Guid> IndustryIds = SelectedValues.IndustryId.Select(x => x.IndustryId).Distinct();
            IEnumerable<Guid> IndustryThemeIds = SelectedValues.IndustryThemeId.Select(x => x.IndustryThemeId).Distinct();
            IEnumerable<Guid> SolutionAreaIds = SelectedValues.SolutionAreaId.Select(x => x.SolutionAreaId).Distinct();
            var lst = (from ps in _context.PartnerSolution
                       join org in _context.Organization
                       on ps.OrganizationId equals org.OrgId
                       join user in _context.PartnerUser
                       on ps.UserId equals user.PartnerUserId
                       join ind in _context.Industries
                       on ps.IndustryId equals ind.IndustryId
                       join subInd in _context.SubIndustries
                       on ps.SubIndustryId equals subInd.SubIndustryId
                       join theme in _context.IndustryTheme
                       on subInd.SubIndustryId equals theme.SubIndustryId
                       join ss in _context.SolutionStatusType
                       on ps.SolutionStatusId equals ss.SolutionStatusId                       
                       join pArea in _context.PartnerSolutionByArea
                       on ps.PartnerSolutionId equals pArea.PartnerSolutionId
                       join area in _context.SolutionAreas
                       on pArea.SolutionAreaId equals area.SolutionAreaId
                       join pRLink in _context.PartnerSolutionResourceLink
                       on pArea.PartnerSolutionByAreaId equals pRLink.PartnerSolutionByAreaId into resources
                       from pRLink in resources.Take(1)
                       join rLink in _context.ResourceLinks
                       on pRLink.ResourceLinkId equals rLink.ResourceLinkId 
                       where ss.SolutionStatus == "Approved"
                       where IndustryIds.Count() > 0 ? IndustryIds.Contains(ind.IndustryId) : true
                       where IndustryThemeIds.Count() > 0 ? IndustryThemeIds.Contains(theme.IndustryThemeId) : true
                       where SolutionAreaIds.Count() > 0 ? SolutionAreaIds.Contains(area.SolutionAreaId) : true
                       select new IndustrySolutionFullReport
                       {
                           OrgName = org.OrgName
                       }).ToList();
           
            var response = new ResponseDTO();
            if (lst.Count() > 0)
            {
                response.Response = true;
            }
            else
            {
                response.Response = false;
            }
            return response;
        }
        [HttpPost]
        [Route("CheckCountForArchiveIndustrySolutionReport")]
        public async Task<ResponseDTO> CheckCountForArchiveIndustrySolutionReport()
        {            
             var lst = (from ps in _context.PartnerSolution
                       join org in _context.Organization
                       on ps.OrganizationId equals org.OrgId
                       join user in _context.PartnerUser
                       on ps.UserId equals user.PartnerUserId
                       join ind in _context.Industries
                       on ps.IndustryId equals ind.IndustryId
                       //join subInd in _context.ArchiveSubIndustries
                       //on ps.SubIndustryId equals subInd.SubIndustryId
                       //join theme in _context.ArchiveIndustryTheme
                       //on subInd.SubIndustryId equals theme.SubIndustryId
                       join ss in _context.SolutionStatusType
                       on ps.SolutionStatusId equals ss.SolutionStatusId
                        //join pArea in _context.PartnerSolutionByArea
                        //on ps.PartnerSolutionId equals pArea.PartnerSolutionId
                        //join area in _context.SolutionAreas
                        //on pArea.SolutionAreaId equals area.SolutionAreaId
                        //join pRLink in _context.PartnerSolutionResourceLink
                        //on pArea.PartnerSolutionByAreaId equals pRLink.PartnerSolutionByAreaId into resources
                        //from pRLink in resources.DefaultIfEmpty()
                        //join rLink in _context.ResourceLinks
                        //on pRLink.ResourceLinkId equals rLink.ResourceLinkId                       
                        //where IndustryThemeIds.Count() > 0 ? IndustryThemeIds.Contains(theme.IndustryThemeId) : true
                        //where SolutionAreaIds.Count() > 0 ? SolutionAreaIds.Contains(area.SolutionAreaId) : true
                       where ps.IsPublished == 5
                       select new IndustrySolutionFullReport
                       {
                           OrgName = org.OrgName
                       }).ToList();

            var response = new ResponseDTO();
            if (lst.Count() > 0)
            {
                response.Response = true;
            }
            else
            {
                response.Response = false;
            }
            return response;
        }
        [HttpGet]
        [Route("GetAllTheme")]
        public async Task<IEnumerable<IndustryThemeDropDownDTO>> GetAllTheme()
        {
            var theme = (from th in _context.IndustryTheme
                         join ss in _context.SolutionStatusType
                         on th.SolutionStatusId equals ss.SolutionStatusId
                         where ss.SolutionStatus == "Approved"
                         select new IndustryThemeDropDownDTO
                        {
                            Theme = th.Theme, //Regex.Replace(th.Theme, "<.*?>", string.Empty),
                            IndustryThemeId = th.IndustryThemeId
                        }).OrderBy(t=>t.Theme).ToList();
            return theme;
        }
        [HttpPost]
        [Route("GetThemeFilter")]
        public async Task<IEnumerable<IndustryThemeDropDownDTO>> GetThemeFilter(List<IndustryDropDownDTO> IndustryIds)
        {
            IEnumerable<Guid> ids = IndustryIds.Select(x => x.IndustryId).Distinct();
            var theme = (from th in _context.IndustryTheme
                         join ss in _context.SolutionStatusType
                         on th.SolutionStatusId equals ss.SolutionStatusId
                         where ss.SolutionStatus == "Approved"
                         where ids.Contains(th.IndustryId)
                         select new IndustryThemeDropDownDTO
                         {
                             Theme = th.Theme, //Regex.Replace(th.Theme, "<.*?>", string.Empty),
                             IndustryThemeId = th.IndustryThemeId
                         }).OrderBy(t => t.Theme).ToList();
            return theme;
        }
        [HttpGet]
        [Route("GetArchiveThemeFilter")]
        public async Task<IEnumerable<IndustryThemeDropDownDTO>> GetArchiveThemeFilter()
        {            
            var theme = (from th in _context.ArchiveIndustryTheme
                         select new IndustryThemeDropDownDTO
                         {
                             Theme = th.Theme, //Regex.Replace(th.Theme, "<.*?>", string.Empty),
                             IndustryThemeId = th.IndustryThemeId
                         }).OrderBy(t => t.Theme).ToList();
            return theme;
        }
        [HttpPost]
        [Route("GetSolutionAreaFilter")]
        public async Task<IEnumerable<SolutionArea>> GetSolutionAreaFilter(List<IndustryThemeDropDownDTO> IndustryThemeIds)
        {
            IEnumerable<Guid> ids = IndustryThemeIds.Select(x => x.IndustryThemeId).Distinct();
            var solutionArea = (from solArea in _context.SolutionAreas
                         join thSolutionArea in _context.IndustryThemeBySolutionArea
                         on solArea.SolutionAreaId equals thSolutionArea.SolutionAreaId
                         where ids.Contains(thSolutionArea.IndustryThemeId)
                         //where thSolutionArea.IndustryThemeId == IndustryThemeId
                         select new SolutionArea
                         {
                             SolutionAreaName = solArea.SolutionAreaName,
                             SolutionAreaId = solArea.SolutionAreaId
                         }).OrderBy(t => t.SolutionAreaName).ToList();
            solutionArea.Select(o => o.SolutionAreaName).Distinct();
            return solutionArea;
        }
        public static string StripHTML(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = Regex.Replace(input, "&nbsp;", " ");
                input = Regex.Replace(input, "&mdash;", " ");
                input = Regex.Replace(input, "&[^;]+;", string.Empty);
                return RemoveSpecialCharacter(Regex.Replace(input, "<.*?>", String.Empty));
            }                
            else
                return input;
        }
        [HttpGet]
        [Route("GetAllIndustry")]
        public async Task<IEnumerable<Industry>> GetAllIndustry()
        {
            var Geos = await _context.Industries.OrderBy(g => g.IndustryName).ToListAsync();
            return Geos;
        }
        [HttpPost]
        [Route("UpdateIndustry")]
        public async Task<IEnumerable<Industry>> UpdateIndustry(Industry updatedDetails)
        {
            string slug = Regex.Replace(updatedDetails.IndustryName, @"<[^>]+>|&nbsp;", "").Trim();
            slug = Regex.Replace(slug, @"\s{2,}", " ");
            slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
            slug = Regex.Replace(slug, @" ", "-");
            slug = slug.ToLower();
            Boolean slugCheck = true;
            string slugTmp = slug;
            int j = 1;
            while (slugCheck)
            {
                var resCheck = _context.Industries.Where(t => t.IndustrySlug == slug).Where(u => u.IndustryId != updatedDetails.IndustryId).FirstOrDefault();
                if (resCheck == null)
                    slugCheck = false;
                else
                {
                    slug = slugTmp + "-" + j;
                    j++;
                }

            }

            var res = _context.Industries.Where(u => u.IndustryId == updatedDetails.IndustryId).FirstOrDefault();
            res.IndustryName = updatedDetails.IndustryName;
            res.IndustryDescription = updatedDetails.IndustryDescription;
            res.RowChangedDate = DateTime.UtcNow;
            res.IndustrySlug = slug;
            res.Image_main = updatedDetails.Image_main;
            res.Image_mobile = updatedDetails.Image_mobile;
            res.Status = "active";
            _context.Industries.Update(res);
            await _context.SaveChangesAsync();
            return await _context.Industries.ToListAsync();
        }
        /*[HttpPost]
        [Route("DeleteIndustry")]
        public async Task<ResponseDTO> DeleteIndustry(Industry updatedDetails)
        {
            var res = _context.Industries.Where(u => u.IndustryId == updatedDetails.IndustryId).FirstOrDefault();
            var response = new ResponseDTO();
            if (res != null)
            {
                var resDetail = _context.PartnerSolutionAvailableGeo.Where(u => u.GeoId == updatedDetails.GeoId).FirstOrDefault();
                var solutionPlayGeo = _context.PartnerSolutionPlayAvailableGeo.Where(u => u.GeoId == updatedDetails.GeoId).FirstOrDefault();
                if (partnerGeo != null || solutionPlayGeo != null)
                {
                    response.Response = false;
                    response.Message = "We cant delete! Geo already added in solutions";
                    return response;
                }
                else
                {
                    _context.Remove(geo);
                    await _context.SaveChangesAsync();
                    response.Response = true;
                    response.Message = "Deleted Successfully";
                    return response;
                }
            }
            else
            {
                response.Response = false;
                response.Message = "Data not exists.";
                return response;
            }
        }*/
        [HttpPost]
        [Route("AddIndustry")]
        public async Task<IEnumerable<Industry>> AddIndustry(Industry updatedDetails)
        {
            string slug = Regex.Replace(updatedDetails.IndustryName, @"<[^>]+>|&nbsp;", "").Trim();
            slug = Regex.Replace(slug, @"\s{2,}", " ");
            slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
            slug = Regex.Replace(slug, @" ", "-");
            slug = slug.ToLower();
            Boolean slugCheck = true;
            string slugTmp = slug;
            int j = 1;
            while (slugCheck)
            {
                var resCheck = _context.Industries.Where(t => t.IndustrySlug == slug).Where(u => u.IndustryId != updatedDetails.IndustryId).FirstOrDefault();
                if (resCheck == null)
                    slugCheck = false;
                else
                {
                    slug = slugTmp + "-" + j;
                    j++;
                }

            }
            var res = new Industry
            {
                IndustryId = Guid.NewGuid(),
                IndustryName = updatedDetails.IndustryName,
                IndustrySlug = slug,
                RowChangedDate = DateTime.UtcNow,
                IndustryDescription = updatedDetails.IndustryDescription,
                Image_main = updatedDetails.Image_main,
                Image_mobile = updatedDetails.Image_mobile,
                Status = "active",
            };
            _context.Industries.Add(res);
            await _context.SaveChangesAsync();
            return await _context.Industries.ToListAsync();
        }
        [HttpGet]
        [Route("GetAllSubIndustry")]
        public async Task<IEnumerable<SubIndustryWithIndustryTheme>> GetAllSubIndustry()
        {
            var res = (from si in _context.SubIndustries
                         join it in _context.IndustryTheme
                         on si.SubIndustryId equals it.SubIndustryId
                         select new SubIndustryWithIndustryTheme
                         {
                             SubIndustryDescription = si.SubIndustryDescription,
                             SubIndustryId = si.SubIndustryId,
                             SubIndustryName = si.SubIndustryName,
                             Image_main = it.Image_Main,
                             Image_thumb = it.Image_Thumb,
                             Image_mobile = it.Image_Mobile

                         }).OrderBy(s => s.SubIndustryName).ToList();
            return res;
        }
        [HttpPost]
        [Route("UpdateSubIndustry")]
        public async Task<IEnumerable<SubIndustry>> UpdateSubIndustry(SubIndustryWithIndustryTheme updatedDetails)
        {
            string slug = Regex.Replace(updatedDetails.SubIndustryName, @"<[^>]+>|&nbsp;", "").Trim();
            slug = Regex.Replace(slug, @"\s{2,}", " ");
            slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
            slug = Regex.Replace(slug, @" ", "-");
            slug = slug.ToLower();
            Boolean slugCheck = true;
            string slugTmp = slug;
            int j = 1;
            while (slugCheck)
            {
                var resCheck = _context.SubIndustries.Where(t => t.SubIndustrySlug == slug).Where(u => u.SubIndustryId != updatedDetails.SubIndustryId).FirstOrDefault();
                if (resCheck == null)
                    slugCheck = false;
                else
                {
                    slug = slugTmp + "-" + j;
                    j++;
                }

            }

            var res = _context.SubIndustries.Where(u => u.SubIndustryId == updatedDetails.SubIndustryId).FirstOrDefault();
            res.SubIndustryName = updatedDetails.SubIndustryName;
            res.SubIndustryDescription = updatedDetails.SubIndustryDescription;
            res.RowChangedDate = DateTime.UtcNow;
            res.SubIndustrySlug = slug;
            res.Status = "active";
            _context.SubIndustries.Update(res);
            var industryTheme = _context.IndustryTheme.Where(u => u.SubIndustryId == updatedDetails.SubIndustryId).FirstOrDefault();
            industryTheme.Image_Main = updatedDetails.Image_main;
            industryTheme.Image_Thumb = updatedDetails.Image_thumb;
            industryTheme.Image_Mobile = updatedDetails.Image_mobile;
            await _context.SaveChangesAsync();
            return await _context.SubIndustries.ToListAsync();
        }
        /*[HttpPost]
        [Route("DeleteIndustry")]
        public async Task<ResponseDTO> DeleteIndustry(Industry updatedDetails)
        {
            var res = _context.Industries.Where(u => u.IndustryId == updatedDetails.IndustryId).FirstOrDefault();
            var response = new ResponseDTO();
            if (res != null)
            {
                var resDetail = _context.PartnerSolutionAvailableGeo.Where(u => u.GeoId == updatedDetails.GeoId).FirstOrDefault();
                var solutionPlayGeo = _context.PartnerSolutionPlayAvailableGeo.Where(u => u.GeoId == updatedDetails.GeoId).FirstOrDefault();
                if (partnerGeo != null || solutionPlayGeo != null)
                {
                    response.Response = false;
                    response.Message = "We cant delete! Geo already added in solutions";
                    return response;
                }
                else
                {
                    _context.Remove(geo);
                    await _context.SaveChangesAsync();
                    response.Response = true;
                    response.Message = "Deleted Successfully";
                    return response;
                }
            }
            else
            {
                response.Response = false;
                response.Message = "Data not exists.";
                return response;
            }
        }*/
        /*[HttpPost]
        [Route("AddSubIndustry")]
        public async Task<IEnumerable<SubIndustry>> AddSubIndustry(SubIndustry updatedDetails)
        {
            string slug = Regex.Replace(updatedDetails.SubIndustryName, @"<[^>]+>|&nbsp;", "").Trim();
            slug = Regex.Replace(slug, @"\s{2,}", " ");
            slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
            slug = Regex.Replace(slug, @" ", "-");
            slug = slug.ToLower();
            Boolean slugCheck = true;
            string slugTmp = slug;
            int j = 1;
            while (slugCheck)
            {
                var resCheck = _context.SubIndustries.Where(t => t.SubIndustrySlug == slug).Where(u => u.SubIndustryId != updatedDetails.SubIndustryId).FirstOrDefault();
                if (resCheck == null)
                    slugCheck = false;
                else
                {
                    slug = slugTmp + "-" + j;
                    j++;
                }

            }
            var res = new SubIndustry
            {
                SubIndustryId = Guid.NewGuid(),
                SubIndustryName = updatedDetails.SubIndustryName,
                SubIndustrySlug = slug,
                RowChangedDate = DateTime.UtcNow,
                SubIndustryDescription = updatedDetails.SubIndustryDescription,
                Status = "active",
            };
            _context.SubIndustries.Add(res);
            await _context.SaveChangesAsync();
            return await _context.SubIndustries.ToListAsync();
        }*/
        [HttpPost]
        [Route("DownloadPartnerSolutionReport")]
        public IActionResult DownloadPartnerSolutionReport(PartnerReportDropDownDTO SelectedValues)
        {
            try
            {
                IEnumerable<Guid> SubIndustryIds = SelectedValues.SubIndustryId.Select(x => x.SubIndustryId).Distinct();
                IEnumerable<Guid> OrgIds = SelectedValues.OrgId.Select(x => x.OrgId).Distinct();
                IEnumerable<Guid> SolutionStatusIds = SelectedValues.SolutionStatusId.Select(x => x.SolutionStatusId).Distinct();
                string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;

                var utilities = new Utilities(configuration);
                var lst = (from ps in _context.PartnerSolution
                           join org in _context.Organization
                           on ps.OrganizationId equals org.OrgId
                           //join user in _context.PartnerUser
                           //on ps.UserId equals user.PartnerUserId
                           join ind in _context.Industries
                           on ps.IndustryId equals ind.IndustryId
                           join subInd in _context.SubIndustries
                           on ps.SubIndustryId equals subInd.SubIndustryId
                           join theme in _context.IndustryTheme
                           on subInd.SubIndustryId equals theme.SubIndustryId
                           join ss in _context.SolutionStatusType
                           on ps.SolutionStatusId equals ss.SolutionStatusId
                           /*join pArea in _context.PartnerSolutionByArea
                           on ps.PartnerSolutionId equals pArea.PartnerSolutionId
                           join area in _context.SolutionAreas
                           on pArea.SolutionAreaId equals area.SolutionAreaId
                           join pRLink in _context.PartnerSolutionResourceLink
                           on pArea.PartnerSolutionByAreaId equals pRLink.PartnerSolutionByAreaId into resources
                           from pRLink in resources.DefaultIfEmpty()
                           join rLink in _context.ResourceLinks
                           on pRLink.ResourceLinkId equals rLink.ResourceLinkId into resourceLink
                           from rLink in resourceLink.DefaultIfEmpty()*/
                           //where ss.SolutionStatus == "Approved"
                           where OrgIds.Count() > 0 ? OrgIds.Contains(org.OrgId) : true
                           where SubIndustryIds.Count() > 0 ? SubIndustryIds.Contains(subInd.SubIndustryId) : true                           
                           where SolutionStatusIds.Count() > 0 ? SolutionStatusIds.Contains(ss.SolutionStatusId) : true
                           select new IndustrySolutionFullReport
                           {
                               OrgName = org.OrgName,
                               IndustryName = ind.IndustryName,
                               UseCase = subInd.SubIndustryName,
                               SolutionName = ps.SolutionName,
                               SolutionStatus = ss.SolutionStatus,
                               MarketPlaceLink = ps.MarketplaceLink,
                               SolutionDescription = StripHTML(ps.SolutionDescription),
                               SolutionOrgWebsite = ps.SolutionOrgWebsite,
                               SpecialOfferLink = ps.SpecialOfferLink,
                               Canada = "No",
                               LatinAmerica = "No",
                               UnitedStates = "No",
                               SolutionAreaName = "",//area.SolutionAreaName,
                               SolutionAreaDescription = "", //StripHTML(pArea.AreaSolutionDescription),
                               //ResourceLinkName = rLink.ResourceLinkName,
                               //ResourceLinkTitle = pRLink.ResourceLinkTitle,
                              // ResourceLinkUrl = pRLink.ResourceLinkUrl,
                               PartnerSolutionId = ps.PartnerSolutionId,
                               //RowChangedDate = ps.RowChangedDate,
                               CreatedDate = ps.RowCreatedDate
                           }).ToList();

                foreach (var data in lst)
                {
                    var geos = (from geoId in _context.PartnerSolutionAvailableGeo
                                join geo in _context.Geos
                                on geoId.GeoId equals geo.GeoId
                                where geoId.PartnerSolutionId == data.PartnerSolutionId
                                select new IndustrySolutionGeoReport
                                {
                                    GeoName = geo.Geoname
                                }).ToList();

                    foreach (var geo in geos)
                    {
                        if (geo.GeoName == "Canada")
                        {
                            data.Canada = "Yes";
                        }
                        if (geo.GeoName == "Latin America")
                        {
                            data.LatinAmerica = "Yes";
                        }
                        if (geo.GeoName == "United States")
                        {
                            data.UnitedStates = "Yes";
                        }
                    }
                    var solutionAreas = (from pArea in _context.PartnerSolutionByArea
                                         join ps in _context.PartnerSolution
                                         on pArea.PartnerSolutionId equals ps.PartnerSolutionId
                                         join area in _context.SolutionAreas
                                         on pArea.SolutionAreaId equals area.SolutionAreaId
                                         where ps.PartnerSolutionId == data.PartnerSolutionId
                                         select new IndustrySolutionAreaReport
                                         {
                                             SolutionAreaName = area.SolutionAreaName,
                                             SolutionAreaDescription = StripHTML(area.SolAreaDescription)
                                         }).ToList();
                    foreach (var solutionArea in solutionAreas)
                    {
                        if (data.SolutionAreaName == "")
                            data.SolutionAreaName = solutionArea.SolutionAreaName;
                        else
                            data.SolutionAreaName += " ;; " + solutionArea.SolutionAreaName;
                        if (data.SolutionAreaDescription == "")
                            data.SolutionAreaDescription = solutionArea.SolutionAreaDescription;
                        else
                            data.SolutionAreaDescription += " ;; " + solutionArea.SolutionAreaDescription;
                    }
                }
                var csvContent = new StringBuilder();

                var properties = typeof(IndustrySolutionFullReport).GetProperties();
                foreach (var prop in properties)
                {
                    if (prop.Name != "SolutionAreaName" && prop.Name != "SolutionAreaDescription" && prop.Name != "PartnerSolutionId")
                        csvContent.Append(prop.Name).Append(",");
                    else if ((prop.Name == "SolutionAreaName" || prop.Name == "SolutionAreaDescription"))
                        csvContent.Append(prop.Name).Append(",");
                }
                // Write the header row

                csvContent.Length--; // Remove the last comma
                csvContent.AppendLine();
                // Write the data rows
                foreach (var item in lst)
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Name != "SolutionAreaName" && prop.Name != "SolutionAreaDescription" && prop.Name != "PartnerSolutionId")
                        {
                            var value = prop.GetValue(item, null);
                            if (value != null)
                            {
                                value = RemoveSpecialCharacter(value.ToString());
                                csvContent.Append(value.ToString().Replace(",", "-").Replace("  ", " ")).Append(",");
                            }
                            else
                            {
                                csvContent.Append(",");
                            }
                        }
                        else if ((prop.Name == "SolutionAreaName" || prop.Name == "SolutionAreaDescription"))
                        {
                            var value = prop.GetValue(item, null);
                            if (value != null)
                            {
                                value = RemoveSpecialCharacter(value.ToString());
                                csvContent.Append(value.ToString().Replace(",", "-").Replace("  ", " ")).Append(",");
                            }
                            else
                            {
                                csvContent.Append(",");
                            }
                        }

                    }
                    csvContent.Length--; // Remove the last comma
                    csvContent.AppendLine();
                }
                // Return as byte array
                //Encoding.UTF8.GetBytes(csvContent.ToString());
                return File(Encoding.UTF8.GetBytes(csvContent.ToString()), "text/csv", "Report");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while downloading the reports. Please try again later.", Details = ex.Message });

            }
        }
        [HttpPost]
        [Route("DownloadIndustrySolutionReport")]
        public IActionResult DownloadIndustrySolutionReport(IndustryReportDropDownDTO SelectedValues)
        {
            try
            {
                IEnumerable<Guid> IndustryIds = SelectedValues.IndustryId.Select(x => x.IndustryId).Distinct();
                IEnumerable<Guid> IndustryThemeIds = SelectedValues.IndustryThemeId.Select(x => x.IndustryThemeId).Distinct();
                IEnumerable<Guid> SolutionAreaIds = SelectedValues.SolutionAreaId.Select(x => x.SolutionAreaId).Distinct();
                string encKey = configuration.GetSection("KnackSettings").GetSection("EncKey").Value;

                var utilities = new Utilities(configuration);
                var lst = (from ps in _context.PartnerSolution
                           join org in _context.Organization
                           on ps.OrganizationId equals org.OrgId
                           //join user in _context.PartnerUser
                           //on ps.UserId equals user.PartnerUserId
                           join ind in _context.Industries
                           on ps.IndustryId equals ind.IndustryId
                           join subInd in _context.SubIndustries
                           on ps.SubIndustryId equals subInd.SubIndustryId
                           join theme in _context.IndustryTheme
                           on subInd.SubIndustryId equals theme.SubIndustryId
                           join ss in _context.SolutionStatusType
                           on ps.SolutionStatusId equals ss.SolutionStatusId
                           /*join pArea in _context.PartnerSolutionByArea
                           on ps.PartnerSolutionId equals pArea.PartnerSolutionId
                           join area in _context.SolutionAreas
                           on pArea.SolutionAreaId equals area.SolutionAreaId
                           join pRLink in _context.PartnerSolutionResourceLink
                           on pArea.PartnerSolutionByAreaId equals pRLink.PartnerSolutionByAreaId into resources
                           from pRLink in resources.DefaultIfEmpty()
                           join rLink in _context.ResourceLinks
                           on pRLink.ResourceLinkId equals rLink.ResourceLinkId into resourceLink
                           from rLink in resourceLink.DefaultIfEmpty()*/
                           where ss.SolutionStatus == "Approved"
                           where IndustryIds.Count() > 0 ? IndustryIds.Contains(ind.IndustryId) : true
                           where IndustryThemeIds.Count() > 0 ? IndustryThemeIds.Contains(theme.IndustryThemeId) : true
                           //where SolutionAreaIds.Count() > 0 ? SolutionAreaIds.Contains(area.SolutionAreaId) : true
                           select new IndustrySolutionFullReport
                           {
                               OrgName = org.OrgName,
                               IndustryName = ind.IndustryName,
                               UseCase = subInd.SubIndustryName,
                               SolutionName = ps.SolutionName,
                               SolutionStatus = ss.SolutionStatus,
                               MarketPlaceLink = ps.MarketplaceLink,
                               SolutionDescription = StripHTML(ps.SolutionDescription),
                               SolutionOrgWebsite = ps.SolutionOrgWebsite,
                               SpecialOfferLink = ps.SpecialOfferLink,
                               Canada = "No",
                               LatinAmerica = "No",
                               UnitedStates = "No",
                               SolutionAreaName = "",//area.SolutionAreaName,
                               SolutionAreaDescription = "",//StripHTML(pArea.AreaSolutionDescription),
                               //ResourceLinkName = rLink.ResourceLinkName,
                               //ResourceLinkTitle = pRLink.ResourceLinkTitle,
                               //ResourceLinkUrl = pRLink.ResourceLinkUrl,
                               PartnerSolutionId = ps.PartnerSolutionId,
                               //RowChangedDate = ps.RowChangedDate,
                               CreatedDate = ps.RowCreatedDate
                           }).ToList();

                foreach (var data in lst)
                {
                    var geos = (from geoId in _context.PartnerSolutionAvailableGeo
                                join geo in _context.Geos
                                on geoId.GeoId equals geo.GeoId
                                where geoId.PartnerSolutionId == data.PartnerSolutionId
                                select new IndustrySolutionGeoReport
                                {
                                    GeoName = geo.Geoname
                                }).ToList();

                    foreach (var geo in geos)
                    {
                        if (geo.GeoName == "Canada")
                        {
                            data.Canada = "Yes";
                        }
                        if (geo.GeoName == "Latin America")
                        {
                            data.LatinAmerica = "Yes";
                        }
                        if (geo.GeoName == "United States")
                        {
                            data.UnitedStates = "Yes";
                        }
                    }

                    var solutionAreas = (from pArea in _context.PartnerSolutionByArea
                                join ps in _context.PartnerSolution
                                on pArea.PartnerSolutionId equals ps.PartnerSolutionId
                                join area in _context.SolutionAreas
                                on pArea.SolutionAreaId equals area.SolutionAreaId
                                where ps.PartnerSolutionId == data.PartnerSolutionId
                                select new IndustrySolutionAreaReport
                                {
                                    SolutionAreaName = area.SolutionAreaName,
                                    SolutionAreaDescription = StripHTML(area.SolAreaDescription)
                                }).ToList();
                    foreach (var solutionArea in solutionAreas)
                    {
                        if(data.SolutionAreaName == "")
                            data.SolutionAreaName = solutionArea.SolutionAreaName;
                        else
                            data.SolutionAreaName += " ;; " + solutionArea.SolutionAreaName;
                        if (data.SolutionAreaDescription == "")
                            data.SolutionAreaDescription = solutionArea.SolutionAreaDescription;
                        else
                            data.SolutionAreaDescription += " ;; " + solutionArea.SolutionAreaDescription;
                    }
                }
                var csvContent = new StringBuilder();

                var properties = typeof(IndustrySolutionFullReport).GetProperties();
                foreach (var prop in properties)
                {
                    if (prop.Name != "SolutionAreaName" && prop.Name != "SolutionAreaDescription" && prop.Name != "PartnerSolutionId")
                        csvContent.Append(prop.Name).Append(",");
                    else if ((prop.Name == "SolutionAreaName" || prop.Name == "SolutionAreaDescription") && SelectedValues.ExcludeSolutionArea == false)
                        csvContent.Append(prop.Name).Append(",");
                }
                // Write the header row

                csvContent.Length--; // Remove the last comma
                csvContent.AppendLine();
                // Write the data rows
                foreach (var item in lst)
                {
                    foreach (var prop in properties)
                    {
                        if (prop.Name != "SolutionAreaName" && prop.Name != "SolutionAreaDescription" && prop.Name != "PartnerSolutionId")
                        {
                            var value = prop.GetValue(item, null);
                            if (value != null)
                            {
                                value = RemoveSpecialCharacter(value.ToString());
                                csvContent.Append(value.ToString().Replace(",", "-").Replace("  ", " ")).Append(",");
                            }
                            else
                            {
                                csvContent.Append(",");
                            }
                        }
                        else if ((prop.Name == "SolutionAreaName" || prop.Name == "SolutionAreaDescription") && SelectedValues.ExcludeSolutionArea == false)
                        {
                            var value = prop.GetValue(item, null);
                            if (value != null)
                            {
                                value = RemoveSpecialCharacter(value.ToString());
                                csvContent.Append(value.ToString().Replace(",", "-").Replace("  ", " ")).Append(",");
                            }
                            else
                            {
                                csvContent.Append(",");
                            }
                        }

                    }
                    csvContent.Length--; // Remove the last comma
                    csvContent.AppendLine();
                }
                // Return as byte array
                //Encoding.UTF8.GetBytes(csvContent.ToString());
                return File(Encoding.UTF8.GetBytes(csvContent.ToString()), "text/csv", "Report");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while downloading the reports. Please try again later.", Details = ex.Message });

            }
        }
    }
}
