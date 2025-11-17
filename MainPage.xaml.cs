using Laba2; 
using System.Diagnostics;
using System.Security.Cryptography;
using System.Xml.Xsl;

namespace Laba2 
{
    public partial class MainPage : ContentPage
    {
        private string xmlPath;
        private string xsltPath;
        private string htmlPath;
        private ISearchStrategy strategy;

        public MainPage()
        {
            InitializeComponent(); 
            pickerSearchField.Items.Add("FullName");
            pickerSearchField.Items.Add("Faculty");
            pickerSearchField.Items.Add("Course");
            pickerSearchField.Items.Add("RoomNumber");
            pickerSearchField.Items.Add("MonthlyFee");

            
            xmlPath = Path.Combine(FileSystem.AppDataDirectory, "Hostel.xml");
            xsltPath = Path.Combine(FileSystem.AppDataDirectory, "Hostel.xslt");
            htmlPath = Path.Combine(FileSystem.AppDataDirectory, "Hostel.html");

            Task.Run(CopyFilesToAppData);
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (rbDOM.IsChecked) strategy = new DomStrategy();
            else if (rbSAX.IsChecked) strategy = new SaxStrategy();
            else if (rbLinq.IsChecked) strategy = new LinqStrategy();

            if (strategy == null)
            {
                await DisplayAlert("Помилка", "Будь ласка, оберіть стратегію аналізу!", "OK");
                return;
            }

            var criteria = new SearchCriteria
            {
                Field = pickerSearchField.SelectedItem?.ToString(),
                Value = txtSearchValue.Text
            };

            try
            {
                List<Student> results = strategy.Search(criteria, xmlPath);
                rtbResult.Text = "";
                if (results.Count == 0)
                {
                    rtbResult.Text = "Нічого не знайдено.";
                }
                else
                {
                    foreach (var student in results)
                    {
                        rtbResult.Text += student.ToString() + "\n----------\n";
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка при пошуку", ex.Message, "OK");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            rtbResult.Text = "";
            txtSearchValue.Text = "";
            pickerSearchField.SelectedIndex = -1;
            pickerSearchField.Title = "Оберіть поле...";
        }

        private async void btnTransform_Click(object sender, EventArgs e)
        {
            try
            {
                XslTransformer transformer = new XslTransformer();
                transformer.Transform(xmlPath, xsltPath, htmlPath);

                await DisplayAlert("Успіх!", $"Файл '{htmlPath}' успішно створено!", "OK");

                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(htmlPath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка при трансформації", ex.Message, "OK");
            }
        }

        private async Task CopyFilesToAppData()
        {
            try
            {
                if (!File.Exists(xmlPath))
                {
                    using var stream = await FileSystem.OpenAppPackageFileAsync("Hostel.xml");
                    using var fileStream = File.Create(xmlPath);
                    await stream.CopyToAsync(fileStream);
                }

                if (!File.Exists(xsltPath))
                {
                    using var stream = await FileSystem.OpenAppPackageFileAsync("Hostel.xslt");
                    using var fileStream = File.Create(xsltPath);
                    await stream.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Помилка копіювання файлів", ex.Message, "OK");
                });
            }
        }

        private async void OnExitButtonClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert(
                "Підтвердження виходу", 
                "Ви дійсно хочете вийти з програми?", 
                "Так, вийти",
                "Ні, залишитись"); 
           
            if (answer)
            {
                Application.Current.Quit();
            }
         
        }
    }
}