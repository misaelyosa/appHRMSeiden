using HRMapp.Data.Database;
using HRMapp.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.SessionViewModel.Interface
{
    public class SessionService : ISessionService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public string Username { get; private set; } = string.Empty;

        public SessionService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> IsUserLoggedInAsync()
        {
            try
            {
                var user_token = await SecureStorage.Default.GetAsync("user_token");

                if (string.IsNullOrWhiteSpace(user_token))
                {
                    Debug.WriteLine("user_token is null or empty.");
                    return false;
                }

                using var context = await _contextFactory.CreateDbContextAsync();
                var session = await context.Session.FirstOrDefaultAsync(s => s.user_token == user_token);

                if (session == null)
                {
                    Debug.WriteLine("Session not found in database.");
                    SecureStorage.Default.Remove("user_token");
                    return false;
                }

                if (session.last_login == default)
                {
                    Debug.WriteLine("session.last_login is default.");
                    SecureStorage.Default.Remove("user_token");
                    return false;
                }

                if (session.status != Sessionstatus.Active)
                {
                    Debug.WriteLine($"current session status is inactive");
                    SecureStorage.Default.Remove("user_token");
                    return false;
                }
                    
                if ((DateTime.Now - session.last_login).TotalHours > 2)
                {
                    session.status = Sessionstatus.Inactive;
                    await context.SaveChangesAsync();
                    SecureStorage.Default.Remove("user_token");
                    Debug.WriteLine(" Session timed out.");
                    return false;
                }

                Debug.WriteLine(" User is logged in.");
                var user = await context.Users.FirstOrDefaultAsync(u => u.user_id == session.user_id);
                Username = user.username;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" Exception in IsUserLoggedInAsync: {ex}");
                return false;
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var token = Guid.NewGuid().ToString();
            var user = await context.Users.FirstOrDefaultAsync(e => e.username == username);
            
            if (user != null)
            {
                var storedPassword = user.password_hash;
                var isPassMatch = PasswordHasher.Verify(password, storedPassword);

                if (isPassMatch)
                {
                    Username = user.username;
                    var existingSession = await context.Session.FirstOrDefaultAsync(s => s.user_id == user.user_id);

                    if (existingSession != null)
                    {
                        existingSession.user_token = token;
                        existingSession.last_login = DateTime.Now;
                        existingSession.status = Sessionstatus.Active;
                        context.Update(existingSession);
                    } 
                    else
                    {
                        var addSession = new Session
                        {
                            user_id = user.user_id,
                            user_token = token,
                            last_login = DateTime.Now,
                            status = Sessionstatus.Active
                        };

                        context.Add(addSession);
                    }

                    await context.SaveChangesAsync();
                    await SecureStorage.Default.SetAsync("user_token", token);
                    return true;
                }
                return false;
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
                    session.status = Sessionstatus.Inactive;
                    await context.SaveChangesAsync();
                }

                SecureStorage.Default.Remove("user_token");
            }
        }

        public async Task<string?> RegisterAsync(string username, string password, string authority)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var hashedPw = PasswordHasher.Hash(password);
            var forgotPwToken = Guid.NewGuid().ToString();

            if (string.IsNullOrEmpty(authority))
            {
                var newUser = new User
                {
                    username = username,
                    password_hash = hashedPw,
                    authority = "user",
                    forgot_pass_token = forgotPwToken
                };
                context.Users.Add(newUser);
            } 
            else
            {
                var newAdmin = new User
                {
                    username = username,
                    password_hash = hashedPw,
                    authority = authority,
                    forgot_pass_token = forgotPwToken
                };
                context.Users.Add(newAdmin);
            }

            bool isDuplicate = await context.Users.AnyAsync(u => u.username == username);
            if (isDuplicate)
            {
                await Application.Current.MainPage.DisplayAlert("Gagal Register", $"username {username} sudah pernah digunakan.", "OK");
                return null;
            }
            else
            {
                //await context.SaveChangesAsync();
                return forgotPwToken;
            }
        }
    }
}
