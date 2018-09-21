using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SchoolPower.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using SchoolPower.Views.Dialogs;
using Windows.UI.Xaml.Navigation;
using Windows.System;
using SchoolPower.Localization;
using System.Diagnostics;

namespace SchoolPower.Views {
    public sealed partial class SubjectsAssignmentsPage : Page {
        private List<Subject> subjects;
        private bool IsKissing = false;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public SubjectsAssignmentsPage() {
            Views.Shell.HamburgerMenu.IsFullScreen = !true;
            Views.Shell.HamburgerMenu.HamburgerButtonVisibility = !false ? Visibility.Visible : Visibility.Collapsed;
            Initialize();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            //Application.Current.Resources["SystemAccentColor"] = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 99, 177));
        }

        void Initialize() {

            localSettings.Values["IsFirstTimeLogin"] = false;

            StudentData.AssignmentFilterParam["time"] = null;
            StudentData.AssignmentFilterParam["cata"] = null;

            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                async (window, e) => {
                    switch (e.VirtualKey) {
                        case VirtualKey.F5:
                            if (!IsKissing)
                                try {
                                    await KissingAsync();
                                } catch (Exception) { }
                            break;
                        case VirtualKey.F6:
                            try {
                                await new SchoolPower.Views.Dialogs.GPADialog().ShowAsync();
                            } catch (Exception) { }
                            break;
                    }
                };

            InitializeComponent();

            // Application.Current.Resources["SystemAccentColor"] = new SolidColorBrush(Windows.UI.Colors.Transparent);

            subjects = null;
            subjects = new List<Subject>();

            if ((bool)localSettings.Values["showInactive"]) {
                subjects = new List<Subject>(StudentData.subjects);
            } else {
                foreach (var subject in StudentData.subjects)
                    if (subject.IsActive)
                        subjects.Add(subject);
            }

            SubjectsListView.ItemsSource = subjects;

            // term or sem
            foreach (var subject in StudentData.subjects) {

                if ((bool)localSettings.Values["DashboardShowGradeOfTERM"]) {
                    foreach (var p in subject.Peroids) {
                        if ((p.IsActive) && ((p.Time == "T1") || (p.Time == "T2") || (p.Time == "T3") || (p.Time == "T4"))) {
                            subject.LetterGradeOnDashboard = p.LetterGrade;
                            subject.PercentageGradeOnDashboard = p.Percent;
                            break;
                        }
                    }
                } else {
                    foreach (var p in subject.Peroids) {
                        if ((p.IsActive) && ((p.Time == "S1") || (p.Time == "S2"))) {
                            subject.LetterGradeOnDashboard = p.LetterGrade;
                            subject.PercentageGradeOnDashboard = p.Percent;
                            break;
                        }
                    }
                    if (subject.PercentageGradeOnDashboard == "--") {
                        foreach (var p in subject.Peroids) {
                            if ((p.IsActive) && ((p.Time == "T1") || (p.Time == "T2") || (p.Time == "T3") || (p.Time == "T4"))) {
                                subject.LetterGradeOnDashboard = p.LetterGrade;
                                subject.PercentageGradeOnDashboard = p.Percent;
                                break;
                             }
                        }
                    } 
                }
                subject.ColorOnDashboard = StudentData.GetColor(subject.LetterGradeOnDashboard);
            }

            // assignments
            // normal -> navigate 
            if (StudentData.SelectedSubjectName != null) {
                if (AdaptiveStates.CurrentState == Normal) {
                    int index = 0;
                    foreach (var subject in StudentData.subjects) {
                        if (subject.Name == StudentData.SelectedSubjectName) {
                            break;
                        }
                        index += 1;
                    }
                    AssignmentsFrame.Navigate(typeof(AssignmentsPage), StudentData.SelectedSubjectName);
                }
            }

            // nothing here
            if (subjects.Count == 0) {
                nothing.Visibility = Visibility.Visible;
            }
        }

        private async Task KissingAsync() {

            IsKissing = true;

            // show
            ShowKissingBar.Begin();

            string result = null;
            // refresh
            try {
                result = await StudentData.Refresh();
            } catch (Exception e) {
                try {
                    ContentDialog ErrorContentDialog = new ContentDialog {
                        Title = LocalizedResources.GetString("unknownError/Text"),
                        Content = LocalizedResources.GetString("bad/Text") + "\r\n\r\n" + e.ToString(),
                        CloseButtonText = LocalizedResources.GetString("yesNet/Text"),
                    };
                    await ErrorContentDialog.ShowAsync();
                } catch (Exception) { }
            }

            // hide
            HideKissingBar.Begin();
            await Task.Delay(300);
            HideKissingBarRow.Begin();

            switch (result) {
                case "okey dokey":
                    StatusTextBlock.Text = "Refreshed!";
                    break;
                case "error":
                    StatusTextBlock.Text = LocalizedResources.GetString("cannotConnect/Text");
                    // Tawny
                    KissingBar.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 202, 81, 0));
                    break;
                case null:
                    StatusTextBlock.Text = LocalizedResources.GetString("unknownError/Text");
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

            StatusTextBlock.Text = "Refreshing ...";
            ProcesR.Visibility = Visibility.Visible;
            KissingBar.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 99, 177));

            Initialize();

            IsKissing = false;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {

            if (App.isMainPageFirstTimeInit) {
                App.isMainPageFirstTimeInit = !App.isMainPageFirstTimeInit;
                try {
                    await KissingAsync();
                } catch(Exception) {
                    // todo error
                }
            }

            if (AdaptiveStates.CurrentState == Narrow && SubjectsListView.SelectedIndex >= 0) {
                GoToDetailButton.Visibility = Visibility.Visible;
                EditButton.Visibility = Visibility.Visible;
                Filter.Visibility = Visibility.Visible;
            } else if (AdaptiveStates.CurrentState == Normal) {
                if (StudentData.SelectedSubjectName == null) {
                    GoToDetailButton.Visibility = Visibility.Collapsed;
                    EditButton.Visibility = Visibility.Collapsed;
                    Filter.Visibility = Visibility.Collapsed;
                }
            } else if (AdaptiveStates.CurrentState == Narrow && SubjectsListView.SelectedIndex == -1) {
                GoToDetailButton.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Collapsed;
                Filter.Visibility = Visibility.Collapsed;
            }
        }

        private void Leap(object sender, RoutedEventArgs e) {
            if (AdaptiveStates.CurrentState == Narrow && SubjectsListView.SelectedIndex >= 0) {
                StudentData.SelectedSubjectName = subjects[SubjectsListView.SelectedIndex].Name;
                Frame.Navigate(typeof(AssignmentsPage));
            }
        }

        private async void GPA_But_Click(object sender, RoutedEventArgs e) {
            await new SchoolPower.Views.Dialogs.GPADialog().ShowAsync();
        }

        private async void Refresh_But_Click(object sender, RoutedEventArgs e) {
            await KissingAsync();
        }

        private async void Filter_Click(object sender, RoutedEventArgs e) {
            int index = 0;
            foreach (var subject in StudentData.subjects) {
                if (subject.Name == StudentData.SelectedSubjectName) {
                    break;
                }
                index += 1;
            }

            var dialog = new SchoolPower.Views.Dialogs.FilterDialog(StudentData.subjects[index].CatagoryList, StudentData.subjects[index].Peroids);
            await dialog.ShowAsync();
            StudentData.AssignmentFilterParam = dialog.Result;
            AssignmentsFrame.Navigate(typeof(AssignmentsPage), StudentData.SelectedSubjectName);
        }

        private void SubjectsListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            // reset data
            StudentData.AssignmentFilterParam["time"] = null;
            StudentData.AssignmentFilterParam["cata"] = null;

            // remove img
            NoGradeIcnImg.Visibility = Visibility.Collapsed;

            if (SubjectsListView.SelectedIndex != -1 ) {

                // store
                StudentData.SubjectListViewAddedItems= e.AddedItems;
                if (StudentData.SubjectListViewRemovedItems == null)
                    StudentData.SubjectListViewRemovedItems = e.RemovedItems;

                StudentData.SelectedSubjectName = subjects[SubjectsListView.SelectedIndex].Name;

                // Assign detail DataTemplate 
                foreach (var item in e.AddedItems) {
                    ListViewItem listViewItem = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                    listViewItem.ContentTemplate = (DataTemplate)this.Resources["SubjectsListTemplate_Detail"];
                }
                // Remove compact DataTemplate
                foreach (var item in e.RemovedItems) {
                    ListViewItem listViewItem = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                    listViewItem.ContentTemplate = (DataTemplate)this.Resources["SubjectsListTemplate_Compact"];
                }

                // normal -> navigate 
                if (AdaptiveStates.CurrentState == Normal) {
                    AssignmentsFrame.Navigate(typeof(AssignmentsPage), StudentData.SelectedSubjectName);
                    EditButton.Visibility = Visibility.Visible;
                    Filter.Visibility = Visibility.Visible;
                }

                // narrow -> show button
                else if (AdaptiveStates.CurrentState == Narrow)
                    GoToDetailButton.Visibility = Visibility.Visible;
            }
        }

        private async void PeroidsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            GridView gridView = sender as GridView;
            GradeInfoDialog dialog = new GradeInfoDialog(subjects[SubjectsListView.SelectedIndex].Peroids[gridView.SelectedIndex]);
            await dialog.ShowAsync();
        }

        private void AdaptiveStatesChanged(object sender, VisualStateChangedEventArgs e) {
            if (AdaptiveStates.CurrentState == Narrow && SubjectsListView.SelectedIndex >= 0) {
                GoToDetailButton.Visibility = Visibility.Visible;
                EditButton.Visibility = Visibility.Collapsed;
                Filter.Visibility = Visibility.Collapsed;
            } else if (AdaptiveStates.CurrentState == Normal && SubjectsListView.SelectedIndex >= 0) {
                AssignmentsFrame.Navigate(typeof(AssignmentsPage), subjects[SubjectsListView.SelectedIndex].Name);
                GoToDetailButton.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Visible;
                Filter.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            Debug.WriteLine(StudentData.SelectedSubjectName);
            if (StudentData.SelectedSubjectName != null) {
                NoGradeIcnImg.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Visible;
                Filter.Visibility = Visibility.Visible;
            } else {
                EditButton.Visibility = Visibility.Collapsed;
                Filter.Visibility = Visibility.Collapsed;
            }
        }
    }
}
