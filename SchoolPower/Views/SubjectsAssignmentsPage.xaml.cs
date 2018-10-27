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
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace SchoolPower.Views {
    public sealed partial class SubjectsAssignmentsPage : Page {
        private List<Subject> subjects;
        private bool IsKissing = false;
        private readonly object kissLock = new object();
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public SubjectsAssignmentsPage() {
            Views.Shell.HamburgerMenu.IsFullScreen = !true;
            Views.Shell.HamburgerMenu.HamburgerButtonVisibility = !false ? Visibility.Visible : Visibility.Collapsed;
            Initialize();
            pageHeader.Background = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["CustomColor"]);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
        }

        void Initialize() {

            localSettings.Values["IsFirstTimeLogin"] = false;

            try {
                StudentData.AssignmentFilterParam["time"] = null;
                StudentData.AssignmentFilterParam["cata"] = null;
            } catch (Exception) {
                StudentData.AssignmentFilterParam = new Dictionary<string, string> {
                    { "time", null},
                    { "cata", null}
                };
            }

            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                async (window, e) => {
                    switch (e.VirtualKey) {
                        case VirtualKey.F5:
                            lock (kissLock) {
                                KissingAsync();
                            }/*
                            if (!IsKissing) {
                                IsKissing = true;
                                try {
                                    await KissingAsync();
                                } catch (Exception) { }
                            }*/
                            break;
                        case VirtualKey.F6:
                            try {
                                await new SchoolPower.Views.Dialogs.GPADialog().ShowAsync();
                            } catch (Exception) { }
                            break;
                    }
                };

            InitializeComponent();

            // apply theme
            var accentColor = App.themes[(int)localSettings.Values["ColorBoardSelectedIndex"]].AccentColor;
            Application.Current.Resources["SystemAccentColor"] = accentColor;
            Application.Current.Resources["CustomColor"] = accentColor;

            // Application.Current.Resources["SystemAccentColor"] = new SolidColorBrush(Windows.UI.Colors.Transparent);

            // data
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

                int std = 0;
                try {
                    std = Convert.ToInt32(subject.PercentageGradeOnDashboard);
                } catch (Exception) { }

                foreach (var cata in subject.CatagoryList) {
                    if ((cata.Percentage * 100) >= std ) {
                        cata.Color = new SolidColorBrush(Windows.UI.Color.FromArgb(200, 0, 121, 107));
                    } else if ((cata.Percentage * 100) >= (std * .8)) {
                        cata.Color = new SolidColorBrush(Windows.UI.Color.FromArgb(200, 56, 124, 60));
                    } else if ((cata.Percentage * 100) >= (std * .6)) {
                        cata.Color = new SolidColorBrush(Windows.UI.Color.FromArgb(200, 255, 179, 0));
                    } else if ((cata.Percentage * 100) >= (std * .4)) {
                        cata.Color = new SolidColorBrush(Windows.UI.Color.FromArgb(200, 255, 87, 34));
                    }
                }
            }

            foreach (var subjectOld in StudentData.subjectsOld) {
                if ((bool)localSettings.Values["DashboardShowGradeOfTERM"]) {
                    foreach (var p in subjectOld.Peroids) {
                        if ((p.IsActive) && ((p.Time == "T1") || (p.Time == "T2") || (p.Time == "T3") || (p.Time == "T4"))) {
                            subjectOld.PercentageGradeOnDashboard = p.Percent;
                            break;
                        }
                    }
                } else {
                    foreach (var p in subjectOld.Peroids) {
                        if ((p.IsActive) && ((p.Time == "S1") || (p.Time == "S2"))) {
                            subjectOld.PercentageGradeOnDashboard = p.Percent;
                            break;
                        }
                    }
                    if (subjectOld.PercentageGradeOnDashboard == "--") {
                        foreach (var p in subjectOld.Peroids) {
                            if ((p.IsActive) && ((p.Time == "T1") || (p.Time == "T2") || (p.Time == "T3") || (p.Time == "T4"))) {
                                subjectOld.PercentageGradeOnDashboard = p.Percent;
                                break;
                            }
                        }
                    }
                }
            }

            // +-
            foreach (var subject in StudentData.subjects) {
                foreach (var subjectOld in StudentData.subjectsOld) {
                    if (subjectOld.Name == subject.Name) {
                        var grade = 0;
                        var gradeOld = 0;
                        int.TryParse(subject.PercentageGradeOnDashboard, out grade);
                        int.TryParse(subjectOld.PercentageGradeOnDashboard, out gradeOld);
                        var result = grade - gradeOld;
                        if (result == 0) {
                            subject.ChangeInGrade = "0";
                            subject.GradeChangePanelVisibility = Visibility.Collapsed;
                            subject.GradeChangePanelColor = new SolidColorBrush(Windows.UI.Colors.Transparent);
                        } else if (result > 0) {
                            subject.ChangeInGrade = "+" + result.ToString();
                            subject.GradeChangePanelVisibility = Visibility.Visible;
                            subject.GradeChangePanelColor = new SolidColorBrush(Windows.UI.Colors.DarkCyan);
                        } else if (result < 0) {
                            subject.ChangeInGrade = result.ToString();
                            subject.GradeChangePanelVisibility = Visibility.Visible;
                            subject.GradeChangePanelColor = new SolidColorBrush(Windows.UI.Colors.DarkRed);
                        }
                    }
                }
            }

            // assignments
            // normal -> navigate 
            if (StudentData.SelectedSubject != null) {
                if (AdaptiveStates.CurrentState == Normal) {
                    int index = 0;
                    foreach (var subject in StudentData.subjects) {
                        if (subject.Name == StudentData.SelectedSubject.Name) {
                            break;
                        }
                        index += 1;
                    }
                    AssignmentsFrame.Navigate(typeof(AssignmentsPage), StudentData.SelectedSubject);
                }
            }

            // nothing here
            if (subjects.Count == 0) {
                nothing.Visibility = Visibility.Visible;
            }
        }

        private async Task KissingAsync() {

            if (IsKissing) {
                return;
            } else {

                IsKissing = true;

                KissingBar.Background = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);

                // show
                ShowKissingBar.Begin();

                string result = null;
                ContentDialog ErrorContentDialog = new ContentDialog();
                // refresh
                try {
                    result = await StudentData.Refresh();
                } catch (Exception e) {
                    try {
                        ErrorContentDialog.Title = LocalizedResources.GetString("unknownError/Text");
                        ErrorContentDialog.Content = LocalizedResources.GetString("bad/Text") + "\r\n\r\n" + e.ToString();
                        ErrorContentDialog.CloseButtonText = LocalizedResources.GetString("yesNet/Text");
                    } catch (Exception) { }
                }

                // hide
                HideKissingBar.Begin();
                await Task.Delay(100);
                HideKissingBarRow.Begin();

                switch (result) {
                    case "okey dokey":
                        StatusTextBlock.Text = "Updated!";
                        break;
                    case "error":
                        StatusTextBlock.Text = LocalizedResources.GetString("cannotConnect/Text");
                        // Tawny
                        KissingBar.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 202, 81, 0));
                        // await ErrorContentDialog.ShowAsync();
                        break;
                    case null:
                        StatusTextBlock.Text = LocalizedResources.GetString("unknownError/Text");
                        // Tawny
                        KissingBar.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 202, 81, 0));
                        // await ErrorContentDialog.ShowAsync();
                        break;
                }

                ProcesR.Visibility = Visibility.Collapsed;
                ShowKissingBar.Begin();
                await Task.Delay(1500);
                HideKissingBar.Begin();
                await Task.Delay(200);
                HideKissingBarRow.Begin();

                StatusTextBlock.Text = "Purr ...";
                ProcesR.Visibility = Visibility.Visible;
                KissingBar.Background = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);

                Initialize();

                IsKissing = false;
            }
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
                if (StudentData.SelectedSubject == null) {
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
                StudentData.SelectedSubject = subjects[SubjectsListView.SelectedIndex];
                Frame.Navigate(typeof(AssignmentsPage));
            }
        }

        private async void GPA_But_Click(object sender, RoutedEventArgs e) {
            await new SchoolPower.Views.Dialogs.GPADialog().ShowAsync();
        }

        private async void Refresh_But_Click(object sender, RoutedEventArgs e) {
            await KissingAsync();
        }

        private void SubjectsListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            // reset data
            try {
                StudentData.AssignmentFilterParam["time"] = null;
                StudentData.AssignmentFilterParam["cata"] = null;
            } catch (Exception) {
                StudentData.AssignmentFilterParam = new Dictionary<string, string> {
                    { "time", null},
                    { "cata", null}
                };
            }

            // remove img
            NoGradeIcnImg.Visibility = Visibility.Collapsed;

            if (SubjectsListView.SelectedIndex != -1 ) {

                // store
                StudentData.SubjectListViewAddedItems= e.AddedItems;
                if (StudentData.SubjectListViewRemovedItems == null)
                    StudentData.SubjectListViewRemovedItems = e.RemovedItems;

                StudentData.SelectedSubject = subjects[SubjectsListView.SelectedIndex];

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
                    AssignmentsFrame.Navigate(typeof(AssignmentsPage), StudentData.SelectedSubject);
                    EditButton.Visibility = Visibility.Visible;
                    Filter.Visibility = Visibility.Visible;
                }

                // narrow -> show button
                else if (AdaptiveStates.CurrentState == Narrow)
                    GoToDetailButton.Visibility = Visibility.Visible;
            }
        }

        private async void PeroidsGridView_ItemClick(object sender, ItemClickEventArgs e) {
            GridView gridView = sender as GridView;
            Peroid p = (Peroid)e.ClickedItem;
            GradeInfoDialog dialog = new GradeInfoDialog(p);
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
            if (StudentData.SelectedSubject != null) {
                NoGradeIcnImg.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Visible;
                Filter.Visibility = Visibility.Visible;
            } else {
                EditButton.Visibility = Visibility.Collapsed;
                Filter.Visibility = Visibility.Collapsed;
            }
        }

        private void TeacherNameTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            var txt = sender as TextBlock;
            if (txt.Text == StudentData.SelectedSubject.TeacherName && StudentData.SelectedSubject.TeacherEmail != "") {
                txt.Text = StudentData.SelectedSubject.TeacherEmail;
                var dataPackage = new DataPackage();
                dataPackage.SetText(StudentData.SelectedSubject.TeacherEmail);
                Clipboard.SetContent(dataPackage);
            } else if (txt.Text == StudentData.SelectedSubject.TeacherEmail) {
                txt.Text = StudentData.SelectedSubject.TeacherName;
            }
        }

        private async void Filter_Click(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            fly.Hide();
            int index = 0;
            foreach (var subject in StudentData.subjects) {
                if (subject.Name == StudentData.SelectedSubject.Name) {
                    break;
                }
                index += 1;
            }

            var dialog = new FilterDialog(StudentData.subjects[index].CatagoryList, StudentData.subjects[index].Peroids);
            await dialog.ShowAsync();
            StudentData.AssignmentFilterParam = dialog.Result;
            if (StudentData.AssignmentFilterParam == null) {
                StudentData.AssignmentFilterParam = new Dictionary<string, string> {
                    { "time", null},
                    { "cata", null}
                };
            }
            AssignmentsFrame.Navigate(typeof(AssignmentsPage), StudentData.SelectedSubject);
        }

        private void ExperimentalFilter_Click(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            fly.Hide();
            AssignmentsFrame.Navigate(typeof(Views.CategoricalAssignmentsPage));
        }
    }
}
