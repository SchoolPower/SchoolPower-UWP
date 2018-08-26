using System;
using Windows.UI.Xaml.Controls;
using SchoolPower.Models;
using System.Collections.Generic;
using SchoolPower.Localization;
using System.Linq;
using Windows.UI.Xaml.Media.Animation;
using System.Diagnostics;

namespace SchoolPower.Views {
    public sealed partial class SettingsPage : Page {

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        List<Subject> subjects = StudentData.subjects;

        public SettingsPage() {
            
            InitializeComponent();

            Language_ComboBox.ItemsSource = LocalizedResources.SupportedLanguages;
            Language_ComboBox.SelectedIndex = Array.IndexOf(LocalizedResources.SupportedLanguages.ToArray(), LocalizedResources.Language);
        }

        private void LogoutButtonClick_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
        }

        private async void InnerFlyoutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            await StudentData.MagicLogout(StudentData.MagicNumber);
            Frame.Navigate(typeof(LoginPage));
        }

        private void SelectSubject_CheckBox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            CheckBox checkBox = sender as CheckBox;
            string subjectName = checkBox.Content.ToString();
            if (StudentData.GPASelectedSubject[subjectName]) {
                checkBox.IsChecked = true;
            }
        }

        private void CalcRule_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { 
            var combo = sender as ComboBox;
            localSettings.Values["CalculateRule"] = combo.SelectedIndex;
        }

        private void CalcRule_ComboBox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) { 
            var combo = sender as ComboBox;
            combo.SelectedIndex = (int)localSettings.Values["CalculateRule"];
        }

        private void SubjectList_CheckBox_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
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
            
            if (Language_ComboBox.SelectedItem != null) {

                if (Language_Combo_IsLoaded) 
                    localSettings.Values["lang"] = Language_ComboBox.SelectedIndex;

                var language = Language_ComboBox.SelectedItem as string;
                LocalizedResources.Language = language;

                if (Language_Combo_IsLoaded) {
                    // await CoreApplication.RequestRestartAsync("");
                    Frame.Navigate(typeof(Views.SettingsPage), null, new DrillInNavigationTransitionInfo());
                    Frame.GoBack();
                }
            }
        }

        private bool Language_Combo_IsLoaded = false;

        private void Language_ComboBox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            var index = (int)localSettings.Values["lang"];
            Language_ComboBox.SelectedIndex = index;
            Language_Combo_IsLoaded = true;
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