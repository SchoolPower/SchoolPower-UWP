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
    class SettingsPageViewModel: ViewModelBase {
        Services.SettingsServices.SettingsService _settings;

        public SettingsPageViewModel() {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) {
                // designtime
            } else {
                _settings = Services.SettingsServices.SettingsService.Instance;
            }
        }
        
        public void ApplyTheme () {

            if (_ColorBoardSelectedIndex != -1) {

                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["ColorBoardSelectedIndex"] = _ColorBoardSelectedIndex;

                var accentColor = App.themes[_ColorBoardSelectedIndex].AccentColor; // Windows.UI.Colors.DeepPink;

                // accent color
                Application.Current.Resources["SystemAccentColor"] = accentColor;
                Application.Current.Resources["CustomColor"] = accentColor;

                // head
                _HeaderForeground = new SolidColorBrush(accentColor);

                // title bar
                App.SetTitleBarUI(accentColor, Windows.UI.Colors.Black);

                // hamburger
                Views.Shell.HamburgerMenu.AccentColor = accentColor;

                // apply 
                _settings.AppTheme = ApplicationTheme.Light;
                _settings.AppTheme = ApplicationTheme.Dark;
                NavigationService.Navigate(typeof(Views.SettingsPage), null, new DrillInNavigationTransitionInfo());
                NavigationService.GoBack();
            }
        }

        public static SolidColorBrush _HeaderForeground = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["CustomColor"]);
        public SolidColorBrush HeaderForeground {
            get { return _HeaderForeground; }
            set {
                _HeaderForeground = value;
                base.RaisePropertyChanged();
            }
        }

        public static int _ColorBoardSelectedIndex = -1;
        public int ColorBoardSelectedIndex {
            get { return _ColorBoardSelectedIndex; }
            set {
                _ColorBoardSelectedIndex = value;
                base.RaisePropertyChanged();
            }
        }
    }
}
