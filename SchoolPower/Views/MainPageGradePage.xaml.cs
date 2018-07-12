﻿using SchoolPower.Models;
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
using SchoolPower.Views.Dialogs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SchoolPower.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPageGradePage : Page {
        private List<AssignmentItem> assignments;
        private ItemsWrapGrid _itemsWrapGrid;
        int index = 0;

        public MainPageGradePage() {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            int index = 0;
            string selectdeSubject = (string)e.Parameter;
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

        private void GradeDetailGrid_Loaded(object sender, RoutedEventArgs e) {
            _itemsWrapGrid = sender as ItemsWrapGrid;
            _itemsWrapGrid.MaximumRowsOrColumns = GetNumberOfRows();
        }

        public static int GetNumberOfRows() {
            return (int)((Template10.Controls.ModalDialog)Window.Current.Content).ActualHeight / 80;
        }

        private async void GradeDetailGridView_Tapped(object sender, TappedRoutedEventArgs e) {
            AssInfoDialog dialog = new AssInfoDialog(assignments[GradeDetailGridView.SelectedIndex]);
            await dialog.ShowAsync();
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) {

        }

        private void Border_Loaded(object sender, RoutedEventArgs e) {
            Border border = sender as Border;
            if (assignments[index].IsNew) {
                border.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 99, 177));
            } else {
                border.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
            }
            index += 1;
        }
    }
}
