using HRMapp.ViewModels;
using System.Threading.Tasks;

namespace HRMapp.Pages;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _viewModel;
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.CheckIsAdmin();
        await _viewModel.LoadContractEndDatesAsync();
        await _viewModel.LoadContractEndPerMonthAsync(_viewModel.CalendarCurrentDate);
    }
}
