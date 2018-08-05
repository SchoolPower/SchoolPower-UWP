using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
    
namespace SchoolPower.Models {
    public class StudentData {

        static Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        internal const string APIURL = "http://127.0.0.1:8000/";
        // internal const string APIURL = "https://schoolpower.harrynull.tech:8443/api/2.0/get_data.php";
        // internal const string APIURL = "https://api.schoolpower.tech/api/2.0/get_data.php";

        public enum NewOrOld { New, Old };

        public static List<Subject> subjects                = new List<Subject>();
        public static List<AttendanceItem> attendances      = new List<AttendanceItem>();
        public static List<HistoryData> historyDatas        = new List<HistoryData>();
        public static Info info;
        public static String SelectedSubjectName;
        public static IList<object> SubjectListViewRemovedItems;
        public static IList<object> SubjectListViewAddedItems;

        public StudentData(dynamic data, dynamic dataOld) {

            List<Subject> subjectsOld = new List<Subject>();
            List<AttendanceItem> attendancesOld = new List<AttendanceItem>();

            info = new Info(data.information);

            // parse
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

            // sort
            attendances.Sort((x, y) => DateTime.Compare(DateTime.Parse(x.Date), DateTime.Parse(y.Date)));
            attendances.Reverse();
            attendancesOld.Sort((x, y) => DateTime.Compare(DateTime.Parse(x.Date), DateTime.Parse(y.Date)));
            attendancesOld.Reverse();

            // new assignments 
            foreach (var subject in subjects) {
                foreach (var subjectOld in subjectsOld) {
                    if (subjectOld.Name == subject.Name) {
                        foreach (var assignment in subject.Assignments) {
                            try {
                                if (!subjectOld.Assignments.Contains(assignment)) {
                                    subject.IsNew = true;
                                    assignment.IsNew = true;
                                    assignment.DisplayName += " *";
                                    assignment.LargeTextFontWeight = FontWeights.SemiBold;
                                    assignment.SmallTextFontWeight = FontWeights.Bold;
                                }
                            } catch (System.ArgumentOutOfRangeException) { }
                        }

                    }
                }
                if (subject.IsNew) {
                    subject.DisplayName += " *";
                    subject.LargeTextFontWeight = FontWeights.SemiBold;
                    subject.SmallTextFontWeight = FontWeights.Bold;
                }
            }

            // new attendance
            foreach (var attendance in attendances) {
                try { 
                    if (!attendancesOld.Contains(attendance)){
                        attendance.SmallTextFontWeight = FontWeights.SemiBold;
                        attendance.DisplayName += " *";
                    }
                } catch (System.ArgumentOutOfRangeException) {}
            }


            // get a list of dates that has history
            string[] dateArray = ((string)localSettings.Values["dates"]).Split(' ');

            // add every history to List<HistoryData>
            foreach (var date in dateArray) {
                historyDatas.Add(new HistoryData(date));
            }

            // remove null data
            if (historyDatas[0].Date == null && historyDatas[0].HistoryDataItems == null)
                historyDatas.RemoveAt(0);
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

        public static async Task<string> GetStudentData(NewOrOld NoO) {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            switch (NoO) {
                case NewOrOld.New: 
                    StorageFile file = await folder.GetFileAsync("studentInfo");
                    string ret = await FileIO.ReadTextAsync(file);
                    return ret;
                case NewOrOld.Old:
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

        public async static Task SaveStudentData(string json, NewOrOld NoO) {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            switch (NoO) {
                case NewOrOld.New:
                    StorageFile file = await folder.CreateFileAsync("studentInfo", CreationCollisionOption.ReplaceExisting);
                    StorageFile sampleFile = await folder.GetFileAsync("studentInfo");
                    file = await folder.GetFileAsync("studentInfo");
                    await FileIO.WriteTextAsync(file, json);
                    break;
                case NewOrOld.Old:
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
                foreach (var grade in subject.Peroids) {
                    if (grade.Time == SelectedPeroid && grade.Percent != "0") {
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
                foreach (var grade in subject.Peroids) {

                    System.Diagnostics.Debug.WriteLine(grade.Time + " " + grade.Percent + " "+ localSettings.Values[subject.Name]);

                    if (grade.Time == SelectedPeroid && grade.Percent != "0" && (bool)localSettings.Values[subject.Name]) {
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
                foreach (var grade in subject.Peroids) {
                    if (grade.Time != SelectedPeroid && grade.Percent != "0" && (bool)localSettings.Values[subject.Name]) {
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

        public static async Task<String> Refresh() {

            string studata = "";
            string studataOld = "";

            // get account info
            ApplicationDataContainer account = ApplicationData.Current.LocalSettings;
            string username = (string)account.Values["UsrName"];
            string password = (string)account.Values["Passwd"];

            // kissing
            try { studata = await Kissing(username, password); } catch (Exception) { }

            // bad network or server error 
            if (studata == "") {
                return "error";
            }

            // save studata
            else {
                
                // move previous studata to old
                studataOld = await GetStudentData(NewOrOld.New);
                await SaveStudentData(studataOld, NewOrOld.Old);

                // save current studata to new
                await SaveStudentData(studata, NewOrOld.New);

                // clear history
                for (int i = subjects.Count; i >= 1; i--) {
                    subjects.RemoveAt(i - 1);
                }
                for (int j = attendances.Count; j >= 1; j--) {
                    attendances.RemoveAt(j - 1);
                }

                // new StudentData
                StudentData studentData = new StudentData(StudentData.ParseJSON(studata), StudentData.ParseJSON(studataOld));
                SaveHistoryData(CollectCurrentHistoryData());
                string s = GetHistoryData(DateTime.Now.ToString("yyyy-MM-dd"));
                Debug.WriteLine(s);
                return "okey dokey";
            }
        }

        public static SolidColorBrush GetColor(string letterGrade) {
            switch (letterGrade) {
                case "A":
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(200, 0, 121, 107));
                case "B":
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(200, 56, 124, 60));
                case "C+":
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(200, 255, 179, 0));
                case "C":
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(200, 255, 87, 34));
                case "C-":
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(200, 211, 47, 47));

                case "E":
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(200, 0, 121, 107));
                case "2":
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(200, 56, 124, 60));
                case "3":
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(200, 255, 179, 0));
                case "H":
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(200, 255, 87, 34));
                case "I":
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(200, 211, 47, 47));
                default:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(200, 30, 30, 30));
            }
        }

        public static string CollectCurrentHistoryData() {

            dynamic json = new JObject();
            json.date = DateTime.Now.ToString("yyyy-MM-dd");

            JArray subjectItemArray = new JArray();

            foreach (var subject in subjects) {

                dynamic subjectItem = new JObject();
                JArray peroidInfoArray = new JArray();
                subjectItem.name = subject.Name;
                subjectItem.peroids = peroidInfoArray;

                foreach (var peroid in subject.Peroids) {

                    if (peroid.IsActive) {

                        dynamic peroidInfo = new JObject();
                        peroidInfo.time = peroid.Time;
                        peroidInfo.percent = peroid.Percent;
                        peroidInfoArray.Add(peroidInfo);

                    }
                }
                subjectItemArray.Add(subjectItem);
            }
            json.subjects = subjectItemArray;
            return json.ToString();
        }

        public static void SaveHistoryData(string json) {

            // get today date
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            // save json
            localSettings.Values[currentDate] = json;

            // add date to localSettings.dates
            // get current datelist
            var date = (string)localSettings.Values["dates"];
            // datalist -> []
            string[] dateArray = null;
            dateArray = date.Split(' ');
            // add today, avoid duplication
            if (dateArray[dateArray.Length - 1] != currentDate) {
                date += " ";
                date += currentDate;
                localSettings.Values["dates"] = date;
            }
        }

        public static string GetHistoryData(string date) {
            return (string)localSettings.Values[date];
        }
    }
}
