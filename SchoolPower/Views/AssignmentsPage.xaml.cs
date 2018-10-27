using SchoolPower.Models;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SchoolPower.Views.Dialogs;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SchoolPower.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AssignmentsPage : Page {
        private ObservableCollection<AssignmentItem> assignments;
        private List<string> CatagoryList = new List<string>();
        private List<Peroid> TimeList;
        private string selectdeSubject = StudentData.SelectedSubject.Name;

        int GetNumberOfRows() {
            return (int)(((Template10.Controls.ModalDialog)Window.Current.Content).ActualHeight / 75) -1;
        }

        public AssignmentsPage() {
        }

        void Init() {

            assignments = new ObservableCollection<AssignmentItem>();
            List<AssignmentItem> assss = StudentData.SelectedSubject.Assignments;

            foreach (var cata in StudentData.SelectedSubject.CatagoryList) {
                CatagoryList.Add(cata.Name);
            }
            TimeList = StudentData.SelectedSubject.Peroids;//  subjects[index].Peroids;

            var AssignmentFilter = StudentData.AssignmentFilterParam;
            try {
                if (AssignmentFilter["time"] != null || AssignmentFilter["cata"] != null) {
                    if (AssignmentFilter["time"] == null) {
                        foreach (var ass in assss) {
                            if (ass.Category == AssignmentFilter["cata"]) {
                                assignments.Add(ass);
                            }
                        }
                    } else if (AssignmentFilter["cata"] == null) {
                        foreach (var ass in assss) {
                            if (new List<string>(ass.Terms).Contains(AssignmentFilter["time"])) {
                                assignments.Add(ass);
                            }
                        }
                    } else {
                        foreach (var ass in assss) {
                            if (ass.Category == AssignmentFilter["cata"] && new List<string>(ass.Terms).Contains(AssignmentFilter["time"])) {
                                assignments.Add(ass);
                            }
                        }
                    }
                } else {
                    assignments = new ObservableCollection<AssignmentItem>(assss);
                }
            } catch (Exception) { }
            //foreach(var v in )

            InitializeComponent();
            pageHeader.Background = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["CustomColor"]);
            AssignmentsGridView.ItemsSource = assignments;
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            Init();
        }

        private void AssignmentsWarp_Loaded(object sender, RoutedEventArgs e) {
            var gridView = sender as GridView;
            var itemsWrapGrid = (ItemsWrapGrid)gridView.ItemsPanelRoot;
            itemsWrapGrid.MaximumRowsOrColumns = GetNumberOfRows();
        }

        private void AssignmentsWarp_SizeChanged(object sender, SizeChangedEventArgs e) {
            var gridView = sender as GridView;
            var itemsWrapGrid = (ItemsWrapGrid)gridView.ItemsPanelRoot;
            itemsWrapGrid.MaximumRowsOrColumns = GetNumberOfRows();
        }


        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) { 

        }

        int index = 0;
        private void Border_Loaded(object sender, RoutedEventArgs e) { 
            Border border = sender as Border;
            // if (assignments[this.index].IsNew) {
               //  border.Background = StudentData.GetColor(assignments[index].LetterGrade);
                // new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 99, 177));
            //} 
            // this.index += 1; 
        }

        private void Head_Loaded(object sender, RoutedEventArgs e) {
            var head = sender as RelativePanel;
            if (AdaptiveStates.CurrentState == Narrow) 
                head.Visibility = Visibility.Visible;
            else if (AdaptiveStates.CurrentState == Normal)
                head.Visibility = Visibility.Collapsed;
        }

        private void AdaptiveStatesChanged(object sender, VisualStateChangedEventArgs e) {
            if (AdaptiveStates.CurrentState == Normal) {
                if (Frame.CanGoBack) {
                    Frame.GoBack();
                }
            }
        }

        private async void AssignmentsGridView_ItemClick(object sender, ItemClickEventArgs e) {
            var assignment = (AssignmentItem)e.ClickedItem;
            AssInfoDialog dialog = new AssInfoDialog(assignment);
            await dialog.ShowAsync();
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
            Init();

            // InitializeComponent();
        }

        private void ExperimentalFilter_Click(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            fly.Hide();
            Frame.Navigate(typeof(Views.CategoricalAssignmentsPage));
        }
    }
}
