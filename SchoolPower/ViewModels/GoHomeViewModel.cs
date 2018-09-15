using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SchoolPower.ViewModels {
    public class GoHomeViewModel : ViewModelBase {

        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private bool _IsStayAtSchool = true;
        private bool _IsBus = false;
        private bool _IsDate = false;

        public GoHomeViewModel() {
            _IsStayAtSchool = (bool)localSettings.Values["IsStayAtSchool"];
            _IsBus = (bool)localSettings.Values["IsBus"];
            _IsDate = (bool)localSettings.Values["IsDate"];
        }

        public bool IsStayAtSchool {
            get { return _IsStayAtSchool; }
            set {
                _IsStayAtSchool = value;
                localSettings.Values["IsStayAtSchool"] = value;
                if (value) {
                    IsBus = false;
                } else if (!value) {
                    IsDate = false;
                }
                base.RaisePropertyChanged(); }
        }

        public bool IsBus {
            get { return _IsBus; }
            set {
                _IsBus = value;
                localSettings.Values["IsBus"] = value;
                base.RaisePropertyChanged();
            }
        }

        public bool IsDate {
            get { return _IsDate; }
            set {
                _IsDate = value;
                localSettings.Values["IsDate"] = value;
                base.RaisePropertyChanged();
            }
        }
    }

    public class InverseBooleanConverter: IValueConverter {

        public object Convert(object value, Type targetType, object parameter, string language) {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return !(bool)value;
        }
    }

    public class DisableTxtColorConverter: IValueConverter {

        public object Convert(object value, Type targetType, object parameter, string language) {
            if ((bool)value)
                return new SolidColorBrush(Colors.White);
            else
                return new SolidColorBrush(Color.FromArgb(255, 102, 102, 102));//Colors.DarkGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return !(bool)value;
        }
    }
}
