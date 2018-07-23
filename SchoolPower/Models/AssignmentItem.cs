using System;
using Windows.UI.Xaml.Media;

/*
Sample:
    {
        "category": "Quizzes",
        "description": "Steps of the scientific process for science fair project",
        "name": "Scientific Method Quiz",
        "percentage": "86.96",
        "score": "20",
        "letterGrade": "A",
        "pointsPossible": "23.0",
        "date": "2017-09-11T16:00:00.000Z",
        "weight": "0.43",
        "includeInFinalGrade": "1"
    },
    {
        "category": "Quizzes",
        "description": null,
        "name": "Scientific Notation Quiz",
        "percentage": null,
        "score": null,
        "letterGrade": null,
        "pointsPossible": "10.0",
        "date": "2017-09-05T16:00:00.000Z",
        "weight": "1.0",
        "includeInFinalGrade": "1"
    }
*/

namespace SchoolPower.Models {
    public class AssignmentItem : IEquatable<AssignmentItem> {
        public String Name { get; set; }
        public String Category { get; set; }
        public String Description { get; set; }
        public String Percentage { get; set; }
        public String Score { get; set; }
        public String LetterGrade { get; set; }
        public String IncludeInFinalGrade { get; set; }
        public String MaximumScore { get; set; }
        public String Date { get; set; }
        public String Weight { get; set; }
        public string[] Terms { get; set; }
        public bool IsNew { get; set; }
        public SolidColorBrush Color { get; set; }

        public AssignmentItem(dynamic data) {
            Name                = data.name;
            Date                = data.date;
            Date                = Date.Substring(0, Date.IndexOf(" "));
            Percentage          = data.percent;
            Score               = data.score;
            MaximumScore        = data.pointsPossible;
            MaximumScore        = MaximumScore.Substring(0, MaximumScore.IndexOf("."));
            LetterGrade         = data.letterGrade;
            Category            = data.category;
            IncludeInFinalGrade = data.includeInFinalGrade;
            Weight              = data.weight;
            Terms               = data.terms.ToObject<string[]>();
            IsNew               = true;
            Color               = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 99, 177));

            switch (LetterGrade) {

            }

        }

        public bool Equals(AssignmentItem darling) {
            return Name == darling.Name && Percentage == darling.Percentage && Date == darling.Date && Score == darling.Score && LetterGrade == darling.LetterGrade;
        }
    }
}
