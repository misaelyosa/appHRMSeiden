using CommunityToolkit.Maui.Views;
using HRMapp.Pages.EmployeeForms.Popups;
using HRMapp.ViewModels;

namespace HRMapp.Pages;

public partial class ManageReferenceDataPage : ContentPage
{
	private readonly ManageReferenceDataViewModel _viewModel;
	public ManageReferenceDataPage(ManageReferenceDataViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

    private async void OnDepartmentFrameTapped(object sender, EventArgs e)
    {
        if (!departmentCollectionView.IsVisible)
        {
            departmentCollectionView.Opacity = 0;
            departmentCollectionView.IsVisible = true;
            hideDeptBtn.IsVisible = true;
            deptListTitle.IsVisible = true;

            departmentFrame.IsVisible = false;
            await departmentCollectionView.FadeTo(1, 250);
        }
    }
    private async void OnHideDepartmentTapped(object sender, EventArgs e)
    {
        if (departmentCollectionView.IsVisible)
        {
            await departmentCollectionView.FadeTo(0, 250);
            departmentCollectionView.IsVisible = false;
            deptListTitle.IsVisible = false;
            hideDeptBtn.IsVisible = false;

            departmentFrame.IsVisible = true;
        }
    }
    
    //jobs 
    private async void OnJobsFrameTapped(object sender, EventArgs e)
    {
        if (!jobsCollectionView.IsVisible)
        {
            jobsCollectionView.Opacity = 0;
            await jobsCollectionView.FadeTo(1, 250);
            jobsCollectionView.IsVisible = true;
            hideJobsBtn.IsVisible = true;
            jobsListTitle.IsVisible = true;

            jobsFrame.IsVisible = false;
        }
    }
    private async void OnHideJobsTapped(object sender, EventArgs e)
    {
        if (jobsCollectionView.IsVisible)
        {
            await jobsCollectionView.FadeTo(0, 250);
            jobsCollectionView.IsVisible = false;
            hideJobsBtn.IsVisible = false;
            jobsListTitle.IsVisible = false;

            jobsFrame.IsVisible = true;
        }
    }

    //religion
    private async void OnReligionFrameTapped(object sender, EventArgs e)
    {
        if (!religionCollectionView.IsVisible)
        {
            religionCollectionView.Opacity = 0;
            await religionCollectionView.FadeTo(1, 250);
            religionFrame.IsVisible = false;

            religionCollectionView.IsVisible = true;
            religionListTitle.IsVisible = true;
            hideReligionBtn.IsVisible = true;
        }
    }
    private async void OnHideReligionTapped(object sender, EventArgs e)
    {
        if (religionCollectionView.IsVisible)
        {
            await religionCollectionView.FadeTo(0, 250);
            religionFrame.IsVisible = true;

            religionCollectionView.IsVisible = false;
            religionListTitle.IsVisible = false;
            hideReligionBtn.IsVisible = false;
        }
    }

    //factory
    private async void OnFactoryFrameTapped(object sender, EventArgs e)
    {
        if (!factoryCollectionView.IsVisible)
        {
            factoryCollectionView.Opacity = 0;
            await factoryCollectionView.FadeTo(1, 250);
            factoryFrame.IsVisible = false;

            factoryCollectionView.IsVisible = true;
            factoryListTitle.IsVisible = true;
            hideFactoryBtn.IsVisible = true;
        }
    }
    private async void OnHideFactoryTapped(object sender, EventArgs e)
    {
        if (factoryCollectionView.IsVisible)
        {
            await factoryCollectionView.FadeTo(0, 250);
            factoryFrame.IsVisible = true;

            factoryCollectionView.IsVisible = false;
            factoryListTitle.IsVisible = false;
            hideFactoryBtn.IsVisible = false;
        }
    }

    //City Province
    private async void OnCityProvFrameTapped(object sender, EventArgs e)
    {
        if (!cityProvCollectionView.IsVisible)
        {
            cityProvCollectionView.Opacity = 0;
            await cityProvCollectionView.FadeTo(1, 250);
            cityProvFrame.IsVisible = false;

            cityProvCollectionView.IsVisible = true;
            cityProvListTitle.IsVisible = true;
            hideCityProvBtn.IsVisible = true;
        }
    }
    private async void OnHideCityProvTapped(object sender, EventArgs e)
    {
        if (cityProvCollectionView.IsVisible)
        {
            await cityProvCollectionView.FadeTo(0, 250);
            cityProvFrame.IsVisible = true;

            cityProvCollectionView.IsVisible = false;
            cityProvListTitle.IsVisible = false;
            hideCityProvBtn.IsVisible = false;
        }
    }

    private async void OnOpenAddCityPopup(object sender, EventArgs e)
    {
        var popup = new NewCityProvince(this.BindingContext);
        this.ShowPopup(popup);
    }

    //Last Education
    private async void OnEducationFrameTapped(object sender, EventArgs e)
    {
        if (!educationCollectionView.IsVisible)
        {
            educationCollectionView.Opacity = 0;
            await educationCollectionView.FadeTo(1, 250);
            educationFrame.IsVisible = false;

            educationCollectionView.IsVisible = true;
            educationListTitle.IsVisible = true;
            hideEducationBtn.IsVisible = true;
        }
    }
    private async void OnHideEducationTapped(object sender, EventArgs e)
    {
        if (educationCollectionView.IsVisible)
        {
            await educationCollectionView.FadeTo(0, 250);
            educationFrame.IsVisible = true;

            educationCollectionView.IsVisible = false;
            educationListTitle.IsVisible = false;
            hideEducationBtn.IsVisible = false;
        }
    }

    private async void OnOpenAddEduPopup(object sender, EventArgs e)
    {
        var popup = new NewEducation(this.BindingContext);
        this.ShowPopup(popup);
    }


}