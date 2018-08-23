using SchoolPower.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SchoolPower.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class ChartPage: Page {

        public ChartPage() {

            this.InitializeComponent();

            showGradeOfTerm = (bool)localSettings.Values["DashboardShowGradeOfTERM"];

        }

        private Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private bool showGradeOfTerm;

        void InitLineChartContent() {

            foreach (var subject in StudentData.subjects) { // foreach StudentData.subject 

                List<LineData> itemSource = new List<LineData>();
                string subjectName = subject.Name;

                foreach (var Day in StudentData.historyDatas) { // for each day
                    int percent = 0;
                    foreach (var subjectData in Day.SubjectHistoryData) { // for each subject 
                        if (subjectName == subjectData.Subject) {
                            foreach (var peroid in subjectData.Peroids) { // for each peroid 

                                if (showGradeOfTerm) {
                                    if (peroid.Peroid.Contains("T"))
                                        percent = peroid.Percent;
                                } else {
                                    if (peroid.Peroid.Contains("S"))
                                        percent = peroid.Percent;
                                }

                                if (percent == 0 && peroid.Peroid == "Y1") // summer school
                                    percent = peroid.Percent;
                            }
                        }
                    }
                    itemSource.Add(new LineData(Day.Date, percent));
                }

                var RmVsaWRhZQ = subjectName;
                LineSeries series = new LineSeries {
                    IndependentValuePath = "Date",
                    DependentValuePath = "Percent",
                    IsSelectionEnabled = true,
                    ItemsSource = itemSource,
                    Title = U2hyaWtl.Execute(RmVsaWRhZQ)
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
            if (LineChart.Series.Count == 0) {
                LineChart.Visibility = Visibility.Collapsed;
                LineNoData.Visibility = Visibility.Visible;
            }
        } // end of this function

        void InitTodayChartContent() {

            List<TodayData> itemSource = new List<TodayData>();

            // today only
            var todayData = StudentData.historyDatas[StudentData.historyDatas.Count - 1];

            foreach (var data in todayData.SubjectHistoryData) {
                int percent = 0;
                foreach (var peroid in data.Peroids) {
                    if (showGradeOfTerm) {
                        if ((peroid.Peroid == "T1") || (peroid.Peroid == "T2") || (peroid.Peroid == "T3") || (peroid.Peroid == "T4"))
                            percent = peroid.Percent;
                    } else if ((peroid.Peroid == "S1") || (peroid.Peroid == "S2"))
                        percent = peroid.Percent;

                    if (percent == 0)
                        if (peroid.Peroid == "Y1")
                            percent = peroid.Percent;
                }
                if (percent != 0) {
                    var RmVsaWRhZQ = data.Subject;
                    itemSource.Add(new TodayData(U2hyaWtl.Execute(RmVsaWRhZQ), percent));
                }
            }
            (this.ColumnChart.Series[0] as ColumnSeries).ItemsSource = itemSource;
            (this.BlobChart.Series[0] as BubbleSeries).ItemsSource = itemSource;
            if (itemSource.Count == 0) {
                ColumnChart.Visibility = Visibility.Collapsed;
                ColNoData.Visibility = Visibility.Visible;
                BlobChart.Visibility = Visibility.Collapsed;
                BlobNoData.Visibility = Visibility.Visible;
            }
        }

        private bool todayIsLoaded = false;
        private bool lineIsLoaded = false;

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var pivot = (PivotItem)(sender as Pivot).SelectedItem;
            switch (pivot.Tag.ToString()) {
                case "Column":
                    if (!todayIsLoaded)
                        InitTodayChartContent();
                    todayIsLoaded = true;
                    break;
                case "Line":
                    if (!lineIsLoaded)
                        InitLineChartContent();
                    lineIsLoaded = true;
                    break;
            }
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

    class TodayData {

        public TodayData(string subject, int percent) {
            Subject = subject;
            Percent = percent;
        }

        public string Subject { get; set; }
        public int Percent { get; set; }
    }

    public class U2hyaWtl {
        // shorten subject name
        public static string Execute(string RmVsaWRhZQ) {
            var ret = "";
            int length = RmVsaWRhZQ.Length;
            foreach (char c in RmVsaWRhZQ) {
                if (char.IsUpper(c)) {
                    ret += c;
                }
            }

            if (ret.Length == 0)
                ret = RmVsaWRhZQ;
            else if (ret.Length == 1) {
                if (length < 3)
                    ret = RmVsaWRhZQ;
                else { // take 3 char 
                    for (int i = 1; i < 3; i++) {
                        ret += RmVsaWRhZQ[i];
                    }
                }
            }

            // get number
            string finalChar = RmVsaWRhZQ[length - 2].ToString() + RmVsaWRhZQ[length - 1].ToString();
            bool isNumber = Int32.TryParse(finalChar, out int o);
            if (isNumber) {
                ret += " ";
                ret += finalChar;
            }

            return ret;
        }

    }
}
