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
using SchoolPower.Models;

namespace SchoolPower.Views.Dialogs {
    public sealed partial class AssInfoDialog : ContentDialog {
        public AssInfoDialog(AssignmentItem ass) {
            this.InitializeComponent();
            Date.Text     = ass.Date;
            Name.Text     = ass.Name;
            Score.Text    = ass.Score + "/" + ass.MaximumScore;
            Weight.Text   = ass.Weight;

            if (ass.Percentage == null) {
                Percent.Text = "--";
            } else {
                Percent.Text = ass.Percentage;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Hide();
        }
    }
}
