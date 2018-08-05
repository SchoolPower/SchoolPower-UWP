using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPower.Models {
    public class HistoryData {
        public string Date { get; set; }
        public List<HistoryDataItem> HistoryDataItems { get; set; }

        static Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public HistoryData(string date) {

            if (date != "") {

                // init
                HistoryDataItems = new List<HistoryDataItem>();

                var data = StudentData.ParseJSON((string)localSettings.Values[date]);
                Date = data.date;
                JArray historyDataArray = (JArray)data["subjects"];

                foreach (dynamic historyDataItem in historyDataArray) {
                     
                    string subject = historyDataItem.name;
                    List<PeroidItem> peroidItems = new List<PeroidItem>();

                    JArray peroidArray = (JArray)historyDataItem["peroids"];
                    foreach (dynamic peroidItem in peroidArray) {
                        peroidItems.Add(new PeroidItem((string)peroidItem.time, (int)peroidItem.percent));
                    }

                    HistoryDataItems.Add(new HistoryDataItem(subject, peroidItems));
                }
            }
        }

        public static string GetHistoryData(string date) {
            return (string)localSettings.Values[date];
        }
    }

    public class HistoryDataItem {
        public string Subject { get; set; }
        public List<PeroidItem> Peroids { get; set; }

        public HistoryDataItem(string subject, List<PeroidItem> peroids) {
            Subject = subject;
            Peroids = peroids;
        }
    }

    public class PeroidItem {
        public string Peroid { get; set; }
        public int Percent { get; set; }

        public PeroidItem(string d, int p) {
            Peroid = d;
            Percent = p;
        }
    }
}
