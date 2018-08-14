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

    public sealed partial class ChartPage: Page {

        public ChartPage() {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) => { };

            showGradeOfTerm = (bool)localSettings.Values["DashboardShowGradeOfTERM"];

            InitLineChartContent();
            InitColumnChartContent();
        }

        private Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private bool showGradeOfTerm;

        void InitLineChartContent() {

            foreach (var subject in StudentData.subjects) {

                List<LineData> itemSource = new List<LineData>();
                string subjectName = subject.Name;

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
                    itemSource.Add(new LineData(Day.Date, percent));
                }

                LineSeries series = new LineSeries {
                    IndependentValuePath = "Date",
                    DependentValuePath = "Percent",
                    IsSelectionEnabled = true,
                    ItemsSource = itemSource,
                    Title = subjectName
                };

                this.LineChart.Series.Add(series);
            } // end of foreach subject 
            /*
            if (LineChart.Series.Count >= 1) 
                ((LineSeries)LineChart.Series[0]).DependentRangeAxis = new LinearAxis() {
                    Maximum = 100,
                    Minimum = 0,
                    Orientation = AxisOrientation.Y,
                    Interval = 10,
                    ShowGridLines = true,
                };
                */
        } // end of this function

        void InitColumnChartContent() {

            List<ColumnData> itemSource = new List<ColumnData>();

            // today only
            var todayData = StudentData.historyDatas[StudentData.historyDatas.Count - 1];

            foreach (var data in todayData.SubjectHistoryData) {
                int percent = 0;
                foreach (var peroid in data.Peroids) {
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
                itemSource.Add(new ColumnData(data.Subject, percent));
            }

            (this.ColumnChart.Series[0] as ColumnSeries).ItemsSource = itemSource;
        }
    }



    class LineData {

        public LineData(string date, int percent) {
            Date = date;
            Percent = percent;
        }

        public string Date { get; set; }
        public int Percent { get; set; }
    }

    class ColumnData {

        public ColumnData(string subject, int percent) {
            Subject = subject;
            Percent = percent;
        }

        public string Subject { get; set; }
        public int Percent { get; set; }
    }
}
