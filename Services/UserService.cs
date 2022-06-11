using AzureAuthenticationWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AzureAuthenticationWebApp.Services
{
    public static class UserServiceExtension
    {
        public static void AddUserService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IUserService, UserService>();
        }
    }
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _httpClient;
        private readonly string _UserServiceScope = string.Empty;
        private readonly string _UserServiceBaseAddress = string.Empty;
        private readonly ITokenAcquisition _tokenAcquisition;

        public UserService(IConfiguration configuration, IHttpContextAccessor contextAccessor, HttpClient httpClient,
            ITokenAcquisition tokenAcquisition)
        {
            _contextAccessor = contextAccessor;
            _httpClient = httpClient;
            _UserServiceScope = configuration["UserService:UserServiceScope"];
            _UserServiceBaseAddress = configuration["UserService:UserServiceBaseAddress"];
            _tokenAcquisition = tokenAcquisition;
        }
        private async Task PrepareAuthenticatedClient()
        {
            string[] scopes = new string[] { _UserServiceScope };

            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            Debug.WriteLine($"access token-{accessToken}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<IEnumerable<Users>> GetAsync()
        {
            await PrepareAuthenticatedClient();

            //// var newuser = _contextAccessor.HttpContext.User.Claims;
            //var claimsIdentity = new ClaimsIdentity();
            //claimsIdentity.AddClaim(_contextAccessor.HttpContext.User.FindFirst("newUser"));
            //claimsIdentity.AddClaim(_contextAccessor.HttpContext.User.FindFirst("emails"));
            //claimsIdentity.AddClaim(_contextAccessor.HttpContext.User.
            //    FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"));
            //var userinfo = new ClaimsPrincipal(claimsIdentity);
            ////IEnumerable<Customer> customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(userinfo);

            //  await GetUserInfo();

            var response = await _httpClient.GetAsync($"{ _UserServiceBaseAddress}/api/Protected");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<Users> userlist = JsonConvert.DeserializeObject<IEnumerable<Users>>(content);

                return userlist;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
        public async Task DeleteAsync(int id)
        {
            await PrepareAuthenticatedClient();

            var response = await this._httpClient.DeleteAsync($"{ _UserServiceBaseAddress}/api/Protected/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
        public async Task<Users> GetAsync(int id)
        {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync($"{ _UserServiceBaseAddress}/api/Protected/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                Users user = JsonConvert.DeserializeObject<Users>(content);

                return user;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
        public async Task<Users> EditAsync(Users user)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(user);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

            var response = await _httpClient.PatchAsync($"{ _UserServiceBaseAddress}/api/Protected/{user.Id}", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<Users>(content);

                return user;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<bool> AddAsync(Customer customer)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(customer);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await this._httpClient.PostAsync($"{ _UserServiceBaseAddress}/api/protected", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                customer = JsonConvert.DeserializeObject<Customer>(content);

                return true;
            }
            return false;
            // throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
        public async Task GetNewUserInfoAndCreate()
        {
            var claimnewUser = _contextAccessor.HttpContext.User.Claims;
            foreach (var userinfo in claimnewUser)
            {
                if (userinfo == _contextAccessor.HttpContext.User.FindFirst("newUser"))
                {
                    var newuser = _contextAccessor.HttpContext.User.FindFirst("newUser").Value;

                    if (newuser == "true")
                    {
                        var email = _contextAccessor.HttpContext.User.FindFirst("emails").Value;
                        var objectid = _contextAccessor.HttpContext.User.
                            FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                        var customerinfo = new Customer
                        {
                            ObjectId = objectid,
                            Email = email
                        };
                        await AddAsync(customerinfo);

                    }
                }
            }
            return;
        }
    }
}
