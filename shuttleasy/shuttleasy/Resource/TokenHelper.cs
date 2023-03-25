using Microsoft.Net.Http.Headers;
using Org.BouncyCastle.Asn1.Ocsp;
using shuttleasy.DAL.Models;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.DAL.Models.dto.Credentials.dto;
using System.IdentityModel.Tokens.Jwt;

namespace shuttleasy.Resource
{
    public static class TokenHelper
    {
        public static int GetUserIdFromRequestToken(IHeaderDictionary headers)
        {
            string requestToken = headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(requestToken);
            string user = jwt.Claims.First(c => c.Type == "id").Value;
            int userId = int.Parse(user);
            return userId;
        }
        public static int GetDriverIdFromRequestToken(IHeaderDictionary headers)
        {
            string requestToken = headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(requestToken);
            string user = jwt.Claims.First(c => c.Type == "id").Value;
            int userId = int.Parse(user);
            return userId;
        }
        public static string GetUserRoleFromRequestToken(IHeaderDictionary headers)
        {
            string requestToken = headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(requestToken);
            string userEmail = jwt.Claims.First(c => c.Type == "role").Value;
            return userEmail;
        }
        public static string GetUserTokenFromRequestToken(IHeaderDictionary headers)
        {
            string requestToken = headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            return requestToken;
        }

        public static CompanyWorker? GetCompanyWorkerFromRequestToken(IHeaderDictionary headers,ICompanyWorkerLogic _driverLogic)
        {
            string requestToken = headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            CompanyWorker? companyWorkerFromToken = _driverLogic.GetCompanyWorkerWithToken(requestToken);
            return companyWorkerFromToken;
        }
        public static Passenger? GetUserFromRequestToken(IHeaderDictionary headers, IPassengerLogic _passengerLogic)
        {
            string requestToken = headers[HeaderNames.Authorization].ToString().Replace("bearer ", "");
            Passenger? passengerFromToken = _passengerLogic.GetPassengerWithToken(requestToken);
            return passengerFromToken;
        }
        public static UserVerifyingDto GetUserInformation(IHeaderDictionary headers)
        {
            UserVerifyingDto userVerifyingDto = new UserVerifyingDto();
            userVerifyingDto.Id = GetUserIdFromRequestToken(headers);
            userVerifyingDto.Token = GetUserTokenFromRequestToken(headers);
            userVerifyingDto.Role = GetUserRoleFromRequestToken(headers);
            return userVerifyingDto;
        }
    }
}
