using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.SessionViewModel.Interface
{
    public interface ISessionService
    {
        Task<bool> IsUserLoggedInAsync();
        public string Username { get; }
        Task<bool> LoginAsync(string username, string password);
        Task LogoutAsync();

        //register
        Task<string?> RegisterAsync(string username, string password, string authority);
    }
}
