using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net.Http;
using SchoolPower.Models;
using Windows.UI.ViewManagement;
using Windows.UI.Core;
using SchoolPower.Localization;

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
            App.SetUIBlack();

            try { UsernameTextBox.Text = localSettings.Values["UsrName"].ToString(); } catch (System.NullReferenceException) { } 
            try { PasswordTextBox.Password = localSettings.Values["Passwd"].ToString(); } catch (System.NullReferenceException) { }
            
        }
        
        private void SignInButton_Click(object sender, RoutedEventArgs e) {
            Login();
        }

        async void Login() {

            string studata = "";
            string studataOld = "";

            App.isMainPageFirstTimeInit = false;

            // get account info
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;
            localSettings.Values["UsrName"] = username;
            localSettings.Values["Passwd"] = password;

            // when empty box
            if (username == "" || password == "") {
                ContentDialog ErrorEmptyContentDialog = new ContentDialog {
                    Title = LocalizedResources.GetString("/Text"),
                    Content = LocalizedResources.GetString("pleaseInput/Text"),
                    CloseButtonText = LocalizedResources.GetString("yesInput/Text"),
                }; ContentDialogResult result = await ErrorEmptyContentDialog.ShowAsync();
            }
            
            // load
            else {
                App.SetUIBlue();

                // kissing
                Views.Busy.SetBusy(true, "Kissing");

                try { studata = await StudentData.Kissing(username, password); } catch (Exception) { }

                // bad network or server
                if (studata == "") {
                    ContentDialog ErrorContentDialog = new ContentDialog {
                        Title = LocalizedResources.GetString("error/Text"),
                        Content = LocalizedResources.GetString("netError/Text"),
                        CloseButtonText = LocalizedResources.GetString("yesNet/Text"),
                    }; ContentDialogResult result = await ErrorContentDialog.ShowAsync();
                } 

                // wrong account info
                else if (studata.Contains("Something went wrong!")) {
                    PasswordTextBox.PlaceholderText = "";
                    ContentDialog ErrorContentDialog = new ContentDialog {
                        Title = LocalizedResources.GetString("error/Text"),
                        Content = LocalizedResources.GetString("someError/Text"),
                        CloseButtonText = LocalizedResources.GetString("yesAcc/Text"),
                    }; ContentDialogResult result = await ErrorContentDialog.ShowAsync();
                }

                else {

                    // save student data to new
                    studataOld = studata;
                    await StudentData.SaveStudentData(studata, StudentData.NewOrOld.New);
                    localSettings.Values["IsFirstTimeLogin"] = false;

                    try {
                        // new StudentData
                        StudentData studentData = new StudentData(StudentData.ParseJSON(studata), StudentData.ParseJSON(studataOld));
                        StudentData.SaveHistoryData(StudentData.CollectCurrentHistoryData());
                        // navigate
                        Frame.Navigate(typeof(SubjectsAssignmentsPage));
                    } catch (Exception e) {
                        await Windows.Storage.ApplicationData.Current.ClearAsync();
                        localSettings.Values["UsrName"] = username;
                        localSettings.Values["Passwd"] = password;
                        ContentDialog ErrorContentDialog = new ContentDialog {
                            Title = "ERROR",
                            Content = e.ToString(),
                            CloseButtonText = "哦。",
                        }; ContentDialogResult result = await ErrorContentDialog.ShowAsync();
                    }
                }
                Views.Busy.SetBusy(false);
            }
        }
    }
}
