using Laba2;
using System.Diagnostics;
using System.Xml.Xsl;
using System.Xml.Linq;

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

            lblSelectedFile.Text = $"Файл: {Path.GetFileName(xmlPath)}";
        }

        
        private async void OnPickFileClicked(object sender, EventArgs e)
        {
            try
            {
              
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".xml" } },
                        { DevicePlatform.Android, new[] { "application/xml", "text/xml" } },
                        { DevicePlatform.iOS, new[] { "public.xml" } }, 
                        { DevicePlatform.macOS, new[] { "xml" } },
                    });

                var pickOptions = new PickOptions
                {
                    PickerTitle = "Оберіть XML файл",
                    FileTypes = customFileType,
                };

               
                FileResult result = await FilePicker.Default.PickAsync(pickOptions);

                if (result != null)
                {
                    
                    xmlPath = result.FullPath;

                    
                    lblSelectedFile.Text = $"Файл: {result.FileName}";

                    rtbResult.Text = "Новий XML файл обрано. Готовий до пошуку.";
                    txtSearchValue.Text = "";
                    pickerSearchField.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Не вдалося обрати файл: {ex.Message}", "OK");
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(xmlPath) || !File.Exists(xmlPath))
            {
                await DisplayAlert("Помилка", "Будь ласка, спочатку оберіть коректний XML файл.", "OK");
                return;
            }

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
          
            if (string.IsNullOrEmpty(xmlPath) || !File.Exists(xmlPath))
            {
                await DisplayAlert("Помилка", "Будь ласка, спочатку оберіть коректний XML файл.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(xsltPath) || !File.Exists(xsltPath))
            {
                await DisplayAlert("Помилка", "Не знайдено XSLT файл для трансформації.", "OK");
                return;
            }

           
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

           
            List<Student> results;
            try
            {
                results = strategy.Search(criteria, xmlPath);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка при пошуку", ex.Message, "OK");
                return;
            }

            if (results.Count == 0)
            {
                await DisplayAlert("Нічого не знайдено", "Нічого трансформувати за вашим запитом.", "OK");
                return;
            }

            
            string tempXmlPath = Path.Combine(FileSystem.AppDataDirectory, "temp_filtered.xml");
            try
            {
               
                var filteredDoc = new XDocument(
                    new XElement("Hostel", 
                        from student in results
                        select new XElement("Student",
                            new XElement("FullName", student.FullName),
                            new XElement("Faculty", student.Faculty),
                            new XElement("Course", student.Course),
                            new XElement("RoomNumber", student.RoomNumber),
                            new XElement("MonthlyFee", student.MonthlyFee)
                        )
                    )
                );
                filteredDoc.Save(tempXmlPath);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка створення тимчасового файлу", ex.Message, "OK");
                return;
            }

            
            try
            {
                XslTransformer transformer = new XslTransformer();

               
                transformer.Transform(tempXmlPath, xsltPath, htmlPath);

                await DisplayAlert("Успіх!", $"Відфільтрований HTML-файл '{htmlPath}' успішно створено!", "OK");

                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(htmlPath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка при трансформації", ex.Message, "OK");
            }
            finally
            {
                
                if (File.Exists(tempXmlPath))
                {
                    File.Delete(tempXmlPath);
                }
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
                    await DisplayAlert("Помилка копіювання файлів",
                        "Не вдалося скопіювати файли за замовчуванням. " +
                        "Будь ласка, оберіть XML файл вручну.\n" +
                        $"Деталі: {ex.Message}", "OK");

                    xmlPath = null;
                    lblSelectedFile.Text = "Файл: (Оберіть XML вручну)";
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
