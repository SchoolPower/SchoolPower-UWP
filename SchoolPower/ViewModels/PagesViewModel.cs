using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace SchoolPower.ViewModels {
    class PagesViewModel: ViewModelBase {
        Services.SettingsServices.SettingsService _settings;

        public PagesViewModel() {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) {
                // designtime
            } else {
                _settings = Services.SettingsServices.SettingsService.Instance;
            }
        }
        
        public void ApplyThemeOnStart () {

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            var accentColor = App.themes[(int)localSettings.Values["ColorBoardSelectedIndex"]].AccentColor;

            // accent color
            Application.Current.Resources["SystemAccentColor"] = accentColor;
            Application.Current.Resources["CustomColor"] = accentColor;

            // title bar
            App.SetTitleBarUI(accentColor, Windows.UI.Colors.Black);

            // hamburger
            Views.Shell.HamburgerMenu.AccentColor = accentColor;

            // apply 
            _settings.AppTheme = ApplicationTheme.Light;
            _settings.AppTheme = ApplicationTheme.Dark;
            // NavigationService.Navigate(typeof(Views.SubjectsAssignmentsPage), null, new DrillInNavigationTransitionInfo());
            // NavigationService.GoBack();
        }
    }
}
