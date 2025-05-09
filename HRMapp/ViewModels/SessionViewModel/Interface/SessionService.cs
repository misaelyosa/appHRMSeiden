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
                await context.SaveChangesAsync();
                return forgotPwToken;
            }
        }

        public async Task<bool> CheckIsAdmin()
        {
            var user_token = await SecureStorage.Default.GetAsync("user_token");
            if (string.IsNullOrEmpty(user_token))
            {
                return false;
            }

            var context = await _contextFactory.CreateDbContextAsync();
            var session = await context.Session.FirstOrDefaultAsync(s => s.user_token == user_token);
            var user = await context.Users.FirstOrDefaultAsync(u => u.user_id == session.user_id);

            bool checkauth = user.authority.ToLower() == "admin";
            bool checktoken = user_token == session.user_token;
            Debug.WriteLine($"hasil check auth : {checkauth}");
            Debug.WriteLine($"hasil check token : {checktoken}");

            if (user.authority.ToLower() == "admin" && user_token == session.user_token)
            {
                Debug.WriteLine("admin datank");
                return true;
            } 
            return false;
        }


        //Reset Password
        public async Task<string?> ValidateForgotPasswordToken(string username, string token)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            var user = await context.Users.FirstOrDefaultAsync(u => u.username == username);
            var resettoken = await context.Users.FirstOrDefaultAsync(u => u.forgot_pass_token == token);
            if (user != null)
            {
                var userId = user.user_id.ToString();
                if (resettoken != null)
                {
                    Debug.WriteLine("ini true");
                    return userId;
                }
                await Application.Current.MainPage.DisplayAlert("Data tidak valid", "Token reset password yang diberikan salah.", "OK");
                return null;
            }
            await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", $"User dengan username {username} tidak ditemukan.", "OK");
            return null;        
        }    

        public async Task<string?> ResetPassword(string userId, string newPassword)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.FirstOrDefaultAsync(u => u.user_id == int.Parse(userId));

            if (!string.IsNullOrEmpty(userId) && user != null)
            {
                if (!string.IsNullOrEmpty(newPassword))
                {
                    var newPassHash = PasswordHasher.Hash(newPassword);
                    var newToken = Guid.NewGuid().ToString();
                    user.password_hash = newPassHash;
                    user.forgot_pass_token = newToken;

                    await context.SaveChangesAsync();
                    return newToken;
                }
                return null;
            }
            await Application.Current.MainPage.DisplayAlert("User ID not set", "Ulang proses verifikasi kembali", "OK");
            return null;
        }

        public async Task<string?> GetResetToken()
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var user_token = await SecureStorage.GetAsync("user_token");
            var session = await context.Session.FirstOrDefaultAsync(s => s.user_token == user_token);

            if (session != null)
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.user_id == session.user_id);

                if (user != null)
                {
                    return user.forgot_pass_token;
                }
                return null;
            }
            return null;
        }
    }   
}
