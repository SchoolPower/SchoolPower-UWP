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
        public static List<Subject> subjectsOld             = new List<Subject>();
        public static List<AttendanceItem> attendancesOld   = new List<AttendanceItem>();
        public static Info info;

        public StudentData(dynamic data, dynamic dataOld) {

            info = new Info(data.information);

            JArray sectionsJarray = (JArray)data["sections"];
            foreach (var section in sectionsJarray) {
                subjects.Add(new Subject(section));
            }
            JArray sectionsJarrayOld = (JArray)dataOld["sections"];
            foreach (var sectionOld in sectionsJarrayOld) {
                subjectsOld.Add(new Subject(sectionOld));
            }

            JArray attendancesJarray = (JArray)data["attendances"];
            foreach (var attendence in attendancesJarray) {
                attendances.Add(new AttendanceItem(attendence));
            }
            JArray attendancesJarrayOld = (JArray)dataOld["attendances"];
            foreach (var attendenceOld in attendancesJarrayOld) {
                attendancesOld.Add(new AttendanceItem(attendenceOld));
            }

            attendances.Sort((x, y) => DateTime.Compare(DateTime.Parse(x.Date), DateTime.Parse(y.Date)));
            attendances.Reverse();
            attendancesOld.Sort((x, y) => DateTime.Compare(DateTime.Parse(x.Date), DateTime.Parse(y.Date)));
            attendancesOld.Reverse();

            int index = 0;
            foreach (var subject in subjects) {
                foreach (var assignment in subject.Assignments) {
                    if (subjectsOld[index].Assignments.Contains(assignment)) {
                        assignment.IsNew = false;
                    }
                }
                index += 1;
            }
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

        public static async Task<string> GetJSONFromLocal(string NewOrOld) {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            switch (NewOrOld) {
                case "new": 
                    StorageFile file = await folder.GetFileAsync("studentInfo");
                    string ret = await FileIO.ReadTextAsync(file);
                    return ret;
                case "old":
                    StorageFile fileOld = await folder.GetFileAsync("studentInfoOld");
                    string retOld = await FileIO.ReadTextAsync(fileOld);
                    return retOld;
                default: 
                    return null;
            }
        }

        public static dynamic ParseJSON(string json) {
            dynamic ret = JObject.Parse(json);
            return ret;
        }

        public async static Task<StudentData> GetStudentDataFromLocal() {
            Task<string> task = GetJSONFromLocal("new");
            Task<string> taskOld = GetJSONFromLocal("old");
            string str = await task;
            string strOld = await task;
            dynamic data = JObject.Parse(str);
            dynamic dataOld = JObject.Parse(str);
            StudentData ret = new StudentData(data, dataOld);
            return ret;
        }

        public async static void SaveJSONtoLocal(string json, string NewOrOld) {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            switch (NewOrOld) {
                case "new":
                    StorageFile file = await folder.CreateFileAsync("studentInfo", CreationCollisionOption.ReplaceExisting);
                    StorageFile sampleFile = await folder.GetFileAsync("studentInfo");
                    file = await folder.GetFileAsync("studentInfo");
                    await FileIO.WriteTextAsync(file, json);
                    break;
                case "old":
                    StorageFile fileOld = await folder.CreateFileAsync("studentInfoOld", CreationCollisionOption.ReplaceExisting);
                    StorageFile sampleFileOld = await folder.GetFileAsync("studentInfoOld");
                    fileOld = await folder.GetFileAsync("studentInfoOld");
                    await FileIO.WriteTextAsync(fileOld, json);
                    break;
            }
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
            string studataOld = "";
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
                                
                // mv previous studata to old
                Task<string> getHistoryJSON = GetJSONFromLocal("new");
                studataOld = await getHistoryJSON;
                SaveJSONtoLocal(studataOld, "old");

                // save current studata to new
                SaveJSONtoLocal(studata, "new");

                // new StudentData
                StudentData studentData = new StudentData(StudentData.ParseJSON(studata), StudentData.ParseJSON(studataOld));

                await Task.Delay(200);
                Views.Busy.SetBusy(true, "Refreshing");
                await Task.Delay(100);
            }
            Views.Busy.SetBusy(false);
        }
        
        /*
        public static async void MarkNewAssignments() {

            Windows.Storage.ApplicationDataContainer setting = Windows.Storage.ApplicationData.Current.LocalSettings;
           
            try { if ((bool)setting.Values["IsFirstTimeLogin"]) { }
            } catch (System.NullReferenceException) {
                setting.Values["IsFirstTimeLogin"] = true;
            }

            // create history
            if ((bool)setting.Values["IsFirstTimeLogin"]) {
                setting.Values["IsFirstTimeLogin"] = false;
                Task<string> t = GetJSONFromLocal("new");
                string s = await t;
            } 
                // get history
                Task<string> task = GetJSONFromLocal("old");
                string studata = await task;
                dynamic data = JObject.Parse(studata);

                // get subject
                List<Subject> subjectsOld = new List<Subject>();
                JArray sectionsJarray = (JArray)data["sections"];
                foreach (var section in sectionsJarray) {
                    subjectsOld.Add(new Subject(section));
                }

                // compare
                int index = 0;
                foreach (var subject in subjects) {
                    index += 1;
                    foreach (var assignment in subject.Assignments) {
                        if (subjectsOld[index].Assignments.Contains(assignment)) {
                            assignment.IsNew = false;
                        }
                    }
                }

                // save file 
                //SaveStudentDataToLocal(studata, "old");
            

        } */
    }
}
