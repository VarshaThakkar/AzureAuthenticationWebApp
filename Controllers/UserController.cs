using AzureAuthenticationWebApp.Models;
using AzureAuthenticationWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAuthenticationWebApp.Controllers
{
    public class UserController : Controller
    {
        private IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [AuthorizeForScopes(ScopeKeySection = "UserService:_UserServiceScope")]
        public async Task<ActionResult> Index()
        {
            return View(await _userService.GetAsync());
        }
        // GET: User/Details/5
        public async Task<ActionResult> Details(int id)
        {
            return View(await _userService.GetAsync(id));
        }
        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Users user = await this._userService.GetAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Status,HasswordHash,PasswordSalt,Role")] Users user)
        {
            await _userService.EditAsync(user);
            return RedirectToAction("Index");
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(int id)
        {   
            Users user = await this._userService.GetAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, [Bind("Id,FirstName,LastName,Email,Status")] Users user)
        {            
            await _userService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
        //// GET: User/Create
        //public ActionResult Create()
        //{
        //    Customer customer = new Customer();
        //    return View(customer);
        //}
        
    }
}
