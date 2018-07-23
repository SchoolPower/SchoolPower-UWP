using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SchoolPower.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using System;
using SchoolPower.Views.Dialogs;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

namespace SchoolPower.Views {
    public sealed partial class SubjectsAssignmentsPage : Page {
        private List<Subject> subjects;
        public static string selectedSubjectName;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public SubjectsAssignmentsPage() {
            Views.Shell.HamburgerMenu.IsFullScreen = !true;
            Views.Shell.HamburgerMenu.HamburgerButtonVisibility = !false ? Visibility.Visible : Visibility.Collapsed;
            Initialize();
        }

        void Initialize() {

            InitializeComponent();

            subjects = null;
            subjects = new List<Subject>();

            if ((bool)localSettings.Values["showInactive"]) {
                subjects = new List<Subject>(StudentData.subjects);
            } else {
                foreach (var subject in StudentData.subjects)
                    if (subject.IsActive)
                        subjects.Add(subject);
            }

            foreach (var subject in subjects) {
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
                        } else if ((p.IsActive) && ((p.Time == "T1") || (p.Time == "T2") || (p.Time == "T3") || (p.Time == "T4"))) {
                            subject.LetterGradeOnDashboard = p.LetterGrade;
                            subject.PercentageGradeOnDashboard = p.Percent;
                            break;
                        }
                    }
                }
            }

            SubjectsListView.ItemsSource = subjects;

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
            // Frame.Navigate(typeof(MainPage));
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {

            if (App.isMainPageFirstTimeInit) {
                App.isMainPageFirstTimeInit = !App.isMainPageFirstTimeInit;
                await KissingAsync();
            }

            if (AdaptiveStates.CurrentState == Narrow && SubjectsListView.SelectedIndex >= 0) {
                GoToDetailButton.Visibility = Visibility.Visible;
                EditButton.Visibility = Visibility.Visible;
            } else if (AdaptiveStates.CurrentState == Normal) {
                GoToDetailButton.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Collapsed;
            } else if (AdaptiveStates.CurrentState == Narrow && SubjectsListView.SelectedIndex == -1) {
                GoToDetailButton.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Collapsed;
            }
        }


        private void Leap(object sender, RoutedEventArgs e) {
            if (AdaptiveStates.CurrentState == Narrow && SubjectsListView.SelectedIndex >= 0) {
                StudentData.SelectedSubjectName = subjects[SubjectsListView.SelectedIndex].Name;
                Frame.Navigate(typeof(AssignmentsPage));
            }
        }

        private async void GPA_But_Click(object sender, RoutedEventArgs e) {
            SchoolPower.Views.Dialogs.GPADialog dialog = new SchoolPower.Views.Dialogs.GPADialog();
            await dialog.ShowAsync();
        }

        private async void Refresh_But_Click(object sender, RoutedEventArgs e) {
            await KissingAsync();
        }

        private void SubjectsListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {

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
            } else if (AdaptiveStates.CurrentState == Normal && SubjectsListView.SelectedIndex >= 0) {
                AssignmentsFrame.Navigate(typeof(AssignmentsPage), subjects[SubjectsListView.SelectedIndex].Name);
                GoToDetailButton.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) { /*
            if (StudentData.SelectedSubjectName != null) {
                int index = 0;
                foreach (var subject in this.subjects) {
                    if (subject.Name == StudentData.SelectedSubjectName) {
                        break;
                    }
                    index += 1;
                }
                SubjectsListViewLastSelection = index;
                //SubjectsListView.SelectedItem = SubjectsListView.Items[index];
                SubjectsListView_SelectionChanged(this, new SelectionChangedEventArgs(StudentData.SubjectListViewRemovedItems, StudentData.SubjectListViewAddedItems));
            }*/
        }
    }
}
