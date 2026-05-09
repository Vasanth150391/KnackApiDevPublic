using Knack.API.Common;
using System.Configuration;
using Knack.API.Data;
using Knack.API.Interfaces;
using Knack.DBEntities;
using Microsoft.Extensions.Configuration;
using Knack.API.Components.ApiClient;
using Knack.API.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Azure;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Knack.API.DataManagers
{
    public class LeadGeneratorManager : ILeadGeneratorManager
    {
        private readonly ILogger<PartnerSolutionManager> _logger;
        private readonly KnackContext _knackContext;
        private readonly IConfiguration _configuration;
        private readonly string _basefilePath = string.Empty;
        public LeadGeneratorManager(ILogger<PartnerSolutionManager> logger,
            KnackContext knackContext,
            IConfiguration configuration)
        {
            _logger = logger;
            _knackContext = knackContext;
            _configuration = configuration;
            _basefilePath = Directory.GetCurrentDirectory() + "\\Mail_Templates";
        }
        public async Task<ResponseDTO> RegisterLeadPartner(LeadPartner leadPartnerModel)
        {
            try
            {
                _logger.LogDebug("Entering the method RegisterLeadPartner");
                var utilities = new Utilities(_configuration);
                string encKey = _configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
                string emailAddress = utilities.EncryptString(encKey, leadPartnerModel.EmailAddress);
                var checkUsername = _knackContext.LeadPartners.Where(user => user.EmailAddress == emailAddress).FirstOrDefault();
                if(checkUsername == null)
                {

                    var httpApiclient = new AppHttpClient(_configuration);
                    var token = await httpApiclient.GenerateToken();
                    var sftpUserDetails = new SFTPGoModel();
                    sftpUserDetails.username = leadPartnerModel.EmailAddress;
                    sftpUserDetails.password = CreateRandomPassword(15);
                    string permissionVal = "{ '/' : ['list', 'upload', 'download'] }";
                    JObject permissionJson = JObject.Parse(permissionVal);
                    sftpUserDetails.permissions = permissionJson;
                    sftpUserDetails.status = 1;

                    var fileSystem = new FileSystem();
                    var azBlobConfig = new AzBlobConfig();
                    var accountKey = new AccountKey();

                    accountKey.status = "Plain";
                    accountKey.payload = _configuration.GetSection("SFTPGoConnetionString").GetSection("payload").Value;
                    azBlobConfig.container = _configuration.GetSection("SFTPGoConnetionString").GetSection("container").Value;
                    azBlobConfig.account_name = _configuration.GetSection("SFTPGoConnetionString").GetSection("account_name").Value;
                    azBlobConfig.account_key = accountKey;
                    azBlobConfig.key_prefix = "input/" + leadPartnerModel.CompanyName + "/";
                    fileSystem.provider = 3;
                    fileSystem.azblobconfig = azBlobConfig;

                    sftpUserDetails.filesystem = fileSystem;
                    var response = await httpApiclient.SendCreateUserRequest(sftpUserDetails, token);
                    if (response.Response == true)
                    {
                        leadPartnerModel.SFTPGoResponse = response.Message;
                        leadPartnerModel.SFTPGoRequest = JsonConvert.SerializeObject(sftpUserDetails);


                        var filePath = _basefilePath + "\\Lead_Registration.html";
                        var emailBody = File.ReadAllText(filePath);
                        emailBody = emailBody.Replace("#firstName#", leadPartnerModel.FirstName);
                        emailBody = emailBody.Replace("#lastName#", leadPartnerModel.LastName);
                        emailBody = emailBody.Replace("#loginUrl#", "https://kcl-38vsf7krdd2zer9wrymqj2rggo.softwareng.cloud/web/client/login");
                        emailBody = emailBody.Replace("#userName#", leadPartnerModel.EmailAddress);
                        emailBody = emailBody.Replace("#password#", sftpUserDetails.password);

                        
                        utilities.SendLeadMail(leadPartnerModel.EmailAddress.ToString(), "Registration Success", emailBody, true);

                        
                        leadPartnerModel.FirstName = utilities.EncryptString(encKey, leadPartnerModel.FirstName);
                        leadPartnerModel.LastName = utilities.EncryptString(encKey, leadPartnerModel.LastName);
                        leadPartnerModel.EmailAddress = utilities.EncryptString(encKey, leadPartnerModel.EmailAddress);
                        leadPartnerModel.FtpuserName = utilities.EncryptString(encKey, leadPartnerModel.EmailAddress);
                        leadPartnerModel.FTPFolderName = leadPartnerModel.CompanyName;
                        var result = await _knackContext.AddAsync(leadPartnerModel);
                        await _knackContext.SaveChangesAsync();
                        return response;
                    }
                    else
                    {
                        return response;
                    }
                }
                else
                {
                    var response = new ResponseDTO();
                    response.Response = false;
                    response.Message = "Email Address already exists";
                    return response;
                }

                   //return leadPartnerModel.LeadPartnerId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occurred in method RegisterLeadPartner {ex.InnerException}");

                throw;
            }
        }
        private static string CreateRandomPassword(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                //chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
                chars[i] = allowedChars[RandomNumberGenerator.GetInt32(allowedChars.Length)];
            }

            return new string(chars);
        }
    }
}
