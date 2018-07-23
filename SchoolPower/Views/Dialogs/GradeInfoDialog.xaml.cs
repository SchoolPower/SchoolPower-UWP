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
    public sealed partial class GradeInfoDialog : ContentDialog {
        public GradeInfoDialog(Peroid peroid) {
            this.InitializeComponent();
            Time.Text    = peroid.Time;
            Percent.Text = peroid.Percent;
            Letter.Text  = peroid.LetterGrade;
            Eval.Text    = peroid.Eval;
            try {
                if (peroid.Comment == null) { }
                Comment.Text = peroid.Comment;
            } catch (System.NullReferenceException) {
                Comment.Text = "N/A";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Hide();
        }
    }
}
