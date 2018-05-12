using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Storage;

namespace SchoolPower.Models {
    public class StudentData {

        //internal const string APIURL = "https://schoolpower.harrynull.tech:8443/api/2.0/get_data.php";
        internal const string APIURL = "https://api.schoolpower.tech/api/2.0/get_data.php";

        public static List<Subject> subjects                = new List<Subject>();
        public static List<AttendanceItem> attendances      = new List<AttendanceItem>();
        public static Info info;

        public StudentData(dynamic data) {
            JArray sectionsJarray = (JArray)data["sections"];
            foreach (var section in sectionsJarray) {
                subjects.Add(new Subject(section));
            }
            JArray attendancesJarray = (JArray)data["attendances"];
            foreach (var attendence in attendancesJarray) {
                attendances.Add(new AttendanceItem(attendence));
            }
            attendances.Sort((x, y) => DateTime.Compare(DateTime.Parse(x.Date), DateTime.Parse(y.Date)));
            attendances.Reverse();
            info = new Info(data.information);
            MarkNewAssignments();
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

        private static async Task<string> GetJSONFromLocal(String NewOrOld) {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            switch (NewOrOld) {
                case "new": 
                    StorageFile file = await folder.GetFileAsync("studentInfo");
                    string ret = await FileIO.ReadTextAsync(file);
                    return ret;
                case "old":
                    StorageFile fileOld = await folder.GetFileAsync("studentInfo-old");
                    string retOld = await FileIO.ReadTextAsync(fileOld);
                    return retOld;
                default: 
                    return null;
            }
        }

        public async static Task<StudentData> GetStudentDataFromServer(string username, string password) {
            Task<string> task = Kissing(username, password);
            string str = await task;
            dynamic data = JObject.Parse(str);
            StudentData ret = new StudentData(data);
            return ret;
        }

        public async static void SaveStudentDataToLocal(string json) {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("studentInfo", CreationCollisionOption.ReplaceExisting);
            StorageFile sampleFile = await folder.GetFileAsync("studentInfo");
            file = await folder.GetFileAsync("studentInfo");
            await FileIO.WriteTextAsync(file, json);
        }

        public async static Task<StudentData> GetStudentDataFromLocal() {
            Task<string> task = GetJSONFromLocal("new");
            string str = await task;
            dynamic data = JObject.Parse(str);
            StudentData ret = new StudentData(data);
            return ret;
        }

        public static double GetAllGPA(string SelectedPeroid) {   

            double index = 0; 
            double gradeSum = 0;

            foreach (var subject in subjects) {
                foreach (var grade in subject.Grades) {
                    if (grade.Time.Equals(SelectedPeroid) && !grade.Percent.Equals("0")) {
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
        
        public static double GetSelectedGPA(string SelectedPeroid) {

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            double index = 0;
            double gradeSum = 0;

            System.Diagnostics.Debug.WriteLine(SelectedPeroid);

            foreach (var subject in subjects) {
                System.Diagnostics.Debug.WriteLine(subject.Name);
                foreach (var grade in subject.Grades) {

                    System.Diagnostics.Debug.WriteLine(grade.Time + " " + grade.Percent + " "+ localSettings.Values[subject.Name]);

                    if (grade.Time.Equals(SelectedPeroid) && !grade.Percent.Equals("0") && (bool)localSettings.Values[subject.Name]) {
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
        
        public static double GetSelectedGPA(string SelectedPeroid, int total) {

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            List<double> grades = new List<double>();

            foreach (var subject in StudentData.subjects) {
                foreach (var grade in subject.Grades) {
                    if (grade.Time.Equals(SelectedPeroid) && !grade.Percent.Equals("0") && (bool)localSettings.Values[subject.Name]) {
                        grades.Add(Convert.ToDouble(grade.Percent));
                    }
                }
            }
            grades.Sort();
            grades.Reverse();

            double sum = 0;
            if (grades.Count < total) {
                foreach (double grade in grades) {
                    sum += grade;
                }
                total = grades.Count;
            } 
            else {
                for (int i = 0; i < total; i++) {
                    sum += grades[i];
                }
            }
            return Math.Round(sum / total, 3);
        }

        public static async void Refresh() {

            Views.Busy.SetBusy(true, "Loading");
            App.isMainPageFirstTimeInit = true;
            await Task.Delay(100);

            // get account info
            Views.Busy.SetBusy(true, "Getting username and assword");
            ApplicationDataContainer account = ApplicationData.Current.LocalSettings;
            String username = (string)account.Values["UsrName"];
            string password = (string)account.Values["Passwd"];
            await Task.Delay(100);

            // kissing
            Views.Busy.SetBusy(true, "Kissing");
            Task<string> task = Kissing(username, password);
            string studata = "";
            try { studata = await task; } catch (Exception) { }

            // bad network or server
            if (studata.Equals("")) {
                ContentDialog ErrorContentDialog = new ContentDialog {
                    Title = "ERROR",
                    Content = "Network error, grades will not be updates. Please refresh later. ",
                    CloseButtonText = "哦。",
                }; ContentDialogResult result = await ErrorContentDialog.ShowAsync();
            }
            // save studata
            else {
            SaveStudentDataToLocal(studata);
                await Task.Delay(200);
                Views.Busy.SetBusy(true, "Refreshing");
                await Task.Delay(100);
            }

            Views.Busy.SetBusy(false);
        }

        public async void MarkNewAssignments() {

            // get dynamic data
            Task<string> task = GetJSONFromLocal("new");
            string str = await task;
            dynamic data = JObject.Parse(str);

            // get subject
            List<Subject> subjectsOld = new List<Subject>();
            JArray sectionsJarray = (JArray)data["sections"];
            foreach (var section in sectionsJarray) {
                subjectsOld.Add(new Subject(section));
            }

            // 
            int index = 0;
            foreach (var subject in subjects) {
                index += 1;

                // compare  subject.Assignments  and  subjectsOld[index].Assignments
                // todo
            }

        }
    }
}
