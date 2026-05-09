using Knack.API.Models;
using Knack.DBEntities;
using System.Runtime.CompilerServices;

namespace Knack.API.Mapping
{
    public static class LeadGeneratorMapping
    {
        public static LeadPartner ToLeadPartnerModel(this LeadPartnerDTO leadPartnerDTO)
        {
			try
			{
				return new LeadPartner
				{
					FirstName = leadPartnerDTO.FirstName,
					LastName = leadPartnerDTO.LastName,
					EmailAddress = leadPartnerDTO.EmailAddress,
					CompanyName = leadPartnerDTO.CompanyName,
					Createdon = DateTime.Now,
					PhoneNumber = leadPartnerDTO.PhoneNumber,
					FtpuserName = leadPartnerDTO.FTPUserName,
					Status = leadPartnerDTO.Status
				};
			}
			catch (Exception ex)
			{

				throw ex;
			}
        }
    }
}
