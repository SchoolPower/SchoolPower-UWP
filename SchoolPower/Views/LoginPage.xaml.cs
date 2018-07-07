using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net.Http;
using SchoolPower.Models;
using Windows.UI.ViewManagement;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SchoolPower.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page {

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private readonly HttpClient client = new HttpClient();
        
        public LoginPage() { 
            InitializeComponent();

            Views.Shell.HamburgerMenu.IsFullScreen = true; // set haumburger invisible
            Views.Shell.HamburgerMenu.HamburgerButtonVisibility = false ? Visibility.Visible : Visibility.Collapsed;  // set haumburger invisible
            SetUIBlack();

            try { UsernameTextBox.Text = localSettings.Values["UsrName"].ToString(); } catch (System.NullReferenceException) { } 
            try { PasswordTextBox.Password = localSettings.Values["Passwd"].ToString(); } catch (System.NullReferenceException) { }

            try { if (UsernameTextBox.Text != "" && PasswordTextBox.Password != "") {
                    Login();
                }
            } catch (System.NullReferenceException) { }

        }
        
        private void SignInButton_Click(object sender, RoutedEventArgs e) {
            Login();
        }

        async void Login() {

            string studata = "";
            string studataOld = "";

            App.isMainPageFirstTimeInit = false;

            // get account info
            String username = UsernameTextBox.Text;
            String password = PasswordTextBox.Password;
            localSettings.Values["UsrName"] = username;
            localSettings.Values["Passwd"] = password;

            // when empty box
            if (username == "" || password == "") {
                ContentDialog ErrorEmptyContentDialog = new ContentDialog {
                    Title = "ERROR",
                    Content = "Please input user name and/or password.",
                    CloseButtonText = "哦。",
                }; ContentDialogResult result = await ErrorEmptyContentDialog.ShowAsync();
            }
            
            // load
            else {
                SetUIBlue();

                // kissing
                Views.Busy.SetBusy(true, "Kissing");

                try { studata = await StudentData.Kissing(username, password); } catch (Exception) { }

                // bad network or server
                if (studata == "") {
                    ContentDialog ErrorContentDialog = new ContentDialog {
                        Title = "ERROR",
                        Content = "Network error. Please try again later.",
                        CloseButtonText = "哦。",
                    }; ContentDialogResult result = await ErrorContentDialog.ShowAsync();
                } 

                // wrong account info
                else if (studata.Contains("Something went wrong! Invalid Username or password")) {
                    PasswordTextBox.PlaceholderText = "";
                    ContentDialog ErrorContentDialog = new ContentDialog {
                        Title = "ERROR",
                        Content = "Wrong username and/or password.",
                        CloseButtonText = "哦。",
                    }; ContentDialogResult result = await ErrorContentDialog.ShowAsync();
                }

                else {

                    // save student data to new
                    studataOld = studata;
                    await StudentData.SaveJSON(studata, StudentData.NewOrOld.New);
                    localSettings.Values["IsFirstTimeLogin"] = false;

                    // new StudentData
                    StudentData studentData = new StudentData(StudentData.ParseJSON(studata), StudentData.ParseJSON(studataOld));
                    
                    // navigate
                    Frame.Navigate(typeof(MainPage));
                }
                Views.Busy.SetBusy(false);
            }
        }

        void SetUIBlue() {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 0, 99, 177);
            titleBar.ForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 0, 99, 177);
            titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(0, 25, 114, 184);
            titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 0, 99, 177);
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.LightGray;
            titleBar.InactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 0, 99, 177);
            titleBar.InactiveForegroundColor = Windows.UI.Colors.LightGray;
            titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(0, 25, 114, 184);
            titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.White;
        }

        void SetUIBlack() {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Windows.UI.Colors.Black;
            titleBar.ForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonBackgroundColor = Windows.UI.Colors.Black;
            titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonPressedBackgroundColor = Windows.UI.Colors.Black;
            titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Black;
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.LightGray;
            titleBar.InactiveBackgroundColor = Windows.UI.Colors.Black;
            titleBar.InactiveForegroundColor = Windows.UI.Colors.LightGray;
            titleBar.ButtonHoverBackgroundColor = Windows.UI.Colors.Black;
            titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.White;
        }

    }
}
