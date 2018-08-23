using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SchoolPower.Models;
using System.Diagnostics;

namespace SchoolPower.Views {

    public sealed partial class AttendancePage : Page {

        private List<AttendanceItem> attendanceItems;

        int GetNumberOfRows() {
            return (int)(((Template10.Controls.ModalDialog)Window.Current.Content).ActualHeight / 56) - 1;
        }

        public AttendancePage() {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) => { };
            attendanceItems = StudentData.attendances;
            if (attendanceItems.Count() == 0) {
                NoAttendanceImg.Visibility = Visibility.Visible;
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {

        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e) {

        }

        private void AttendanceDetailGrid_Loaded(object sender, RoutedEventArgs e) {
            var gridView = sender as GridView;
            var itemsWrapGrid = (ItemsWrapGrid)gridView.ItemsPanelRoot;
            itemsWrapGrid.MaximumRowsOrColumns = GetNumberOfRows();
        }

        private void AttendanceDetailGrid_SizeChanged(object sender, SizeChangedEventArgs e) {
            var gridView = sender as GridView;
            var itemsWrapGrid = (ItemsWrapGrid)gridView.ItemsPanelRoot;
            itemsWrapGrid.MaximumRowsOrColumns = GetNumberOfRows();
        }
    }
}
