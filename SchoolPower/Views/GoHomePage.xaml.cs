using SchoolPower.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SchoolPower.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GoHomePage: Page {
        public GoHomePage() {
            this.InitializeComponent();
            pageHeader.Background = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["CustomColor"]);

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if ((bool)localSettings.Values["FirstTimeDisplayHomeDialog"]) { 
                ContentDialog contentDialog = new ContentDialog {
                    Content = "此功能仅支持部分学生。\n不提交默认不留校且不坐校车。\n服务器仅保存上一次提交的信息，若信息没有变更不必反复提交。",
                    CloseButtonText = "不再显示"
                };
                await contentDialog.ShowAsync();
            }
            localSettings.Values["FirstTimeDisplayHomeDialog"] = false;
        }
    

        private string result = "";

        private async void Sumbission(object sender, RoutedEventArgs e) {

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            HttpClient client = new HttpClient();
            var content = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                    { "studentID", (string)localSettings.Values["UsrName"] },
                    { "studentName", StudentData.info.LastName + " " + StudentData.info.FirstName },
                    { "stayInSchool", StayInSchool.IsOn.ToString() },
                    { "schoolBus", Bus.IsOn.ToString() },
                    { "friN", friN.IsChecked.ToString() },
                    { "sat", sat.IsChecked.ToString() },
                    { "sun", sun.IsChecked.ToString() },
                    { "DateOutsideFri", DateOutsideFri.IsChecked.ToString() },
                    { "DateOutsideSat", DateOutsideSat.IsChecked.ToString() },
                    { "DateOutsideSun", DateOutsideSun.IsChecked.ToString() },
                    { "SubmissionTime", System.DateTime.Now.ToString("h:mm:ss tt yyyy-MM-dd")}
                }
            );

            // show
            ShowKissingBar.Begin();
            StatusTextBlock.Text = "Updating ...";
            txtIcn.Visibility = Visibility.Collapsed;
            txtIcn.Glyph = "&#xE73E;";
            ProcesR.Visibility = Visibility.Visible;
            KissingBar.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 99, 177));

            // kiss
            result = "";
            try {
                var v = await client.PostAsync("http://35.187.217.44:4567/student/", content);
                // var v = await client.PostAsync("http://127.0.0.1:8000/student/", content);
                result = await v.Content.ReadAsStringAsync();
            } catch (Exception) { }

            // hide
            HideKissingBar.Begin();
            await Task.Delay(300);
            HideKissingBarRow.Begin();

            switch (result) {
                case "okey dokey":
                    StatusTextBlock.Text = "留校信息更新成功。";
                    txtIcn.Visibility = Visibility.Visible;
                    txtIcn.Glyph = "\uE73E";
                    ProcesR.Visibility = Visibility.Collapsed;
                    KissingBar.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 99, 177));
                    break;
                case "human is dead":
                    StatusTextBlock.Text = "坏死。";
                    txtIcn.Visibility = Visibility.Visible;
                    txtIcn.Glyph = "\uEA6A";
                    ProcesR.Visibility = Visibility.Collapsed;
                    KissingBar.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 202, 81, 0));
                    break;
                default:
                    StatusTextBlock.Text = "生死去来，棚头傀儡。一线断时，落落磊磊。";
                    txtIcn.Visibility = Visibility.Visible;
                    txtIcn.Glyph = "\uEA6A";
                    ProcesR.Visibility = Visibility.Collapsed;
                    KissingBar.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 202, 81, 0));
                    break;
            }
            ShowKissingBar.Begin();
        }

        private async void Help(object sender, RoutedEventArgs e) {
            ContentDialog contentDialog = new ContentDialog {
                Title = "帮助",
                Content = "此功能仅支持部分学生。\n不提交默认不留校且不坐校车。\n服务器仅保存上一次提交的信息，若信息没有变更不必反复提交。",
                CloseButtonText = "好"
            };
            await contentDialog.ShowAsync();
        }
    }
}
