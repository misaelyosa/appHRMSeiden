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
            var user_token = await SecureStorage.GetAsync("user_token");
            if (string.IsNullOrEmpty(user_token))
            {
                return false;
            }
            else
            {
                using var context = await _contextFactory.CreateDbContextAsync();

                return await context.Session.AnyAsync(s => s.user_token == user_token);
            }

        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var token = Guid.NewGuid().ToString();
            var user = await context.Users.FirstOrDefaultAsync(e => e.username == username && e.password_hash == password);
            
            if (user != null)
            {
                var existingSession = await context.Session.FirstOrDefaultAsync(s => s.user_id == user.user_id);

                if (existingSession != null)
                {
                    existingSession.last_login = DateTime.Now;
                    context.Update(existingSession);
                } 
                else
                {
                    var addSession = new Session
                    {
                        user_id = user.user_id,
                        user_token = token,
                        last_login = DateTime.Now,
                    };

                    context.Add(addSession);
                }

                await context.SaveChangesAsync();
                await SecureStorage.Default.SetAsync("user_token", token);
                return true;
            }
            return false;
        }

        public async Task LogoutAsync()
        {
            var token = await SecureStorage.GetAsync("user_token");

            if (!string.IsNullOrEmpty(token))
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var session = await context.Session.FirstOrDefaultAsync(s => s.user_token == token);
                if (session != null)
                {
                    context.Session.Remove(session);
                    await context.SaveChangesAsync();
                }

                SecureStorage.Default.Remove("user_token");
            }
        }
    }
}
