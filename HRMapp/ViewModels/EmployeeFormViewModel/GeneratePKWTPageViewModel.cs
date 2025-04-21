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
        private void CreatePdf()
        {
            var folderPath = @"F:\Coolyeah\G_Smt 6\tesPdf";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var trimEmp = SelectedEmployee.name.Replace(" ", "");

            var contractDate = SelectedContract.contract_date.ToString("dd-MM-yyyy");
            var fileName = $"PKWT_{SelectedContract.contract_nip}_{trimEmp ?? "Unknown"}_{contractDate}.pdf";
            var fullPath = Path.Combine(folderPath, fileName);

            GeneratePdf(fullPath);
            Debug.WriteLine("PDF created at: " + fullPath);
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
            var dayToday = today.DayOfWeek.ToString(indo);
            string currencyFormat(int amount) => $"Rp. {amount:N0}";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11).LineHeight(1.5f));
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

                        col.Item().AlignCenter().Text("PERJANJIAN KERJA WAKTU TERTENTU").Bold();
                        col.Item().AlignCenter().Text($"Nomor : {employee.nip} B / SEI-PKWT / I / 2025");

                        col.Item().Text($"Perjanjian Kerja Waktu Tertentu/PKWT (selanjutnya disebut ‘Perjanjian’) ini dibuat dan ditandatangani pada hari ini {dayToday} tanggal {printedToday} oleh dan antara:");
                        
                        col.Item().Text("1. Nama       : Andreas Rikky R");
                        col.Item().Text("   Jabatan    : HRD");
                        col.Item().Text("   Alamat     : Jl. P.TP. Ngobo XVIII, Dusun Ngimbun, RT/RW 1/3, Kelurahan Karangjati, Kecamatan Bergas, Kabupaten Semarang, Jawa Tengah.");
                        col.Item().Text("Dalam jabatannya tersebut di atas, bertindak untuk dan atas nama PT. SEIDENSTICKER INDONESIA, selanjutnya disebut PIHAK PERTAMA.");

                        col.Item().Text($"2. Nama       : {employee.name}");
                        col.Item().Text($"   Tempat, Tgl Lahir : {employee.birthplace}, {employee.birthdate:dd MMMM yyyy}");
                        col.Item().Text($"   Alamat     : {employee.address}");
                        col.Item().Text($"   Nomor KTP  : {employee.nik}");
                        col.Item().Text("Bertindak untuk dan atas nama diri sendiri, selanjutnya disebut PIHAK KEDUA.");

                        col.Item().Text("PIHAK PERTAMA dan PIHAK KEDUA secara bersama-sama selanjutnya disebut PARA PIHAK.");
                        col.Item().Text("PARA PIHAK dengan ini sepakat mengikatkan diri dalam Perjanjian ini dengan syarat dan ketentuan sebagai berikut:");

                        col.Item().AlignCenter().Text("Pasal 1\nPosisi").Bold();
                        col.Item().Text($"PIHAK PERTAMA bersedia menerima PIHAK KEDUA demikian pula PIHAK KEDUA bersedia bekerja pada PIHAK PERTAMA sebagai karyawan dengan jabatan sebagai {employee.Job?.job_name ?? "-"}. Tugas dan tanggung jawab sesuai dengan Job Description yang berlaku di Perusahaan.");

                        col.Item().AlignCenter().Text("Pasal 2\nGaji").Bold();
                        col.Item().Text($"Gaji Pokok         : {currencyFormat(contract.gaji_pokok)} (Dua juta tujuh ratus lima puluh satu ribu rupiah)");
                        col.Item().Text("Tunjangan ……       : Rp. -\nTunjangan MK       : Rp. -");
                        col.Item().Text("Gaji akan dipotong PPh Ps21 dan dibayarkan sesuai ketentuan berlaku.");
                        col.Item().Text("THR akan diberikan proporsional jika masa kerja kurang dari 1 tahun: (Masa Kerja / 12) x Upah.");

                        col.Item().AlignCenter().Text("Pasal 3\nJangka Waktu Perjanjian").Bold();
                        col.Item().Text($"Perjanjian ini berlaku untuk jangka waktu {contract.contract_duration} (enam) bulan terhitung sejak tanggal {formattedStart} dan berakhir pada tanggal {formattedEnd}.");
                        col.Item().Text("Setelah perjanjian berakhir:");
                        Bullet("Hubungan kerja berakhir tanpa pemberitahuan.");
                        Bullet("Diberikan kompensasi sesuai ketentuan.");
                        Bullet("Bisa diperpanjang dengan kompensasi atau diangkat sebagai karyawan tetap.");
                    });
                });

                // PAGE 2 START
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11).LineHeight(1.5f));
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

                        col.Item().Text("Jika PIHAK KEDUA melakukan pelanggaran berat, PIHAK PERTAMA berhak mengakhiri hubungan kerja tanpa kompensasi.");
                        col.Item().AlignCenter().Text("Pasal 4\nWaktu Kerja").Bold();
                        col.Item().Text("Hari dan jam kerja sesuai peraturan Perusahaan:");
                        col.Item().Text("Senin s/d Kamis : 08.00 – 15.30 WIB, Istirahat 12.01 – 12.30 WIB");
                        col.Item().Text("Jum’at          : 07.30 – 15.30 WIB, Istirahat 12.01 – 13.00 WIB");
                        col.Item().Text("Sabtu           : 08.00 – 13.00 WIB, Istirahat 12.01 – 12.30 WIB");
                        col.Item().Text("Jam kerja dapat berubah sesuai kebutuhan Perusahaan.");
                        col.Item().Text("Cuti tahunan 12 hari kerja setelah 12 bulan kerja.");
                        col.Item().Text("Ketidakhadiran selain sakit/cuti dikenakan potongan gaji sesuai hari tidak masuk.");

                        col.Item().AlignCenter().Text("Pasal 5\nTempat Kerja").Bold();
                        col.Item().Text("Tempat kerja adalah lokasi Perusahaan dan/atau lokasi lain sesuai tanggung jawab PIHAK KEDUA.");

                        col.Item().AlignCenter().Text("Pasal 6\nTata Tertib Kerja").Bold();
                        col.Item().Text("PIHAK KEDUA wajib melaksanakan tugas dengan penuh tanggung jawab dan mematuhi tata tertib yang berlaku.");
                        col.Item().Text("Tidak diperbolehkan kerja rangkap.");
                        col.Item().Text("Perusahaan berhak memutuskan hubungan kerja tanpa syarat jika PIHAK KEDUA melakukan pelanggaran berat seperti:");
                        Bullet("Penipuan, pencurian, penggelapan.");
                        Bullet("Memberikan keterangan palsu.");
                        Bullet("Mabuk, narkotika, perjudian.");
                        Bullet("Perbuatan asusila.");
                        Bullet("Mengintimidasi rekan kerja.");
                        Bullet("Membocorkan rahasia perusahaan.");

                        col.Item().AlignCenter().Text("Pasal 7\nRahasia Perusahaan").Bold();
                        col.Item().Text("PIHAK KEDUA wajib menjaga rahasia perusahaan selama dan setelah hubungan kerja. Dilarang menyebarkan atau menyalin informasi, produk, dan temuan perusahaan.");

                        col.Item().AlignCenter().Text("Pasal 8\nJaminan Pihak Kedua").Bold();
                        Bullet("Tidak sedang bekerja di tempat lain.");
                        Bullet("Tidak terlibat perkara pidana.");
                        Bullet("Dokumen yang diserahkan adalah benar.");
                        col.Item().Text("Jika tidak dipenuhi, hubungan kerja dapat diputuskan secara sepihak.");

                        col.Item().AlignCenter().Text("Pasal 9\nPenutup").Bold();
                        col.Item().Text("PARA PIHAK tunduk pada peraturan perundang-undangan ketenagakerjaan. Perselisihan diselesaikan secara musyawarah.");

                        col.Item().Text($"Karangjati, {printedToday}");

                        col.Item().Row(row =>
                        {
                            row.RelativeColumn().Text(text =>
                            {
                                text.Span("PIHAK PERTAMA\n");
                                text.Span("PT. SEIDENSTICKER INDONESIA\n\n\n\n");
                                text.Span("ANDREAS RIKKY RAHMALLY").Bold();
                            });

                            row.RelativeColumn().Text(text =>
                            {
                                text.Span("PIHAK KEDUA\n\n\n\n\n");
                                text.Span($"{employee.name}").Bold();
                            });
                        });
                    });
                });

            }).GeneratePdf(outputPath);
            Debug.WriteLine("Saving PDF to: " + outputPath);
        }
    }
}
