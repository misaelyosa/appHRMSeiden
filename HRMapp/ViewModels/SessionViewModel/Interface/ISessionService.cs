﻿using HRMapp.Data.Model;
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
        Task UpdateLastLogin();

        //register
        Task<string?> RegisterAsync(string username, string password, string authority);
        Task<bool> CheckIsAdmin();

        //reset password
        Task<string?> ValidateForgotPasswordToken(string username, string token);
        Task<string?> ResetPassword(string userId, string newPassword);
        Task<string?> GetResetToken();

        //manage user session page
        Task<List<UserSessionDTO>> GetAllUserSessionsAsync();
        Task<bool> TerminateSessionAsync(int sessionId, bool isAdmin);
    }
}
