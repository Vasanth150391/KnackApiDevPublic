using System.Net;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Knack.API.Models;

namespace Knack.API.Components.ApiClient
{
    public class AppHttpClient
    {
        private readonly IConfiguration _configuration;
        public AppHttpClient(IConfiguration iConfig)
        {
            _configuration = iConfig;
        }
        public async Task<string> GenerateToken()
        {
            try
            {
                string username = "admin"; //_configuration.GetSection("LeadSettings:username").Value;
                string password = "xaexuJais6aeZahlae0H";//_configuration.GetSection("LeadSettings:password").Value;

                var httpClient = new HttpClient();
                //var authenticationString = $"username:{username},password:{password}";
                var authenticationString = Encoding.ASCII.GetBytes($"{username}:{password}");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", 
                    Convert.ToBase64String(authenticationString));
                var response = await httpClient.GetAsync("https://kcl-38vsf7krdd2zer9wrymqj2rggo.softwareng.cloud/api/v2/token");
                
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var actualResponse = await response.Content.ReadAsStringAsync();                    
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponseModel>(actualResponse);
                    return tokenResponse.AccessToken;
                }
                else
                {
                    throw new Exception("Error while generating the token");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<ResponseDTO> SendCreateUserRequest(SFTPGoModel sFTPGoModel, string token)
        {
            try
            {           
                var httpclient=new HttpClient();
                httpclient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(JsonConvert.SerializeObject(sFTPGoModel), Encoding.UTF8,
                                        "application/json");
                   // System.Diagnostics.Debug.WriteLine(content);
                    
                var response= await httpclient.PostAsync("https://kcl-38vsf7krdd2zer9wrymqj2rggo.softwareng.cloud/api/v2/users", content);
                var resultDTO = new ResponseDTO();
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode.ToString() == "Created")
                {
                    var result = await response.Content.ReadAsStringAsync();
                    resultDTO.Response = true;
                    resultDTO.Message = result;
                    return resultDTO;
                }
                else
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var res =  JsonConvert.DeserializeObject<ResponseErrortDTO>(result);
                    resultDTO.Response = false;
                    resultDTO.Message = res?.Error;
                    return resultDTO;
                }
            
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
