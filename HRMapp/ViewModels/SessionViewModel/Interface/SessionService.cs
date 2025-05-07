using HRMapp.Data.Database;
using HRMapp.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.SessionViewModel.Interface
{
    public class SessionService : ISessionService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public SessionService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> IsUserLoggedInAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Session.AnyAsync();
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var user = await context.Users.FirstOrDefaultAsync(e => e.username == username && e.password_hash == password);
           
            if (user != null)
            {
                var addSession = new Session
                {   
                    user_id = user.user_id,
                    user_token = user.username,
                    last_login = DateTime.Now,
                };

                context.Add(addSession);
                await context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task LogoutAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Session.RemoveRange(context.Session);
            await context.SaveChangesAsync();
        }
    }
}
