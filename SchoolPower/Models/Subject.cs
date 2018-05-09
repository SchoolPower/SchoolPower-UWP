using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public String StartDate { get; set; }
        public String EndDate { get; set; }
        public List<Peroid> Grades = new List<Peroid>();

        public Subject(dynamic data) {

            Name         = data.name;
            TeacherName  = data.teacher.firstName + " " + data.teacher.lastName;
            TeacherEmail = data.teacher.email;
            BlockLetter  = data.expression;
            RoomNumber   = data.roomName;
            StartDate    = data.startDate;
            EndDate      = data.endDate;

            JArray assignmentsJarray = (JArray)data.assignments;
            try {
                for (int index = 0; index < assignmentsJarray.Count; index++) {
                    Assignments.Add(new AssignmentItem(assignmentsJarray[index]));
                } 
            } catch (System.NullReferenceException) { }

            String[] peroidList = { "T1", "T2", "X1", "S1", "T3", "T4", "X2", "S2", "Y1" };

            foreach (var time in peroidList) {
                try {
                    if (data.finalGrades[time].percent != null) { 
                        Grades.Add(new Peroid(time, data.finalGrades[time]));
                    }
                } catch (Exception) { }
            }

            // sort
            Assignments.Sort((x, y) => DateTime.Compare(DateTime.Parse(x.Date), DateTime.Parse(y.Date)));
            Assignments.Reverse();
        }
    }

    public class Peroid {
        public String Time { get; set; }
        public String Percent { get; set; }
        public String Letter { get; set; }    
        public String Comment { get; set; }
        public String Eval { get; set; }
        public String StartDate { get; set; }
        public String EndDate { get; set; }

        public Peroid(String time, dynamic data) {
            try {
                Time      = time;
                Percent   = data.percent;
                Percent   = Percent.Substring(0, Percent.IndexOf("."));
                Letter    = data.letter;
                Comment   = data.comment;
                Eval      = data.eval;
                StartDate = data.startDate;
                EndDate   = data.endDate;
            } catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException) { }
        }
    }
}
