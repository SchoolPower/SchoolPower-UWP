using SchoolPower.Models;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SchoolPower.Views.Dialogs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SchoolPower.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AssignmentsPage : Page {
        private List<AssignmentItem> assignments;
        private string selectdeSubject = StudentData.SelectedSubjectName;

        int GetNumberOfRows() {
            return (int)(((Template10.Controls.ModalDialog)Window.Current.Content).ActualHeight / 75) -1;
        }

        public AssignmentsPage() {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            int index = 0;
            foreach (var subject in StudentData.subjects) {
                if (subject.Name == selectdeSubject) {
                    break;
                }
                index += 1;
            }
            assignments = StudentData.subjects[index].Assignments;
            InitializeComponent();
            Window.Current.SizeChanged += Current_SizeChanged;
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
            if (assignments[this.index].IsNew) {
                border.Background = StudentData.GetColor(assignments[index].LetterGrade);
                // new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 99, 177));
            } 
            this.index += 1; 
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
    }
}
