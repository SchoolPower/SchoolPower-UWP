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

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private readonly HttpClient client = new HttpClient();
        
        public LoginPage() { 
            this.InitializeComponent();
            Views.Shell.HamburgerMenu.IsFullScreen = true; // set haumburger invisible
            Views.Shell.HamburgerMenu.HamburgerButtonVisibility = false ? Visibility.Visible : Visibility.Collapsed;  // set haumburger invisible

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

                // kissing
                Views.Busy.SetBusy(true, "Loading");
                Task<string> task = StudentData.Kissing(username, password);
                string studata = "";
                string studataOld = "";
                try { studata = await task; } catch (Exception) { }

                // bad network or server
                if (studata == "") {
                    ContentDialog ErrorContentDialog = new ContentDialog {
                        Title = "ERROR",
                        Content = "Network error, grades will not be updates. Please refresh later. ",
                        CloseButtonText = "哦。",
                    }; ContentDialogResult result = await ErrorContentDialog.ShowAsync();
                } 

                // wrong account info
                else if (studata == "Something went wrong! Invalid Username or password") {
                    PasswordTextBox.PlaceholderText = "";
                    ContentDialog ErrorContentDialog = new ContentDialog {
                        Title = "ERROR",
                        Content = "Wrong username and/or password.",
                        CloseButtonText = "哦。",
                    }; ContentDialogResult result = await ErrorContentDialog.ShowAsync();
                }

                // save data
                else {

                    // ? await Task.Delay(1000);
                    
                    // when no IsFirstTimeLogin
                    try {
                        if ((bool)localSettings.Values["IsFirstTimeLogin"]) { }
                    } catch (System.NullReferenceException) {
                        localSettings.Values["IsFirstTimeLogin"] = true;
                    }

                    // 
                    if ((bool)localSettings.Values["IsFirstTimeLogin"]) {
                        // cp new to old
                        studataOld = studata;
                        StudentData.SaveJSONtoLocal(studata, "old");
                        localSettings.Values["IsFirstTimeLogin"] = false;
                    } 
                    else {
                        // mv previous studata to old
                        Task<string> getHistoryJSON = StudentData.GetJSONFromLocal("new");
                        studataOld = await getHistoryJSON;
                        StudentData.SaveJSONtoLocal(studataOld, "old");

                        // save current studata to new
                        StudentData.SaveJSONtoLocal(studata, "new");
                    }
                    
                    // new StudentData
                    StudentData studentData = new StudentData(StudentData.ParseJSON(studata), StudentData.ParseJSON(studataOld));
                    
                    // navigate
                    Frame.Navigate(typeof(MainPage));
                }
                Views.Busy.SetBusy(false);
            }
        }
    }
}
