using SchoolPower.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SchoolPower.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoricalAssignmentsPage: Page {
        private ObservableCollection<Catagory> catagories;
        public CategoricalAssignmentsPage() {
            this.InitializeComponent();
        }

        int GetNumberOfRows() {
            return (int)(((Template10.Controls.ModalDialog)Window.Current.Content).ActualHeight / 75) - 1;
        }

        private void Border_Loaded(object sender, RoutedEventArgs e) {

        }

        private void CatagoryWarp_Loaded(object sender, RoutedEventArgs e) {
            var gridView = sender as GridView;
            var itemsWrapGrid = (ItemsWrapGrid)gridView.ItemsPanelRoot;
            itemsWrapGrid.MaximumRowsOrColumns = GetNumberOfRows();
        }

        private void CatagoryWarp_SizeChanged(object sender, SizeChangedEventArgs e) {
            var gridView = sender as GridView;
            var itemsWrapGrid = (ItemsWrapGrid)gridView.ItemsPanelRoot;
            itemsWrapGrid.MaximumRowsOrColumns = GetNumberOfRows();
        }

        private void CatagoryGridView_ItemClick(object sender, ItemClickEventArgs e) {
            var catagory = (Catagory)e.ClickedItem;
            StudentData.AssignmentFilterParam["cata"] = catagory.Name;
            Frame.Navigate(typeof(AssignmentsPage));
        }

        void Init() {

            catagories = new ObservableCollection<Catagory>(StudentData.SelectedSubject.CatagoryList);

            InitializeComponent();
            pageHeader.Background = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources["CustomColor"]);
            CatagoryGridView.ItemsSource = catagories;
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            Init();
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) {

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
    }
}
