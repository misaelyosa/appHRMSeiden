using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Model;
using HRMapp.ViewModels.SessionViewModel.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels
{
    public partial class ManageSessionUserViewModel : ObservableObject
    {
        private readonly ISessionService _sessionService;
        public ManageSessionUserViewModel(ISessionService sessionService)
        {   
            _sessionService = sessionService;
            _ = LoadDataAsync();
        }

        [ObservableProperty]
        private ObservableCollection<UserSessionDTO> userSessions = new();
        [ObservableProperty]
        private bool isAdmin;

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            IsAdmin = await _sessionService.CheckIsAdmin();
            Debug.WriteLine($"{isAdmin}");

            var data = await _sessionService.GetAllUserSessionsAsync();
            UserSessions = new ObservableCollection<UserSessionDTO>(data);
        }

        [RelayCommand]
        private async Task TerminateUserSessionAsync(UserSessionDTO selectedSession)
        {
            if (selectedSession == null)
                return;

            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Konfirmasi", $"Ingin mengakhiri sesi untuk {selectedSession.Username}?", "Ya", "Tidak");

            if (!confirm)
                return;

            var result = await _sessionService.TerminateSessionAsync(selectedSession.SessionId, IsAdmin);
            if (result)
            {
                await Application.Current.MainPage.DisplayAlert("Berhasil", "Sesi telah dinonaktifkan.", "OK");
                await LoadDataAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Gagal", "Gagal menonaktifkan sesi.", "OK");
            }
        }
    }
}
