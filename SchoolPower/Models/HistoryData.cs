using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SchoolPower.Models {
    public class HistoryData {
        public string Date { get; set; }
        public List<SubjectHistoryData> SubjectHistoryData { get; set; }

        static Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public HistoryData(string date) {

            if (date != "") {

                // init
                SubjectHistoryData = new List<SubjectHistoryData>();

                var data = StudentData.ParseJSON((string)localSettings.Values[date]);
                Date = data["date"].ToString();
                JArray historyDataArray = (JArray)data["subjects"];

                foreach (JObject historyDataItem in historyDataArray) {
                     
                    string subject = historyDataItem["name"].ToString();
                    List<PeroidItem> peroidItems = new List<PeroidItem>();

                    JArray peroidArray = (JArray)historyDataItem["peroids"];
                    foreach (JObject peroidItem in peroidArray) {
                        peroidItems.Add(new PeroidItem((string)peroidItem["time"], (int)peroidItem["percent"]));
                    }

                    SubjectHistoryData.Add(new SubjectHistoryData(subject, peroidItems));
                }
            }
        }

        public static string GetHistoryData(string date) {
            return (string)localSettings.Values[date];
        }
    }

    public class SubjectHistoryData {
        public string Subject { get; set; }
        public List<PeroidItem> Peroids { get; set; }

        public SubjectHistoryData(string subject, List<PeroidItem> peroids) {
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
