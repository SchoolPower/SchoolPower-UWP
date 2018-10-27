using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

/*
    Sample:
    {
        "assignments": [
            (AssignmentItem)...
        ],
        "expression": "1(A-E)",
        "startDate": "2017-08-31T16:00:00.000Z",
        "endDate": "2018-01-21T16:00:00.000Z",
        "finalGrades": {
            "X1": {
                "percent": "0.0",
                "letter": "--",
                "comment": null,
                "eval": "--",
                "startDate": 1515945600,
                "endDate": 1515945600
            },
            "T2": {
                "percent": "92.0",
                "letter": "A",
                "comment": "Some comments",
                "eval": "M",
                "startDate": 1510502400,
                "endDate": 1510502400
            },
            "T1": {
                "percent": "90.0",
                "letter": "A",
                "comment": "Some comments",
                "eval": "M",
                "startDate": 1504195200,
                "endDate": 1504195200
            },
            "S1": {
                "percent": "91.0",
                "letter": "A",
                "comment": null,
                "eval": "M",
                "startDate": 1504195200,
                "endDate": 1504195200
            }
        },
        "name": "Course Name",
        "roomName": "100",
        "teacher": {
            "firstName": "John",
            "lastName": "Doe",
            "email": null,
            "schoolPhone": null
        }
    }
 */

namespace SchoolPower.Models {

    public class Subject {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string TeacherName { get; set; }
        public string TeacherEmail { get; set; }
        public string BlockLetter { get; set; }
        public string RoomNumber { get; set; }
        public List<AssignmentItem> Assignments = new List<AssignmentItem>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Peroid> Peroids = new List<Peroid>();
        public List<Catagory> CatagoryList { get; set; }
        public string LetterGradeOnDashboard { get; set; }
        public string PercentageGradeOnDashboard { get; set; }
        public SolidColorBrush ColorOnDashboard { get; set; }
        public bool IsActive { get; set; }
        public bool IsNew { get; set; }
        public FontWeight LargeTextFontWeight { get; set; }
        public FontWeight SmallTextFontWeight { get; set; }
        public string ChangeInGrade { get; set; }
        public Visibility GradeChangePanelVisibility { get; set; }
        public SolidColorBrush GradeChangePanelColor { get; set; }

        public Subject(JObject data) {

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            Name         = data["name"].ToString();
            DisplayName  = data["name"].ToString();
            TeacherEmail = data["teacher"]["email"].ToString();
            TeacherName  = data["teacher"]["firstName"].ToString() + " " + data["teacher"]["lastName"].ToString();
            BlockLetter  = data["expression"].ToString();
            RoomNumber   = data["roomName"].ToString();
            StartDate    = (DateTime)data["startDate"];
            EndDate      = (DateTime)data["endDate"];
            IsNew        = false;
            LetterGradeOnDashboard = "--";
            PercentageGradeOnDashboard  = "--";
            LargeTextFontWeight = FontWeights.SemiLight;
            SmallTextFontWeight = FontWeights.Normal;
            IsActive = GetActivity(StartDate, EndDate);
            ChangeInGrade = "0";
            GradeChangePanelVisibility = Visibility.Collapsed;
            GradeChangePanelColor = new SolidColorBrush(Windows.UI.Colors.Transparent);

            JArray assignmentsJarray = new JArray();
            try {
                assignmentsJarray = (JArray)data["assignments"];
            } catch (Exception) { }
            try {
                for (int index = 0; index < assignmentsJarray.Count; index++) 
                    Assignments.Add(new AssignmentItem((JObject)assignmentsJarray[index]));
            } catch (System.NullReferenceException) { }


            // peroid
            string[] peroidList = { "T1", "T2", "X1", "S1", "T3", "T4", "X2", "S2", "Y1" };

            foreach (var peroid in peroidList) {
                try {
                    if (data["finalGrades"][peroid]["percent"] != null)
                        Peroids.Add(new Peroid(peroid, (JObject)data["finalGrades"][peroid]));
                } catch (Exception) { }
            }

            // active
            // terms
            var time = new List<DateTime>();
            foreach (var p in Peroids) {
                if (DateTime.Compare(p.Date, DateTime.Now) < 0 && (p.Time == "T1") || (p.Time == "T2") || (p.Time == "T3") || (p.Time == "T4"))
                    time.Add(p.Date);
            }
            time.OrderBy(i => i);

            foreach (var p in Peroids)
                try {
                    if (p.Date == time[0])
                       if ((p.Time == "T1") || (p.Time == "T2") || (p.Time == "T3") || (p.Time == "T4"))
                            p.IsActive = true;
                }
                catch (Exception) {
                    p.IsActive = true;
                }

            //semasters
            time = null;
            time = new List<DateTime>();
            foreach (var p in Peroids)
                if (DateTime.Compare(p.Date, DateTime.Now) < 0 && (p.Time == "S1") || (p.Time == "S2"))
                    time.Add(p.Date);
            time.Sort();
            time.Reverse();
            foreach (var p in Peroids)
                if ((p.Time == "S1") || (p.Time == "S2"))
                    if (p.Date == time[0])
                        p.IsActive = true;


            // sort
            try {
                Assignments.Sort((x, y) => DateTime.Compare(DateTime.Parse(x.Date), DateTime.Parse(y.Date)));
            } catch (Exception) { } /*
            try {
                Assignments.Sort((x, y) => DateTime.Compare
                    (DateTime.ParseExact(x.Date, "MM/dd/yyyy", null),
                     DateTime.ParseExact(x.Date, "MM/dd/yyyy", null)));
            } catch (System.FormatException) { } */
            Assignments.Reverse();


            // catagory
            CatagoryList = new List<Catagory>();

            var catagoryNameList = new List<string>();

            foreach (var ass in Assignments) {
                if (!catagoryNameList.Contains(ass.Category))
                    catagoryNameList.Add(ass.Category);
            }

            // get cataory weight
            Dictionary<string, double> weights = new Dictionary<string, double> { };
            try {
                string json = (string)localSettings.Values[Name + "CataWeight"];
                weights = JsonConvert.DeserializeObject<Dictionary<string, double>>(json);
            } catch (Exception) {
                foreach (var cata in catagoryNameList) {
                    weights.Add(cata, 0);
                }
            }

            foreach (var cataName in catagoryNameList) {
                var catagortAssignmentList = new List<AssignmentItem>();
                foreach (var ass in Assignments) {
                    if (ass.Category == cataName) {
                        catagortAssignmentList.Add(ass);
                    }
                }
                double w = 0;
                try {
                    w = weights[cataName];
                } catch (Exception) {
                    w = 0;
                }
                var v = weights[cataName];
                CatagoryList.Add(new Catagory(cataName, catagortAssignmentList, w));
            }
        }

        bool GetActivity (DateTime start, DateTime end) {
            if (DateTime.Compare(start, DateTime.Now) < 0 && DateTime.Compare(DateTime.Now, end) < 0) 
                return true;
            else 
                return false;
        }
    }

    public class Peroid {
        public string Time { get; set; }
        public string Percent { get; set; }
        public string LetterGrade { get; set; }    
        public string Comment { get; set; }
        public string Eval { get; set; }
        public DateTime Date { get; set; }
        public SolidColorBrush Color { get; set; }
        public bool IsActive { get; set; }

        public Peroid(string time, JObject data) {
            Time            = time;
            Percent         = data["percent"].ToString();
            Percent         = Percent.Substring(0, Percent.IndexOf("."));
            LetterGrade     = data["letter"].ToString();
            Eval            = data["eval"].ToString();
            IsActive        = false;
            Color           = StudentData.GetColor(LetterGrade);
            List <AssignmentItem> assignments = new List<AssignmentItem>();

            DateTime Genesis = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            Date = Genesis.AddSeconds((double)data["startDate"]).ToLocalTime();

            if (data["comment"].ToString() == null) 
                Comment = "--";
            else 
                Comment = data["comment"].ToString();
        }
    }

    public class Catagory {
        public string Name { get; set; }
        public double Weight { get; set; }
        public string WeightDisplay { get; set; }
        public string LetterGrade { get; set; }
        public double Percentage { get; set; }
        public string PercentageDisplay { get; set; }
        public SolidColorBrush Color { get; set; }
        public List<AssignmentItem> Assignments { get; set; }

        public Catagory(string Name, List<AssignmentItem> Assignments, double weight) {
            this.Name = Name;
            this.WeightDisplay = "Weight: " + Weight.ToString();
            this.Assignments = Assignments;
            this.Color = new SolidColorBrush(Windows.UI.Color.FromArgb(200, 30, 30, 30));
            double neuromotor = 0;
            double denominator = 0;

            foreach (var ass in this.Assignments) {
                double www = 1;
                try { www = Convert.ToDouble(ass.Weight); } catch (Exception) { www = 1; }

                try { neuromotor += Convert.ToDouble(ass.Score) * www; } catch (Exception) { }
                try { denominator += Convert.ToDouble(ass.MaximumScore) * www; } catch (Exception) { }

            }

            Percentage = neuromotor / denominator;
            PercentageDisplay = "Percentage: " + (Math.Round(Percentage, 3)).ToString();


        }
    }
}
