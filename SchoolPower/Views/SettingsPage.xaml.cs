using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;
using SchoolPower.Models;
using System.Collections.Generic;

namespace SchoolPower.Views {
    public sealed partial class SettingsPage : Page {

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Template10.Services.SerializationService.ISerializationService _SerializationService;
        List<Subject> subjects = StudentData.subjects;

        public SettingsPage() {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            var index = int.Parse(_SerializationService.Deserialize(e.Parameter?.ToString()).ToString());
            MyPivot.SelectedIndex = index;
        }

        private void LogoutButtonClick_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
        }

        private async void InnerFlyoutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            localSettings.Values.Remove("UsrName");
            localSettings.Values.Remove("Passwd");
            await CoreApplication.RequestRestartAsync(string.Empty);
        }

        private void CheckBox_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            CheckBox checkBox = sender as CheckBox;
            localSettings.Values[checkBox.Content.ToString()] = !(bool)localSettings.Values[checkBox.Content.ToString()];
        }

        private void CheckBox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            CheckBox checkBox = sender as CheckBox;
            try {
                bool b = (bool)localSettings.Values[checkBox.Content.ToString()];
            } catch (System.NullReferenceException) {
                localSettings.Values[checkBox.Content.ToString()] = false;
            }
            if ((bool)localSettings.Values[checkBox.Content.ToString()]) {
                checkBox.IsChecked = true;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var combo = sender as ComboBox;
            localSettings.Values["CalculateRule"] = combo.SelectedIndex.ToString();
        }

        private void ComboBox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            var combo = sender as ComboBox;
            try {
                int i = Convert.ToInt32(localSettings.Values["CalculateRule"].ToString());
            } catch (System.NullReferenceException) {
                localSettings.Values["CalculateRule"] = "0";
            }
            combo.SelectedIndex = Convert.ToInt32(localSettings.Values["CalculateRule"].ToString());
        }
    }
}