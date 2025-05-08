using CommunityToolkit.Mvvm.ComponentModel;
using HRMapp.ViewModels.SessionViewModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.SessionViewModel
{
    public partial class ResetPasswordViewModel : ObservableObject
    {
        private readonly ISessionService _sessionService;

        public ResetPasswordViewModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
    }
}
