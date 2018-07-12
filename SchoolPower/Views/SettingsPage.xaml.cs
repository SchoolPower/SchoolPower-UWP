using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;
using SchoolPower.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace SchoolPower.Views {
    public sealed partial class SettingsPage : Page {

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        List<Subject> subjects = StudentData.subjects;

        public SettingsPage() {
            InitializeComponent();
        }

        private void LogoutButtonClick_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
        }

        private void InnerFlyoutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            localSettings.Values.Remove("UsrName");
            localSettings.Values.Remove("Passwd");
            localSettings.Values["IsFirstTimeLogin"] = true;

            // clear history
            for (int i = StudentData.subjects.Count; i >= 1; i--) { StudentData.subjects.RemoveAt(i - 1); }
            for (int j = StudentData.attendances.Count; j >= 1; j--) { StudentData.attendances.RemoveAt(j - 1); }
            Frame.Navigate(typeof(LoginPage));
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
            localSettings.Values["CalculateRule"] = combo.SelectedIndex;
        }

        private void ComboBox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            var combo = sender as ComboBox;
            try {
                int i = (int)localSettings.Values["CalculateRule"];
            } catch (System.NullReferenceException) {
                localSettings.Values["CalculateRule"] = 0;
            }
            combo.SelectedIndex = (int)localSettings.Values["CalculateRule"];
        }

        private void CheckBox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            var checkBox = sender as CheckBox;
            checkBox.IsChecked = !checkBox.IsChecked;
            if ((bool)checkBox.IsChecked) {
                localSettings.Values[checkBox.Content.ToString()] = true;
            } else {
                localSettings.Values[checkBox.Content.ToString()] = false;
            }
            System.Diagnostics.Debug.WriteLine(checkBox.Content.ToString() + " " + localSettings.Values[checkBox.Content.ToString()]);
        }

        private void DashboardDisplayRadioButtonCheck(object sender, Windows.UI.Xaml.RoutedEventArgs e) {

        }

        private void Language_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var combo = sender as ComboBox;
            localSettings.Values["lang"] = combo.SelectedIndex;
        }

        private void Language_ComboBox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            var combo = sender as ComboBox;
            try {
                int i = (int)localSettings.Values["lang"];
            } catch (System.NullReferenceException) {
                localSettings.Values["lang"] = 0;
            }
            combo.SelectedIndex = (int)localSettings.Values["lang"];
        }

        private void InactiveSubjects_ToggleSwitch(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            var tSwitch = sender as ToggleSwitch;
            localSettings.Values["showInactive"] = tSwitch.IsOn;
        }

        private void InactiveSubjects_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            var tSwitch = sender as ToggleSwitch;
            tSwitch.IsOn = (bool)localSettings.Values["showInactive"];
        }

        private void DisplayScoreOfRadioButtonCheck(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            var rButton = sender as RadioButton;
            string tag = rButton.Tag.ToString();
            switch (tag) {
                case "term":
                    localSettings.Values["DashboardShowGradeOfTERM"] = true;
                    break;
                case "sem":
                    localSettings.Values["DashboardShowGradeOfTERM"] = !true;
                    break;
            }
        }

        private void TermRadioButtonLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            if ((bool)localSettings.Values["DashboardShowGradeOfTERM"]) {
                var rButton = sender as RadioButton;
                rButton.IsChecked = true;
            }
        }

        private void SemRadioButtonLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            if (!(bool)localSettings.Values["DashboardShowGradeOfTERM"]) {
                var rButton = sender as RadioButton;
                rButton.IsChecked = true;
            }
        }
    }
}