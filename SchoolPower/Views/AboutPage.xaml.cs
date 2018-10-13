using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.UI.Xaml.Media.Imaging;

namespace SchoolPower.Views {
    public sealed partial class AboutPage : Page {

        public ImageSource Logo = new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));//Windows.ApplicationModel.Package.Current.Logo;
        public string DisplayName = Windows.ApplicationModel.Package.Current.DisplayName;
        public string Publisher = Windows.ApplicationModel.Package.Current.PublisherDisplayName;
        public string Version {
            get {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public AboutPage() {
            this.InitializeComponent();
            pageHeader.Background = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["CustomColor"]);
        }

        async Task LaunchWebsiteAsync() {
            string uriToLaunch = @"https://feedback.schoolpower.studio";
            var uri = new Uri(uriToLaunch);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }


        private async void Forum_Button_Click(object sender, RoutedEventArgs e) {
            await LaunchWebsiteAsync();
        }

        private async void Dmail_Button_Click(object sender, RoutedEventArgs e) {
            EmailMessage email = new EmailMessage();
            email.To.Add(new EmailRecipient("harryyunull@gmail.com"));
            email.Subject = "Found Bug(s) in SchoolPower";
            email.Body = "Please provide as much info as possible, such as how to reproduce it, what is the expected result. \n" +
                            "Debug Info: SchoolPower UWP Preview \n" + 
                            "PowerSchool Username and Password(Optional, help us solve it faster): \n";
            await EmailManager.ShowComposeNewEmailAsync(email);
        }
    }
}
