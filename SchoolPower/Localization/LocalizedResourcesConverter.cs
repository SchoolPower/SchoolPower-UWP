using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace SchoolPower.Localization {
    public class LocalizedResourcesConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            if (value == null) return string.Empty;

            var languageCode = (string)value;

            return new CultureInfo(languageCode).DisplayName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return value;
        }
    }
}
