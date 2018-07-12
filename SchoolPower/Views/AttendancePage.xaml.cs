using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SchoolPower.Models;

namespace SchoolPower.Views {

    public sealed partial class AttendancePage : Page {

        private List<AttendanceItem> attendanceItems;
        private ItemsWrapGrid _itemsWrapGrid;

        private double GridHeight;

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

        private async void Refresh_But_Click(object sender, RoutedEventArgs e) {
            await StudentData.Refresh();
            InitializeComponent();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e) {

        }

        private void AttendanceDetailGrid_Loaded(object sender, RoutedEventArgs e) {
            _itemsWrapGrid = sender as ItemsWrapGrid;
            _itemsWrapGrid.MaximumRowsOrColumns = GetNumberOfRows();
        }
        /*
        private int AttendanceDetailGridColumns {
            set { SetValue(MaxColumnProperty, value); }
        }
        private static readonly DependencyProperty MaxColumnProperty =
            DependencyProperty.Register(nameof(AttendanceDetailGridColumns), typeof(int), typeof(AttendancePage), new PropertyMetadata(0));
            */
        int GetNumberOfRows() {
            return (int)((Template10.Controls.ModalDialog)Window.Current.Content).ActualHeight / 50 -1;
            // return (int)AttendanceDetailGridView.ActualHeight / 50;
        }

    }
}
