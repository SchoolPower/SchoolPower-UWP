using SchoolPower.Models;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SchoolPower.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChartPage : Page {
        public ChartPage() {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) => { };

            InitLineChartContent();

        }

        void InitLineChartContent() {

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            bool showGradeOfTerm = (bool)localSettings.Values["DashboardShowGradeOfTERM"];

            foreach (var subject in StudentData.subjects) {

                List<SubjectHistory> itemSource = new List<SubjectHistory>();
                string subjectName = subject.Name;

                // show Inactive

                foreach (var Day in StudentData.historyDatas) { // for each day
                    int percent = 0;
                    foreach (var subjectData in Day.SubjectHistoryData) { // for each subject 
                        if (subjectName == subjectData.Subject) {
                            foreach (var peroid in subjectData.Peroids) { // for each peroid 
                                if (showGradeOfTerm) 
                                    if ((peroid.Peroid == "T1") || (peroid.Peroid == "T2") || (peroid.Peroid == "T3") || (peroid.Peroid == "T4")) 
                                        percent = peroid.Percent;
                                else
                                    if ((peroid.Peroid == "S1") || (peroid.Peroid == "S2")) 
                                        percent = peroid.Percent;
                                    
                                if (percent == 0) 
                                    if (peroid.Peroid == "Y1") 
                                        percent = peroid.Percent;
                            }
                        }
                    }
                    itemSource.Add(new SubjectHistory(Day.Date, percent));
                }

                LineSeries series = new LineSeries {
                    IndependentValuePath = "Date",
                    DependentValuePath = "Percent",
                    IsSelectionEnabled = true,
                    ItemsSource = itemSource,
                    Title = subjectName
                };


                foreach(var v in itemSource) {
                    Debug.WriteLine(subjectName);
                    Debug.WriteLine(v.Date + " " + v.Percent);
                    Debug.WriteLine("");
                }

                this.LineChart.Series.Add(series);
            } // end of foreach subject 
        } // end of this function
    }


    class SubjectHistory {

        public SubjectHistory(string date, int percent) {
            Date = date;
            Percent = percent;
        }

        public string Date { get; set; }
        public int Percent { get; set; }
    }
}
