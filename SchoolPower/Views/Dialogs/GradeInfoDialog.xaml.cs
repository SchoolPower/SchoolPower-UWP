using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
