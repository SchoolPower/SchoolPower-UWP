using SchoolPower.Models;
using System;
using System.Collections.Generic;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SchoolPower.Views.Dialogs {
    public sealed partial class FilterDialog: ContentDialog {

        private List<string> CatagoryList = new List<string>();
        private List<Peroid> PeroidList = new List<Peroid>();

        public Dictionary<string, string> Result;

        public FilterDialog(List<Catagory> catagoryList, List<Peroid> peroidList) {
            this.InitializeComponent();
            foreach (var cata in catagoryList) {
                CatagoryList.Add(cata.Name);
            }
            PeroidList = peroidList;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Result = null;
            Result = new Dictionary<string, string>();
            try {
                Result["time"] = TimeCombo.SelectedValue.ToString();
            } catch (System.NullReferenceException) {
                Result["time"] = null;
            }
            try {
                Result["cata"] = CataCombo.SelectedValue.ToString();
            } catch (System.NullReferenceException) {
                Result["cata"] = null;
            }
            Hide();
        }

        private void CataCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void TimeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }
        
        private void CataCombo_Loaded(object sender, RoutedEventArgs e) {
            foreach (var v in CatagoryList) {
                CataCombo.Items.Add(v);
            }
        }

        private void TimeCombo_Loaded(object sender, RoutedEventArgs e) {
            List<string> timeList = new List<string>();
            foreach (var v in PeroidList) {
                if (!TimeCombo.Items.Contains(v.Time)) {
                    TimeCombo.Items.Add(v.Time);
                }
            }
        }
    }
}
