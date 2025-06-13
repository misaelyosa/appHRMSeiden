using HRMapp.ViewModels;

namespace HRMapp.Pages;

public partial class ManageSessionUser : ContentPage
{
    private ManageSessionUserViewModel _viewModel;
    public ManageSessionUser(ManageSessionUserViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}