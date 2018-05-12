using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net.Http;
using SchoolPower.Models;
using Template10.Controls;
using Template10.Common;
using Template10.Services.NavigationService;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SchoolPower.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page {

        Windows.Storage.ApplicationDataContainer account = Windows.Storage.ApplicationData.Current.LocalSettings;
        private readonly HttpClient client = new HttpClient();
        
        public LoginPage() { 
            this.InitializeComponent();
            Views.Shell.HamburgerMenu.IsFullScreen = true; // set haumburger invisible
            Views.Shell.HamburgerMenu.HamburgerButtonVisibility = false ? Visibility.Visible : Visibility.Collapsed;  // set haumburger invisible

            try { UsernameTextBox.Text = account.Values["UsrName"].ToString(); } catch (System.NullReferenceException) { } 
            try { PasswordTextBox.Password = account.Values["Passwd"].ToString(); } catch (System.NullReferenceException) { }

            try { if (!UsernameTextBox.Text.Equals("") && !PasswordTextBox.Password.Equals("")) {
                    Login();
                }
            } catch (System.NullReferenceException) { }

        }
        
        private void SignInButton_Click(object sender, RoutedEventArgs e) {
            Login();
        }

        async void Login() {

            String username = UsernameTextBox.Text;
            String password = PasswordTextBox.Password;
            account.Values["UsrName"] = username;
            account.Values["Passwd"] = password;

            // when empty box
            if (username.Equals("") || password.Equals("")) {
                ContentDialog ErrorEmptyContentDialog = new ContentDialog {
                    Title = "ERROR",
                    Content = "Please input user name and/or password.",
                    CloseButtonText = "哦。",
                }; ContentDialogResult result = await ErrorEmptyContentDialog.ShowAsync();
            }
            
            // load
            else { 

                Views.Busy.SetBusy(true, "Loading");
                Task<string> task = StudentData.Kissing(username, password);
                string studata = "";
                try { studata = await task; } catch (Exception) { }

                // bad network or server
                if (studata.Equals("")) {
                    ContentDialog ErrorContentDialog = new ContentDialog {
                        Title = "ERROR",
                        Content = "Network error, grades will not be updates. Please refresh later. ",
                        CloseButtonText = "哦。",
                    }; ContentDialogResult result = await ErrorContentDialog.ShowAsync();
                } else {
                    StudentData.SaveStudentDataToLocal(studata);
                    await Task.Delay(1000);
                }

                // wrong account info
                if (studata.Equals("Something went wrong! Invalid Username or password")) {
                    PasswordTextBox.PlaceholderText = "";
                    ContentDialog ErrorContentDialog = new ContentDialog {
                        Title = "ERROR",
                        Content = "Wrong username and/or password.",
                        CloseButtonText = "哦。",
                    }; ContentDialogResult result = await ErrorContentDialog.ShowAsync();
                } 
                
                // navigate
                else {
                    Views.Busy.SetBusy(false);
                    Frame.Navigate(typeof(MainPage));
                }
            }
        }
    }
}
