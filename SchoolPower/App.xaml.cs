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
using System;
using Windows.UI.Xaml.Data;
using Windows.Storage;
using SchoolPower.Models;

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
            } catch (System.NullReferenceException) {
                localSettings.Values["IsFirstTimeLogin"] = true;
            }


            if ((bool)localSettings.Values["IsFirstTimeLogin"]) {
                await NavigationService.NavigateAsync(typeof(Views.LoginPage));
            } 
            else {
                Task<string> getHistoryJSON = StudentData.GetJSON("new");
                String studata = await getHistoryJSON;
                StudentData studentData = new StudentData(StudentData.ParseJSON(studata), StudentData.ParseJSON(studata));

                await NavigationService.NavigateAsync(typeof(Views.MainPage));

            }
        }
    }
}
