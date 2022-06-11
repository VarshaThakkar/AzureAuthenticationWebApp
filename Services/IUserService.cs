using AzureAuthenticationWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAuthenticationWebApp.Services
{
    public interface IUserService
    {
        Task<IEnumerable<Users>> GetAsync();
        Task<Users> GetAsync(int id);

        Task DeleteAsync(int id);

        Task<bool> AddAsync(Customer customer);

        Task<Users> EditAsync(Users user);
        Task GetUserInfo();
    }
}
