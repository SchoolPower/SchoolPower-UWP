using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SchoolPower.Models;

namespace SchoolPower.Views.Dialogs {
    public sealed partial class AssInfoDialog : ContentDialog {
        public AssInfoDialog(AssignmentItem ass) {
            this.InitializeComponent();
            Date.Text     = ass.Date;
            Name.Text     = ass.Name;
            Score.Text    = ass.Score + "/" + ass.MaximumScore;
            Weight.Text   = ass.Weight;
            Cata.Text     = ass.Category;

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
