using Azure.Identity;
using AzureAuthenticationWebApp.Models;
using AzureAuthenticationWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AzureAuthenticationWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUserService _userService;
        private readonly ITokenAcquisition tokenAcquisition;


        public HomeController(ILogger<HomeController> logger, ITokenAcquisition tokenAcquisition, IUserService userService)
        {
            _logger = logger;
            this.tokenAcquisition = tokenAcquisition;
            _userService = userService;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Customer customer)
        {
            await _userService.AddAsync(customer);
            return RedirectToAction("Index");
        }
        public IActionResult Claims()
        {
            return View();
        }

        public IActionResult Index()
        {
            _userService.GetNewUserInfoAndCreate();
            return View();
        }
        //public IList<MSGraphUser> Users { get; set; }
        //public async Task OnGetAsync()
        //{
        //    // Create the Graph service client with a ChainedTokenCredential which gets an access
        //    // token using the available Managed Identity or environment variables if running
        //    // in development.
        //    var credential = new ChainedTokenCredential(
        //        new ManagedIdentityCredential(),
        //        new EnvironmentCredential());
        //    var token = credential.GetToken(
        //        new Azure.Core.TokenRequestContext(
        //            new[] { "https://graph.microsoft.com/.default" }));

        //    var accessToken = token.Token;
        //    var graphServiceClient = new GraphServiceClient(
        //        new DelegateAuthenticationProvider((requestMessage) =>
        //        {
        //            requestMessage
        //            .Headers
        //            .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

        //            return Task.CompletedTask;
        //        }));

        //    List<MSGraphUser> msGraphUsers = new List<MSGraphUser>();
        //    try
        //    {
        //        var users = await graphServiceClient.Users.Request().GetAsync();
        //        foreach (var u in users)
        //        {
        //            MSGraphUser user = new MSGraphUser();
        //            user.userPrincipalName = u.UserPrincipalName;
        //            user.displayName = u.DisplayName;
        //            user.mail = u.Mail;
        //            user.jobTitle = u.JobTitle;

        //            msGraphUsers.Add(user);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = ex.Message;
        //    }

        //    Users = msGraphUsers;
        //}
        public IActionResult Privacy()
        {
            return View();
        }
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
