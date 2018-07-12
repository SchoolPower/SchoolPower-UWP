using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SchoolPower.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using System;
using SchoolPower.Views.Dialogs;

namespace SchoolPower.Views {
    public sealed partial class MainPage : Page {
        private List<Subject> subjects;
        private Windows.UI.Xaml.GridLength zeroGridLength;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public MainPage() {
            Views.Shell.HamburgerMenu.IsFullScreen = !true;
            Views.Shell.HamburgerMenu.HamburgerButtonVisibility = !false ? Visibility.Visible : Visibility.Collapsed;
            Initialize();
        }

        void Initialize() {
            if ((bool)localSettings.Values["showInactive"]) {
                subjects = StudentData.subjects;
            } else {
                List<Subject> temp = new List<Subject>();
                foreach (var subject in StudentData.subjects) 
                    if (subject.IsActive) 
                        temp.Add(subject);
                subjects = temp;
            }

            foreach (var subject in subjects) {
                if ((bool)localSettings.Values["DashboardShowGradeOfTERM"]) {
                    foreach (var p in subject.Peroids) {
                        if ((p.IsActive) && ((p.Time == "T1") || (p.Time == "T2") || (p.Time == "T3") || (p.Time == "T4"))) {
                            subject.LetterGradeOnDashboard = p.Letter;
                            subject.PercentageGradeOnDashboard = p.Percent;
                            break;
                        }
                    }
                } else {
                    foreach (var p in subject.Peroids) {
                        if ((p.IsActive) && ((p.Time == "S1") || (p.Time == "S2"))) {
                            subject.LetterGradeOnDashboard = p.Letter;
                            subject.PercentageGradeOnDashboard = p.Percent;
                            break;
                        }
                    }
                }
            }

            InitializeComponent();
            zeroGridLength = new Windows.UI.Xaml.GridLength(); zeroGridLength = EmptyColumn.Width;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) => {
                if (CurrentVisualState.Text == "Narrow" && GradeOverViewColumn.Width == zeroGridLength) 
                    Swap();
            };

            // await Task.Delay(1); // <- this code displays back button, i do not know why

        }


        void Swap() {
            Windows.UI.Xaml.GridLength gridLength = new Windows.UI.Xaml.GridLength();
            gridLength = GradeDetailColumn.Width;
            GradeDetailColumn.Width = GradeOverViewColumn.Width;
            GradeOverViewColumn.Width = gridLength;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            if (App.isMainPageFirstTimeInit) {
                App.isMainPageFirstTimeInit = !App.isMainPageFirstTimeInit;
                await KissingAsync();
            }
        }

        private async void GPA_But_Click(object sender, RoutedEventArgs e) {
            SchoolPower.Views.Dialogs.GPADialog dialog = new SchoolPower.Views.Dialogs.GPADialog();
            await dialog.ShowAsync();
        }

        private async void Refresh_But_Click(object sender, RoutedEventArgs e) {
            await KissingAsync();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // remove img
            NoGradeIcnImg.SetValue(Grid.ColumnProperty, 2);
            // change listview selected itm data template
            // Assign DataTemplate 
            foreach (var item in e.AddedItems) { 
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                lvi.ContentTemplate = (DataTemplate)this.Resources["CoursesListDataTemplate_Detail"];
            }
            // Remove DataTemplate
            foreach (var item in e.RemovedItems) { 
                ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                try {
                    lvi.ContentTemplate = (DataTemplate)this.Resources["CoursesListDataTemplate_Compact"];
                } catch (System.NullReferenceException) { }
            }
            // navigate when normal
            if (CurrentVisualState.Text == "Normal") {
                string selectedSubject = "";
                try {
                    selectedSubject = subjects[ListV.SelectedIndex].Name;
                } catch (System.ArgumentOutOfRangeException) { }
                if (ListV.SelectedIndex != -1) 
                    GradeDetailFrame.Navigate(typeof(MainPageGradePage), selectedSubject);
            }
        }

        private void GoToDetailBut_Loaded(object sender, RoutedEventArgs e) {
            Button _Button = sender as Button;
            switch (CurrentVisualState.Text) {
                case "Narrow":
                    _Button.Visibility = Visibility.Visible;
                    break;
                case "Normal":
                    _Button.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void GoToDetailBut_Click(object sender, RoutedEventArgs e) {
            if (CurrentVisualState.Text == "Narrow") {
                Swap();
                GradeDetailFrame.Navigate(typeof(MainPageGradePage), ListV.SelectedIndex);
            }
        }

        private async void GradeDetailGridView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            GridView gridView = sender as GridView;
            GradeInfoDialog dialog = new GradeInfoDialog(subjects[ListV.SelectedIndex].Peroids[gridView.SelectedIndex]);
            await dialog.ShowAsync();
        }

        private void EditAssignment(object sender, RoutedEventArgs e) {

        }

        private async Task KissingAsync() {
            
            // show
            ShowKissingBar.Begin();

            // refresh
            string result = await StudentData.Refresh();

            // hide
            HideKissingBar.Begin();
            await Task.Delay(300);
            HideKissingBarRow.Begin();

            switch (result) {
                case "okey dokey":
                    StatusTextBlock.Text = "Kissed!";
                    break;
                case "error":
                    StatusTextBlock.Text = "Network error, grades are not updated. Please refresh later. ";
                    // Tawny
                    KissingBar.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 202, 81, 0));
                    break;
            }

            ProcesR.Visibility = Visibility.Collapsed;
            ShowKissingBar.Begin();
            await Task.Delay(2000);
            HideKissingBar.Begin();
            await Task.Delay(300);
            HideKissingBarRow.Begin();

            StatusTextBlock.Text = "Kissing...";
            ProcesR.Visibility = Visibility.Visible;
            KissingBar.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 99, 177));

            Initialize();
        }
    }
}
