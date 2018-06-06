using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;

namespace SchoolPower.Views {
    public sealed partial class AboutPage : Page {
        public AboutPage() {
            this.InitializeComponent();
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
                            "Debug Info: SchoolPower Android 1.1.6. \n" +
                            "PowerSchool Username and Password(Optional, help us solve it faster): \n";
            await EmailManager.ShowComposeNewEmailAsync(email);
        }
    }
}
