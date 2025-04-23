using CommunityToolkit.Mvvm.ComponentModel;
using HRMapp.Data.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Previewer;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace HRMapp.ViewModels.EmployeeFormViewModel
{
    [QueryProperty(nameof(EmployeeId), "employeeId")]
    [QueryProperty(nameof(ContractId), "contractId")]
    public partial class GeneratePKWTPageViewModel : ObservableObject
    {
        private readonly IEmployeeService _employeeService;
        [ObservableProperty]
        private int employeeId;

        [ObservableProperty]
        private int contractId;

        [ObservableProperty]
        private Contract selectedContract;
        [ObservableProperty]
        private Employee selectedEmployee;

        private string fileName;


        public GeneratePKWTPageViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        //update contract id value di vm karna navigation lewat shell ga inject ke viewmodel (harus diupdate manual)
        partial void OnContractIdChanged(int value)
        {
            _ = LoadContractDetailAsync(value);
        }

        [RelayCommand]
        private async Task OpenPdfExternal()
        {
            var folderPath = @"F:\Coolyeah\G_Smt 6\tesPdf";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var trimEmp = string.IsNullOrWhiteSpace(SelectedEmployee?.name)
                    ? "Unknown"
                    : SelectedEmployee.name.Replace(" ", "");

            var contractDate = SelectedContract?.contract_date.ToString("dd-MM-yyyy") ?? "UnknownDate";
            var nip = SelectedContract?.contract_nip ?? "UnknownNIP";

            fileName = $"PKWT_{nip}_{trimEmp}_{contractDate}.pdf";
            var fullPath = Path.Combine(folderPath, fileName);


            if (File.Exists(fullPath))
            {
                try
                {
                    await Launcher.OpenAsync(new Uri(fullPath));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error opening PDF: {ex.Message}");

                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to open the PDF file.", "OK");
                }
            }
            else
            {
                Debug.WriteLine("PDF file does not exist at path: " + fullPath);

                await Application.Current.MainPage.DisplayAlert("File Not Found", "The PDF file does not exist. Please generate it first.", "OK");
            }
        }

        [RelayCommand]
        private void CreatePdf()
        {
            var folderPath = @"F:\Coolyeah\G_Smt 6\tesPdf";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var trimEmp = string.IsNullOrWhiteSpace(SelectedEmployee?.name)
                    ? "Unknown"
                    : SelectedEmployee.name.Replace(" ", "");

            var contractDate = SelectedContract?.contract_date.ToString("dd-MM-yyyy") ?? "UnknownDate";
            var nip = SelectedContract?.contract_nip ?? "UnknownNIP";

            fileName = $"PKWT_{nip}_{trimEmp}_{contractDate}.pdf";
            var fullPath = Path.Combine(folderPath, fileName);

            try
            {
                GeneratePdf(fullPath);
                Debug.WriteLine("PDF created at: " + fullPath);
                Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("PDF Created", "The PDF file was successfully generated.", "OK");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PDF creation failed: {ex.Message}");
                
                Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to generate the PDF file.", "OK");
                });
            }
        }

        public async Task LoadContractDetailAsync(int contractId)
        {
            var contract = await _employeeService.GetContractDetail(contractId);
            if (contract != null)
            {
                SelectedContract = contract;
                SelectedEmployee = await _employeeService.GetEmployeeByIdAsync(contract.employee_id);
                Debug.WriteLine(SelectedContract.contract_date);
                Debug.WriteLine(SelectedEmployee.name);
            } 
        }

        partial void OnSelectedContractChanged(Contract value)
        {
            ContractNip = value.contract_nip.ToString();
            ContractDuration = value.contract_duration.ToString();
            GajiPokok = value.gaji_pokok.ToString();

            SelectedContractDate = value.contract_date;
            SelectedEndDate = value.end_date;

            OnPropertyChanged(nameof(ContractDateTime));
            OnPropertyChanged(nameof(ContractEndDateTime));

            UpdateContractEndDate();
        }

        partial void OnSelectedContractDateChanged(DateOnly value)
        {
            UpdateContractEndDate();
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

        [RelayCommand]
        private async Task EditContractDetail()
        {
            var formattedMessage = $"Data to be updated:\n\n" +
                           $"NIP             : {ContractNip}\n" +
                           $"Contract Date   : {SelectedContractDate:dd/MM/yyyy}\n" +
                           $"End Date        : {SelectedEndDate:dd/MM/yyyy}\n" +
                           $"Duration        : {ContractDuration} bulan\n" +
                           $"Gaji Pokok      : Rp. {int.Parse(GajiPokok):N0}";

            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Changes",
                formattedMessage,
                "Confirm",
                "Cancel"
            );

            if (!confirm)
                return;
            
            var updatedContract = new Contract
            {
                contract_id = SelectedContract.contract_id,
                employee_id = SelectedContract.employee_id,

                contract_date = SelectedContractDate,
                end_date = SelectedEndDate,
                contract_nip = ContractNip,
                gaji_pokok = int.TryParse(GajiPokok, out var gaji) ? gaji : 0,
                contract_duration = int.TryParse(ContractDuration, out var durasi) ? durasi : 0,

                updated_at = DateTime.Now,
                author = "admin" //todo --> kalo uda ada session ganti 
            };

            try
            {
                await _employeeService.UpdateContractAsync(updatedContract);

                await LoadContractDetailAsync(updatedContract.contract_id);
                Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Kontrak berhasil diupdate.", "OK");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update Contract failed: {ex.Message}");

                Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Gagal mengupdate data kontrak.", "OK");
                });
            }
            
        }

        //proxy buat convert datetime (compatible with datepicker)

        public DateTime ContractDateTime
        {
            get => SelectedContractDate.ToDateTime(TimeOnly.MinValue);
            set
            {
                SelectedContractDate = DateOnly.FromDateTime(value);
                OnPropertyChanged(nameof(ContractDateTime));
            }
        }

        public DateTime ContractEndDateTime
        {
            get =>     SelectedEndDate.ToDateTime(TimeOnly.MinValue);


            set
            {
                if (SelectedContract != null)
                    SelectedContract.end_date = DateOnly.FromDateTime(value);
            }
        }

        public void GeneratePdf(string outputPath)
        {
            Debug.WriteLine("Checking contract...");
            Debug.WriteLine($"SelectedContract: {SelectedContract.contract_id}");
            Debug.WriteLine($"Employee: {SelectedEmployee.employee_id}");
            if (SelectedContract == null || SelectedEmployee == null)
                return;

            var contract = SelectedContract;
            var employee = SelectedEmployee;

            var startDate = contract.contract_date;
            var endDate = contract.end_date;
            var today = DateTime.Today;
            var indo = new System.Globalization.CultureInfo("id-ID");

            var formattedStart = startDate.ToString("dd MMMM yyyy", indo);
            var formattedEnd = endDate.ToString("dd MMMM yyyy", indo);
            var printedToday = today.ToString("dd MMMM yyyy", indo);
            var formattedBirthdate = employee.birthdate.ToString("dd MMMM yyyy", indo);
            var dayToday = today.ToString("dddd", indo);
            var Duration = contract?.contract_duration ?? 0;
            string spellDuration = Terbilang(Duration).Trim();

            long gaji = contract.gaji_pokok;
            string currencyFormat(int amount) => $"Rp. {amount:N0}";
            string spellGaji = Terbilang(gaji).Trim() + " rupiah";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10).LineHeight(1.0f).FontFamily("Times New Roman"));
                    page.Content().Column(col =>
                    {
                        col.Spacing(5);

                        void Bullet(string text)
                        {
                            col.Item().Row(row =>
                            {
                                row.AutoItem().Text("•");
                                row.RelativeItem().Text(text);
                            });
                        }

                        col.Item().AlignCenter().Text("PERJANJIAN KERJA WAKTU TERTENTU").FontSize(13).Bold().Underline();
                        col.Item().AlignCenter().Text($"Nomor : {contract.contract_nip ?? employee.nip} B / SEI-PKWT / I / 2025");

                        col.Item().Text(text =>
                        {
                            text.Span("Perjanjian Kerja Waktu Tertentu/PKWT ");
                            text.Span("(selanjutnya disebut ‘Perjanjian’)").Bold();
                            text.Span($" ini dibuat dan ditandatangani pada hari ini {dayToday} tanggal {printedToday} oleh dan antara:");
                        });

                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("1.").AlignCenter();
                            row.RelativeItem(2).Text("Nama").AlignLeft();
                            row.RelativeItem(9).Text(":     Andreas Rikky R");
                        });                   
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("");
                            row.RelativeItem(2).Text("Jabatan").AlignLeft();
                            row.RelativeItem(9).Text(":     HRD");
                        });                        
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("");
                            row.RelativeItem(2).Text("Alamat").AlignLeft();
                            row.RelativeItem(9).Text(":     Jl. P.TP. Ngobo XVIII, Dusun Ngimbun, RT/RW 1/3, Kelurahan Karangjati, Kecamatan Bergas, Kabupaten Semarang, Jawa Tengah.");
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("Dalam jabatannya tersebut di atas, bertindak untuk dan atas nama PT. SEIDENSTICKER INDONESIA, selanjutnya disebut ");
                            text.Span("PIHAK PERTAMA (Pemberi Kerja/Perusahaan).").Bold();
                        });

                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("2.").AlignCenter();
                            row.RelativeItem(2).Text("Nama");
                            row.RelativeItem(9).Text($":    {employee.name}");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("");
                            row.RelativeItem(2).Text("Tempat, Tgl Lahir");
                            row.RelativeItem(9).Text($":    {employee.birthplace}, {formattedBirthdate}");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("");
                            row.RelativeItem(2).Text("Alamat");
                            row.RelativeItem(9).Text($":    {employee.address}");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("");
                            row.RelativeItem(2).Text("Nomor KTP");
                            row.RelativeItem(9).Text($":    {employee.nik}");
                        });

                        col.Item().Text(text =>
                        {
                            text.Span("Bertindak untuk dan atas nama diri sendiri, selanjutnya disebut sebagai ");
                            text.Span("PIHAK KEDUA (Penerima Kerja).").Bold();
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("PIHAK PERTAMA dan PIHAK KEDUA ").Bold();
                            text.Span("secara bersama-sama selanjutnya disebut PARA PIHAK.");
                        });
                        col.Item().Text("PARA PIHAK dengan ini sepakat mengikatkan diri dalam Perjanjian ini dengan syarat dan ketentuan sebagai berikut:");

                        col.Item().AlignCenter().Text("Pasal 1\nPosisi");
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("1.").AlignCenter();
                            row.RelativeItem(11).Text($"PIHAK PERTAMA bersedia menerima PIHAK KEDUA demikian pula PIHAK KEDUA bersedia bekerja pada PIHAK PERTAMA sebagai karyawan dengan jabatan sebagai {employee.Job?.job_name ?? "-"}.");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text($"2.").AlignCenter();
                            row.RelativeItem(11).Text("Tugas dan tanggungjawab pada jabatan tersebut, sesuai dengan Job Description yang berlaku di Perusahaan sebagaimana dijelaskan oleh PIHAK PERTAMA.");
                        });
                        
                        col.Item().AlignCenter().Text("Pasal 2\nGaji");
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("1.").AlignCenter();
                            row.RelativeItem(11).Text("Gaji/Penghasilan yang diterima oleh PIHAK KEDUA ditetapkan sebagai berikut : ");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("");
                            row.RelativeItem(3).Text("a.    Gaji Pokok");
                            row.RelativeItem(8).Text($":    {currencyFormat(contract.gaji_pokok)} ({spellGaji}).");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("");
                            row.RelativeItem(3).Text("b.    Tunjangan-Tunjangan");
                            row.RelativeItem(8).Text($":");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(2).Text("");
                            row.RelativeItem(2).Text("- Tunjangan ……");
                            row.RelativeItem(8).Text(":     Rp. -");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(2).Text("");
                            row.RelativeItem(2).Text("- Tunjangan MK");
                            row.RelativeItem(8).Text(":     Rp. -");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("2.").AlignCenter();
                            row.RelativeItem(11).Text("Gaji yang diterima oleh PIHAK KEDUA akan dipotong Pajak Penghasilan (PPh. Ps21) dan dibayarkan sesuai peraturan perundang-undangan yang berlaku.");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("3.").AlignCenter();
                            row.RelativeItem(11).Text("THR akan diberikan proporsional jika masa kerja kurang dari 1 tahun: (Masa Kerja / 12) x Upah.");
                        });

                        col.Item().AlignCenter().Text("Pasal 3\nJangka Waktu Perjanjian");
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("1.").AlignCenter();
                            row.RelativeItem(11).Text(text =>
                            {
                                text.Span("Perjanjian ini berlaku untuk jangka waktu ");
                                text.Span($"{Duration} ({spellDuration}) ").Bold();
                                text.Span($"bulan terhitung sejak tanggal ");
                                text.Span($"{formattedStart} ").Bold();
                                text.Span("dan berakhir pada tanggal ");
                                text.Span($"{formattedEnd}.").Bold();
                            });
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("2.").AlignCenter();
                            row.RelativeItem(11).Text("Apabila jangka waktu Perjanjian telah berakhir, maka : ");
                        });                        
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("");
                            row.RelativeItem(1).Text(" a.\n\n\n b.");
                            row.RelativeItem(11).Text("Sejak tanggal berakhirnya Perjanjian ini maka berakhir pula hubungan kerja antara PARA PIHAK tanpa perlu pemberitahuan terlebih dahulu. Dengan berakhirnya perjanjian kerja waktu tertentu ini maka kepada pekerja diberikan kompensasi sesuai peraturan perundang-undangan yang berlaku." +
                                "\nPerjanjian dapat diperpanjang kembali sesuai kesepakatan PARA PIHAK dengan pemberian kompensasi sebesar 1 bulan gaji : 12 bulan X lama periode kontrak atau PIHAK KEDUA dapat diangkat sebagai karyawan tetap Perusahaan.");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("3.").AlignCenter();
                            row.RelativeItem(11).Text("Selama jangka waktu Perjanjian ini berlaku apabila PIHAK KEDUA terbukti melakukan pelanggaran berat sesuai yang diatur dalam Peraturan Perusahaan dan/atau peraturan perundang-undangan yang berlaku, maka PIHAK PERTAMA berhak seketika mengakhiri hubungan kerja tanpa membayar kompensasi apapun.");
                        });
                        col.Item().Text("Jika PIHAK KEDUA melakukan pelanggaran berat, PIHAK PERTAMA berhak mengakhiri hubungan kerja tanpa kompensasi.");
                    

                // PAGE 2 START

                        col.Item().AlignCenter().Text("Pasal 4\nWaktu Kerja");
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("1.").AlignCenter();
                            row.RelativeItem(11).Text("Hari dan jam kerja yang berlaku bagi Pihak Kedua ditetapkan sesuai peraturan  yang berlaku di Perusahaan, dan untuk saat ini hari dan jam kerja tersebut adalah sebagai berikut :");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("");
                            row.RelativeItem(3).Text("Senin s/d Kamis\nJum'at\nSabtu");
                            row.RelativeItem(8).Text(":      08.00 – 15.30 WIB, Istirahat 12.01 – 12.30 WIB\n" +
                                ":      07.30 – 15.30 WIB, Istirahat 12.01 – 13.00 WIB\n" +
                                ":      08.00 – 13.00 WIB, Istirahat 12.01 – 12.30 WIB");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("2.\n\n3.\n\n4.\n\n5.").AlignCenter();
                            row.RelativeItem(11).Text("Hari dan jam kerja sebagaimana dimaksud pada ayat 1 di atas, sewaktu-waktu dapat berubah sesuai kebutuhan Perusahaan dengan tetap memperhatikan Peraturan Perundang-undangan yang berlaku.\n" +
                                "Cuti Tahunan sebanyak 12 (dua belas) hari kerja diberikan setelah PIHAK KEDUA bekerja selama 12 (dua belas) bulan secara terus menerus.\n" +
                                "Tidak masuk kerja karena sakit dan/atau alasan khusus lainnya hanya dapat diberikan apabila memenuhi prosedur dan persyaratan yang ditetapkan dalam Peraturan Perusahaan.\n" +
                                "Ketidakhadiran PIHAK KEDUA kecuali disebabkan oleh : ");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("");
                            row.RelativeItem(1).Text(" a.\n b. \n c.");
                            row.RelativeItem(11).Text("cuti tahunan (setelah hak cuti tahunan timbul);" +
                                "\nsakit (sesuai prosedur);" +
                                "\ncuti khusus (sesuai Peraturan Perusahaan dan/atau peraturan perundang-undangan yang berlaku);");
                        });

                        col.Item().AlignCenter().Text("Pasal 5\nTempat Kerja");
                        col.Item().Text("Tempat kerja yang berlaku bagi PIHAK KEDUA adalah lokasi Perusahaan sebagaimana alamat PIHAK PERTAMA tersebut di atas, dan/atau lokasi lain yang menurut PIHAK  PERTAMA ada hubungannya dengan tanggung jawab kerja PIHAK KEDUA.");

                        col.Item().AlignCenter().Text("Pasal 6\nTata Tertib Kerja");
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("1.\n\n\n2.\n3.").AlignCenter();
                            row.RelativeItem(11).Text("PIHAK  KEDUA sanggup untuk melaksanakan tugas dan kewajiban yang diberikan oleh PIHAK PERTAMA dengan penuh tanggung jawab dan bersedia untuk mematuhi seluruh peraturan dan tata-tertib yang berlaku dan dikeluarkan oleh PIHAK PERTAMA.\n" +
                                "PIHAK KEDUA tidak diperbolehkan melakukan kerja rangkap di tempat lain.\n" +
                                "Sesuai ketentuan hukum yang berlaku, PIHAK PERTAMA berhak untuk setiap saat memutuskan hubungan kerja tanpa syarat dan tanpa kompensasi jika PIHAK KEDUA karena alasan mendesak dan diberikan uang pisah sebagaimana diatur dalam Peraturan Perundang-Undangan dan/atau Peraturan Perusahaan yang berlaku, antara lain atas pelanggaran sebagai berikut :");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("");
                            row.RelativeItem(1).Text(" a.\n b. \n c. \n\n d. \n e. \n f. \n\n g. \n h.");
                            row.RelativeItem(11).Text("Melakukan penipuan, pencurian, korupsi, atau penggelapan barang dan atau uang milik Perusahaan" +
                                "\nMemberikan keterangan palsu atau yang dipalsukan sehingga merugikan Perusahaan.\n" +
                                "Mabuk, minum minuman keras yang memabukkan, memakai atau mengedarkan narkotika, psikotropika, dan zat adiktif lainnya.\n" +
                                "Melakukan perbuatan asusila atau perjudian." +
                                "\nMenyerang, menganiaya, mengancam atau mengintimidasi teman sekerja atau pengusaha di Perusahaan ini. \n" +
                                "Membujuk teman sekerja atau Pengusaha untuk melakukan perbuatan yang bertentangan dengan Peraturan perundangan yang berlaku. \n" +
                                "Dengan ceroboh atau sengaja membiarkan teman sekerja atau Pengusaha dalan keadaan bahaya di tempat kerja.\n" +
                                "Membongkar atau membocorkan rahasia Perusahaan yang seharusnya dirahasiakan.");
                        });


                        col.Item().AlignCenter().Text("Pasal 7\nRahasia Perusahaan");
                        col.Item().Text("PIHAK KEDUA selama hubungan kerja ini berlangsung maupun setelah hubungan kerja ini berakhir wajib menyimpan seluruh Rahasia Perusahaan termasuk larangan memberikan kepada pihak lain segala informasi yang berhubungan dengan kepentingan atau bisnis Perusahaan, produk – produk dan temuan – temuan Perusahaan serta larangan memiliki / melipatgandakan / meng – copy produk dan temuan – temuan tersebut baik untuk kepentingan sendiri maupun pihak lain.");

                        col.Item().AlignCenter().Text("Pasal 8\nJaminan Pihak Kedua");
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("1.").AlignCenter();
                            row.RelativeItem(11).Text("PIHAK KEDUA menjamin bahwa : ");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("");
                            row.RelativeItem(1).Text(" a. \n b. \n\n c. ");
                            row.RelativeItem(11).Text("pada saat Perjanjian ini ditandatangani, tidak sedang mempunyai hubungan kerja dengan pihak lain;" +
                                "\npada saat Perjanjian ni ditandatangani, tidak sedang terlibat dalam perkara pidana dalam bentuk apapun dan pada tingkat penyelesaian apapun juga;" +
                                "\nsegala informasi, dokumen, arsip-arsip yang telah diserahkan kepada Pemberi Kerja sehubungan dengan Perjanjian ini adalah benar dan sah.");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("2.").AlignCenter();
                            row.RelativeItem(11).Text("Apabila PIHAK KEDUA tidak memenuhi jaminannya tersebut, maka PIHAK PERTAMA berhak memutuskan hubungan kerja secara sepihak tanpa syarat apapun.");
                        });


                        col.Item().AlignCenter().Text("Pasal 9\nPenutup");
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).Text("1.\n\n2.\n3.").AlignCenter();
                            row.RelativeItem(11).Text("Disamping ketentuan-ketentuan yang diatur dalam Perjanjian ini PARA PIHAK menyatakan tunduk dan patuh pada peraturan perundang-undangan yang berlaku di bidang ketenagakerjaan.\n" +
                                "Perjanjian ini mulai berlaku dan mengikat PARA PIHAK sejak tanggal ditandatangani.\n" +
                                "Apabila terjadi perselisihan antara kedua pihak maka akan diselesaikan secara musyawarah dan kekeluargaan.");
                        });

                        col.Item().Text("Demikian Perjanjian ini dibuat dan ditandatangani PARA PIHAK.");
                        col.Item().Text($"Karangjati, {printedToday}");

                        col.Item().Row(row =>
                        {
                            row.RelativeItem(7).Text(text =>
                            {
                                text.Span("PIHAK PERTAMA\n").Bold();
                                text.Span("PT. SEIDENSTICKER INDONESIA\n\n\n\n\n").Bold();
                                text.Span("ANDREAS RIKKY RAHMALLY").Bold();
                            });

                            row.RelativeItem(4).Text(text =>
                            {
                                text.Span("PIHAK KEDUA\n\n\n\n\n\n").Bold();
                                text.Span($"{employee.name}").Bold();
                            });
                        });
                    });
                });

            }).GeneratePdf(outputPath);
            Debug.WriteLine("Saving PDF to: " + outputPath);
        }

        //int HitungDurasiBulan(DateTime start, DateTime end)
        //{
        //    return (end.Year - start.Year) * 12 + end.Month - start.Month;
        //}

        string Terbilang(long number)
        {
            string[] angka = { "", "satu", "dua", "tiga", "empat", "lima", "enam", "tujuh", "delapan", "sembilan", "sepuluh", "sebelas" };

            if (number < 12)
                return angka[number];
            else if (number < 20)
                return Terbilang(number - 10) + " belas";
            else if (number < 100)
                return Terbilang(number / 10) + " puluh " + Terbilang(number % 10);
            else if (number < 200)
                return "seratus " + Terbilang(number - 100);
            else if (number < 1000)
                return Terbilang(number / 100) + " ratus " + Terbilang(number % 100);
            else if (number < 2000)
                return "seribu " + Terbilang(number - 1000);
            else if (number < 1_000_000)
                return Terbilang(number / 1000) + " ribu " + Terbilang(number % 1000);
            else if (number < 1_000_000_000)
                return Terbilang(number / 1_000_000) + " juta " + Terbilang(number % 1_000_000);
            else
                return number.ToString(); // For values beyond 999 million
        }
    }
}
