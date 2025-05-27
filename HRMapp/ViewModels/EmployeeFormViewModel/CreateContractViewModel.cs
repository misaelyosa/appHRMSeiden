using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Model;
using System.Diagnostics;

namespace HRMapp.ViewModels.EmployeeFormViewModel
{
    [QueryProperty(nameof(EmployeeId), "employeeId")]
    public partial class CreateContractViewModel : ObservableObject
    {
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private int employeeId;
        [ObservableProperty]
        private Employee selectedEmployee;
        [ObservableProperty]
        private DateOnly selectedContractDate;
        [ObservableProperty]
        private DateOnly selectedEndDate;
        [ObservableProperty]
        private string contractDuration;
        [ObservableProperty]
        private string contractNip;
        [ObservableProperty]
        private string gajiPokok;
        [ObservableProperty]
        private string tunjanganMK;
        [ObservableProperty]
        private string tunjanganOther;
        //proxy datetime
        public DateTime ContractDateTime
        {
            get => SelectedContractDate.ToDateTime(TimeOnly.MinValue);
            set => SelectedContractDate = DateOnly.FromDateTime(value);
        }

        public DateTime ContractEndDateTime
        {
            get => SelectedEndDate.ToDateTime(TimeOnly.MinValue);
            set => SelectedEndDate = DateOnly.FromDateTime(value);
        }

        partial void OnSelectedContractDateChanged(DateOnly value)
        {
            if (ContractDuration == null)
            {
                ContractEndDateTime = ContractDateTime;
                OnPropertyChanged(nameof(ContractEndDateTime));
            }
            else
            {
                UpdateContractEndDate();
            }
        }
        partial void OnContractDurationChanged(string value)
        {
            UpdateContractEndDate();
        }
        private void UpdateContractEndDate()
        {
            if (int.TryParse(ContractDuration, out var months))
            {
                SelectedEndDate = SelectedContractDate.AddMonths(months);
                OnPropertyChanged(nameof(ContractEndDateTime));
            }
        }

        public CreateContractViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task InitializeAsync()
        {
            var contractCount = await _employeeService.GetContractCountByEmployeeIdAsync(EmployeeId);
            var contractIndex = contractCount; //index contract mulai dari 1 

            var latestContract = await _employeeService.GetLastIndexContractDate(contractIndex, EmployeeId);
            
            if (latestContract != null)
            {
                SelectedContractDate = latestContract.end_date;
                OnPropertyChanged(nameof(ContractDateTime));
                ContractEndDateTime = ContractDateTime;
                OnPropertyChanged(nameof(ContractEndDateTime));
                Debug.WriteLine(latestContract.end_date);
            }
            else
            { 
                SelectedContractDate = DateOnly.FromDateTime(DateTime.Today);
                OnPropertyChanged(nameof(ContractDateTime));
                ContractEndDateTime = ContractDateTime;
                OnPropertyChanged(nameof(ContractEndDateTime));
                Debug.WriteLine(SelectedContractDate);
            }
        }

        [RelayCommand]
        private async Task CreateContract()
        {
            var formattedMessage = $"Data to be created:\n\n" +
               $"Contract Date   : {SelectedContractDate:dd/MM/yyyy}\n" +
               $"End Date        : {SelectedEndDate:dd/MM/yyyy}\n" +
               $"Duration        : {ContractDuration} bulan\n" +
               $"Gaji Pokok      : Rp. {int.Parse(GajiPokok):N0}\n"+
               (string.IsNullOrWhiteSpace(TunjanganMK) ? "" : $"Tunjangan MK    : Rp. {int.Parse(TunjanganMK):N0}\n") +
               (string.IsNullOrWhiteSpace(TunjanganOther) ? "" : $"Tunjangan ...   : Rp. {int.Parse(TunjanganOther):N0}\n");

            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Changes",
                formattedMessage,
                "Confirm",
                "Cancel"
            );

            if (!confirm)
                return;

            var contractCount = await _employeeService.GetContractCountByEmployeeIdAsync(EmployeeId);
            var contractIndex = contractCount + 1;

            var newContract = new Contract
            {
                employee_id = EmployeeId,
                contract_date = SelectedContractDate,
                contract_duration = int.Parse(ContractDuration),
                end_date = SelectedEndDate,
                gaji_pokok = int.Parse(GajiPokok),

                author = Preferences.Get("username", "admin"),
                created_at = DateTime.Now,

            };

            SelectedEmployee = await _employeeService.GetEmployeeByIdAsync(EmployeeId);
            var tunjanganList = new List<Tunjangan>();


            if (!string.IsNullOrWhiteSpace(TunjanganMK))
            {
                tunjanganList.Add(new Tunjangan
                {
                    tunjangan_name = $"TunjanganMK_{SelectedEmployee.nip}_{contractIndex}",
                    amount = int.Parse(TunjanganMK),
                });
            }

            if (!string.IsNullOrWhiteSpace(TunjanganOther))
            {
                tunjanganList.Add(new Tunjangan
                {
                    tunjangan_name = $"TunjanganOther_{SelectedEmployee.nip}_{contractIndex}",
                    amount = int.Parse(TunjanganOther)
                });
            }

            var latestContract = await _employeeService.GetLastIndexContractDate(contractIndex-1, EmployeeId);
            if (latestContract != null && latestContract.end_date > SelectedContractDate)
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Tanggal Kontrak baru tidak boleh kurang dari tanggal selesai kontrak sebelumnya.", "OK");
                return;
            }
            
            try
            {
                await _employeeService.CreateContractAsync(newContract);

                foreach(var tunjangan in tunjanganList)
                {
                    tunjangan.contract_id = newContract.contract_id;
                    await _employeeService.CreateTunjanganAsync(tunjangan);
                }
                Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Kontrak berhasil ditambahkan.", "OK");
                    await Shell.Current.GoToAsync("..");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update Contract failed: {ex.Message}");

                Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Gagal menambahkan data kontrak.", "OK");
                });
            }
        }
    }
}
