/*

                   _ooOoo_
                  o8888888o
                  88" . "88
                  (| -_- |)
                  O\  =  /O
               ____/`---'\____
             .'  \\|     |//  `.
            /  \\|||  :  |||//  \
           /  _||||| -:- |||||-  \
           |   | \\\  -  /// |   |
           | \_|  ''\---/''  |   |
           \  .-\__  `-`  ___/-. /
         ___`. .'  /--.--\  `. . __
      ."" '<  `.___\_<|>_/___.'  >'"".
     | | :  `- \`.;`\ _ /`;.`/ - ` : | |
     \  \ `-.   \_ __\ /__ _/   .-` /  /
======`-.____`-.___\_____/___.-`____.-'======
                   `=---='

            佛祖保佑       永无BUG
 */

using Windows.UI.Xaml;
using System.Threading.Tasks;
using SchoolPower.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using Windows.UI.Xaml.Data;
using Windows.Storage;
using SchoolPower.Models;
using Windows.UI.ViewManagement;

namespace SchoolPower {
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : BootStrapper {

        public static bool isMainPageFirstTimeInit = true;

        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public App() {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);
            App.Current.UnhandledException += OnUnhandledException;

            #region app settings

            // some settings must be set in app.constructor
            var settings = SettingsService.Instance;
            RequestedTheme = settings.AppTheme;
            CacheMaxDuration = settings.CacheMaxDuration;
            ShowShellBackButton = settings.UseShellBackButton;

            #endregion
        }

        public override UIElement CreateRootElement(IActivatedEventArgs e) {
            var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
            return new ModalDialog {
                DisableBackButtonWhenModal = true,
                Content = new Views.Shell(service),
                ModalContent = new Views.Busy(),
            };
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args) {

            bool IsLogin;

            try {
                IsLogin = (bool)localSettings.Values["IsFirstTimeLogin"];
                bool b = (bool)localSettings.Values["showInactive"];
                b = (bool)localSettings.Values["DashboardShowGradeOfTERM"];
                var v = (string)localSettings.Values["dates"];
                var vv = localSettings.Values["lang"].ToString();
                var vvv = localSettings.Values["CalculateRule"];
            }
            catch (System.NullReferenceException) {
                localSettings.Values["IsFirstTimeLogin"] = true;
                localSettings.Values["showInactive"] = false;
                localSettings.Values["DashboardShowGradeOfTERM"] = true;
                localSettings.Values["dates"] = "";
                localSettings.Values["lang"] = 0;
                localSettings.Values["CalculateRule"] = 0;
            }

            if ((bool)localSettings.Values["IsFirstTimeLogin"]) {
                await NavigationService.NavigateAsync(typeof(Views.LoginPage));
            } else {
                Task<string> getHistoryJSON = StudentData.GetStudentData(StudentData.NewOrOld.New);
                string studataOld = await getHistoryJSON;
                StudentData studentData = new StudentData(StudentData.ParseJSON(studataOld), StudentData.ParseJSON(studataOld));
                App.SetUIBlue();
                await NavigationService.NavigateAsync(typeof(Views.SubjectsAssignmentsPage));
            }

            // SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

        }

        public static void SetUIBlue() {
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

        public static void SetUIBlack() {
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

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs) {
            string username = (string)localSettings.Values["UsrName"];
            string password = (string)localSettings.Values["Passwd"];
            Windows.Storage.ApplicationData.Current.ClearAsync(); // do not await 
            localSettings.Values["UsrName"] = username;
            localSettings.Values["Passwd"] = password;
        }
    }
}
