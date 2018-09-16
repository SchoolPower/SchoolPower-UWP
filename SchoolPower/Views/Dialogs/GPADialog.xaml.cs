using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SchoolPower.Models;

namespace SchoolPower.Views.Dialogs {
    public sealed partial class GPADialog : ContentDialog {

        private string selectedCombo = "";
        private string selectedRadioBut = "";
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public GPADialog() {
            this.InitializeComponent();
            comboBox.SelectedIndex = 0;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e) {
            RadioButton radioButton = sender as RadioButton;
            selectedRadioBut = radioButton.Tag.ToString();
            switch (radioButton.Tag.ToString()) {
                case "All":
                    GPA.Text = StudentData.GetAllGPA(selectedCombo).ToString() + "%";
                    break;
                case "Custom":
                    switch (localSettings.Values["CalculateRule"]) {
                        case "0":
                            GPA.Text = StudentData.GetSelectedGPA(selectedCombo).ToString() + "%";
                            break;
                        case "1":
                            GPA.Text = StudentData.GetSelectedGPA(selectedCombo, 3).ToString() + "%";
                            break;
                        case "2":
                            GPA.Text = StudentData.GetSelectedGPA(selectedCombo, 4).ToString() + "%";
                            break;
                        case "3":
                            GPA.Text = StudentData.GetSelectedGPA(selectedCombo, 5).ToString() + "%";
                            break;
                        default:
                            GPA.Text = "N/A";
                            break;
                    }
                    break;
                case "Official":
                    try {
                        var v = StudentData.info.GPA;
                        if (StudentData.info.GPA == null) 
                            GPA.Text = "N/A";
                        else 
                            GPA.Text = StudentData.info.GPA + "%";
                    } catch (System.NullReferenceException) {
                        GPA.Text = "NaN%";
                    } break;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var container = sender as ComboBox;
            var selected = container.SelectedItem as ComboBoxItem;
            selectedCombo = selected.Content.ToString();
            switch (selectedRadioBut){
                case "All":
                    GPA.Text = StudentData.GetAllGPA(selectedCombo).ToString() + "%";
                    break;
                case "Custom":
                    switch (localSettings.Values["CalculateRule"]) {
                        case "0":
                            GPA.Text = StudentData.GetSelectedGPA(selectedCombo).ToString() + "%";
                            break;
                        case "1":
                            GPA.Text = StudentData.GetSelectedGPA(selectedCombo, 3).ToString() + "%";
                            break;
                        case "2":
                            GPA.Text = StudentData.GetSelectedGPA(selectedCombo, 4).ToString() + "%";
                            break;
                        case "3":
                            GPA.Text = StudentData.GetSelectedGPA(selectedCombo, 5).ToString() + "%";
                            break;
                    } break;
                case "Official":
                    try {
                        if (StudentData.info.GPA == null) { }
                        GPA.Text = StudentData.info.GPA + "%";
                    } catch (System.NullReferenceException) {
                        GPA.Text = "NaN%";
                    } break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Hide();
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e) {

        }
    }
}
