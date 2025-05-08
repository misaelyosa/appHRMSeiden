using CommunityToolkit.Mvvm.ComponentModel;
using HRMapp.ViewModels.SessionViewModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.SessionViewModel
{
    public partial class SignupViewModel : ObservableObject
    {
        private readonly ISessionService _sessionService;
        public SignupViewModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
    }
}
