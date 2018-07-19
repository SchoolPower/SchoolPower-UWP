using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

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
        public String Name { get; set; }
        public String TeacherName { get; set; }
        public String TeacherEmail { get; set; }
        public String BlockLetter { get; set; }
        public String RoomNumber { get; set; }
        public List<AssignmentItem> Assignments = new List<AssignmentItem>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Peroid> Peroids = new List<Peroid>();
        public String LetterGradeOnDashboard { get; set; }
        public String PercentageGradeOnDashboard { get; set; }
        public bool IsActive { get; set; }

        public Subject(dynamic data) {

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            Name         = data.name;
            TeacherName  = data.teacher.firstName + " " + data.teacher.lastName;
            TeacherEmail = data.teacher.email;
            BlockLetter  = data.expression;
            RoomNumber   = data.roomName;
            StartDate    = data.startDate;
            EndDate      = data.endDate;
            LetterGradeOnDashboard      = "--";
            PercentageGradeOnDashboard  = "--";

            IsActive = GetActivity(StartDate, EndDate);

            JArray assignmentsJarray = (JArray)data.assignments;
            try {
                for (int index = 0; index < assignmentsJarray.Count; index++) 
                    Assignments.Add(new AssignmentItem(assignmentsJarray[index]));
                
            } catch (System.NullReferenceException) { }

            String[] peroidList = { "T1", "T2", "X1", "S1", "T3", "T4", "X2", "S2", "Y1" };

            foreach (var time in peroidList) {
                try {
                    if (data.finalGrades[time].percent != null)
                        Peroids.Add(new Peroid(time, data.finalGrades[time]));
                } catch (Exception) { }
            }
            
            //activity
            if (true) {
            //if (IsActive) {

                // terms
                var time = new List<DateTime>();
                foreach (var p in Peroids) 
                    if (DateTime.Compare(p.Date, DateTime.Now) < 0 && (p.Time == "T1") || (p.Time == "T2") || (p.Time == "T3") || (p.Time == "T4")) 
                        time.Add(p.Date);
                
                time.Sort();
                time.Reverse();

                foreach (var p in Peroids) 
                    if (p.Date == time[0])
                        if ((p.Time == "T1") || (p.Time == "T2") || (p.Time == "T3") || (p.Time == "T4")) 
                            p.IsActive = true;
                

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
               
            }

            // sort
            Assignments.Sort((x, y) => DateTime.Compare(DateTime.Parse(x.Date), DateTime.Parse(y.Date)));
            Assignments.Reverse();

        }

        bool GetActivity (DateTime start, DateTime end) {
            if (DateTime.Compare(start, DateTime.Now) < 0 && DateTime.Compare(DateTime.Now, end) < 0) 
                return true;
            else 
                return false;
        }
    }

    public class Peroid {
        public String Time { get; set; }
        public String Percent { get; set; }
        public String Letter { get; set; }    
        public String Comment { get; set; }
        public String Eval { get; set; }
        public DateTime Date { get; set; }
        // public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public Peroid(String time, dynamic data) {
            Time = time;
            Percent = data.percent;
            Percent = Percent.Substring(0, Percent.IndexOf("."));
            Letter = data.letter;
            Eval = data.eval;
            IsActive = false;

            DateTime Genesis = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            Date = Genesis.AddSeconds((double)data.startDate).ToLocalTime();

            if (data.comment == null) 
                Comment = "--";
            else 
                Comment = data.comment;
            
        }
    }
}
