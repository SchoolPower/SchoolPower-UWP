using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SchoolPower.Models {
    public class StudentData {

        //internal const string APIURL = "https://schoolpower.harrynull.tech:8443/api/2.0/get_data.php";
        internal const string APIURL = "https://api.schoolpower.tech/api/2.0/get_data.php";

        public static List<Subject> subjects                = new List<Subject>();
        public static List<AttendanceItem> attendances      = new List<AttendanceItem>();
        public static Info info;

        public StudentData(dynamic data) {
            JArray sectionsJarray = (JArray)data["sections"];
            for (int index = 0; index < sectionsJarray.Count; index++) {
                subjects.Add(new Subject(sectionsJarray[index]));
            }
            JArray attendancesJarray = (JArray)data["attendances"];
            for (int index = 0; index < attendancesJarray.Count; index++) {
                attendances.Add(new AttendanceItem(attendancesJarray[index]));
            }
            info = new Info(data.information);
        }

        public static async Task<string> Kissing(string username, string password) {
            HttpClient client = new HttpClient();
            var content = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                    { "username", username }, { "password", password }
                }
            );
            var response = await client.PostAsync(APIURL, content);
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<string> GetJSONFromLocal() {
            Windows.Storage.StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile file = await folder.GetFileAsync("studentInfo");
            string ret = await Windows.Storage.FileIO.ReadTextAsync(file);
            return ret;
        }

        public async static Task<StudentData> GetStudentDataFromServer(string username, string password) {
            Task<string> task = Kissing(username, password);
            string str = await task;
            dynamic data = JObject.Parse(str);
            StudentData ret = new StudentData(data);
            return ret;
        }

        public async static void SaveStudentDataToLocal(string json) {
            Windows.Storage.StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile file = await folder.CreateFileAsync("studentInfo", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            Windows.Storage.StorageFile sampleFile = await folder.GetFileAsync("studentInfo");
            file = await folder.GetFileAsync("studentInfo");
            await Windows.Storage.FileIO.WriteTextAsync(file, json);
        }

        public async static Task<StudentData> GetStudentDataFromLocal() {
            Task<string> task = GetJSONFromLocal();
            string str = await task;
            dynamic data = JObject.Parse(str);
            StudentData ret = new StudentData(data);
            return ret;
        }

        public static implicit operator StudentData(Task<StudentData> v) {
            throw new NotImplementedException();
        }

        public static double GetAllGPA(String SelectedPeroid) {   

            double index = 0; 
            double gradeSum = 0;

            foreach (var subject in subjects) {
                foreach (var grade in subject.Grades) {
                    if (grade.Time.Equals(SelectedPeroid) && !grade.Percent.Equals("0.0")) {
                        gradeSum += Convert.ToDouble(grade.Percent);
                        index += 1;
                    }
                }
            }

            switch (index) {
                case 0: return 0;
                default: return Math.Round(gradeSum / index, 3);
            }
        }

        public static double GetSomeGPA(String SelectedPeroid) {

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            double index = 0;
            double gradeSum = 0;

            foreach (var subject in subjects) {
                foreach (var grade in subject.Grades) {
                    if (grade.Time.Equals(SelectedPeroid) && !grade.Percent.Equals("0.0") && (bool)localSettings.Values[subject.Name]) {
                        gradeSum += Convert.ToDouble(grade.Percent);
                        index += 1;
                    }
                }
            }
            switch (index) {
                case 0: return 0;
                default: return Math.Round(gradeSum / index, 3);
            }
        }

        public static double GetSomeGPA(String SelectedPeroid, int total) {

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            List<double> grades = new List<double>();

            foreach (var subject in StudentData.subjects) {
                foreach (var grade in subject.Grades) {
                    if (grade.Time.Equals(SelectedPeroid) && !grade.Percent.Equals("0.0") && (bool)localSettings.Values[subject.Name]) {
                        grades.Add(Convert.ToDouble(grade.Percent));
                    }
                }
            }

            //rank
            int i, j, indexOfMax;
            double temp;
            for (i = 0; i < grades.Count -1 ; i++) {
                indexOfMax = i;
                for (j = i + 1; j < grades.Count; j++) {
                    if (grades[indexOfMax] < grades[j]) {
                        indexOfMax = j;
                    }
                }
                temp = grades[i];
                grades[i] = grades[indexOfMax];
                grades[indexOfMax] = temp;
            }

            double sum = 0;
            for (int k = 0; k < total; k++) {
                sum += grades[k];
            }

            foreach (double d in grades) {
                if (d == 0) {
                    total--;
                }
            }
            return sum/total;
        }

        public static async void Refresh() {
            Views.Busy.SetBusy(true, "Loading");
            await Task.Delay(100);
            Views.Busy.SetBusy(true, "Setting App.isMainPageFirstTimeInit value");
            App.isMainPageFirstTimeInit = true;
            await Task.Delay(100);
            Views.Busy.SetBusy(true, "Getting username and assword");
            Windows.Storage.ApplicationDataContainer account = Windows.Storage.ApplicationData.Current.LocalSettings;
            String username = (String)account.Values["UsrName"];
            String password = (String)account.Values["Passwd"];
            await Task.Delay(100);
            Views.Busy.SetBusy(true, "Kissing");
            Task<string> task = StudentData.Kissing(username, password);
            string studata = await task;
            Views.Busy.SetBusy(true, "Saving data");
            StudentData.SaveStudentDataToLocal(studata);
            await Task.Delay(200);
            Views.Busy.SetBusy(true, "Refreshing");
            await Task.Delay(100);
            Views.Busy.SetBusy(false);
        }
    }
}
