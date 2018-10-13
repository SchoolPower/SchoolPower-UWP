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
using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Reflection;
using System.Linq;
using System;

namespace SchoolPower {
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : BootStrapper {

        public static bool isMainPageFirstTimeInit = true;
        public static List<Theme> themes = new List<Theme>(); 
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

            // init value
            bool IsLogin;
            try {
                int i = (int)localSettings.Values["ColorBoardSelectedIndex"];
                bool b = (bool)localSettings.Values["IsStayAtSchool"];
                bool bb = (bool)localSettings.Values["IsBus"];
                bool bbb = (bool)localSettings.Values["IsDate"];
                bool bbbb = (bool)localSettings.Values["showInactive"];
                bool bbbbb = (bool)localSettings.Values["DashboardShowGradeOfTERM"];
                bool bbbbbb = (bool)localSettings.Values["FirstTimeDisplayHomeDialog"];
                var v = (string)localSettings.Values["dates"];
                var vv = localSettings.Values["lang"].ToString();
                var vvv = localSettings.Values["CalculateRule"];
                IsLogin = (bool)localSettings.Values["IsFirstTimeLogin"];
            } catch (System.NullReferenceException) {
                localSettings.Values["ColorBoardSelectedIndex"] = 0;
                localSettings.Values["FirstTimeDisplayHomeDialog"] = true;
                localSettings.Values["IsStayAtSchool"] = true;
                localSettings.Values["IsBus"] = false;
                localSettings.Values["IsDate"] = false;
                localSettings.Values["IsFirstTimeLogin"] = true;
                localSettings.Values["showInactive"] = false;
                localSettings.Values["DashboardShowGradeOfTERM"] = true;
                localSettings.Values["dates"] = "";
                localSettings.Values["lang"] = 0;
                localSettings.Values["CalculateRule"] = 0;
            }

            // init themes

            /*
            // init all colors
            themes.Add(new Theme("Default", Windows.UI.Color.FromArgb(255, 0, 99, 177)));
            var colors = GetStaticPropertyBag(typeof(Colors));
            foreach (KeyValuePair<string, object> colorPair in colors) {
                themes.Add(new Theme(colorPair.Key, (Color)colorPair.Value));
            }
            */

            
            themes.Add(new Theme("Liberty", Windows.UI.Color.FromArgb(255, 0, 99, 177)));
            themes.Add(new Theme("Despair", Windows.UI.Colors.SteelBlue));
            themes.Add(new Theme("Miku", Windows.UI.Colors.DarkCyan));
            themes.Add(new Theme("Monika", Windows.UI.Colors.CadetBlue));

            themes.Add(new Theme("Soviet", Windows.UI.Colors.Red));
            themes.Add(new Theme("Krunch", Windows.UI.Color.FromArgb(255, 255, 117, 117)));
            // themes.Add(new Theme("Doki", Windows.UI.Color.FromArgb(255, 255, 170, 213)));
            // themes.Add(new Theme("Doki", Windows.UI.Color.FromArgb(255, 217, 179, 179)));

        
            
            if ((bool)localSettings.Values["IsFirstTimeLogin"]) {
                await NavigationService.NavigateAsync(typeof(Views.LoginPage));
            } else {
                Task<string> getHistoryJSON = StudentData.GetStudentData(StudentData.NewOrOld.New);
                string studataOld = await getHistoryJSON;
                StudentData studentData = new StudentData(StudentData.ParseJSON(studataOld), StudentData.ParseJSON(studataOld));
                SetTitleBarUI(Windows.UI.Color.FromArgb(255, 0, 99, 177), Windows.UI.Color.FromArgb(0, 25, 114, 184));
                await NavigationService.NavigateAsync(typeof(Views.SubjectsAssignmentsPage));
            }

            // SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        public static Dictionary<string, object> GetStaticPropertyBag(Type t) {
            const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            var map = new Dictionary<string, object>();
            foreach (var prop in t.GetProperties(flags)) {
                map[prop.Name] = prop.GetValue(null, null);
            }
            return map;
        }

        public static void SetTitleBarUI(Windows.UI.Color color, Windows.UI.Color colorHover) {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = color;
            titleBar.ButtonBackgroundColor = color;
            titleBar.ButtonPressedBackgroundColor = colorHover;
            titleBar.ButtonInactiveBackgroundColor = color;
            titleBar.InactiveBackgroundColor = color;
            titleBar.ButtonHoverBackgroundColor = colorHover;
            titleBar.ForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.LightGray;
            titleBar.InactiveForegroundColor = Windows.UI.Colors.LightGray;
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

    public class Theme {
        public string Name { get; set; }
        public Windows.UI.Color AccentColor { get; set; }
        public SolidColorBrush AccentColorBrush { get; set; }

        public Theme(string Name, Windows.UI.Color AccentColor) {
            this.Name = Name;
            this.AccentColor = AccentColor;
            this.AccentColorBrush = new SolidColorBrush(AccentColor);
        }
    }
}
