using HRMapp.ViewModels;

namespace HRMapp.Pages;

public partial class MainPage : ContentPage
{     
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
