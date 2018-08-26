using System;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
using Newtonsoft.Json.Linq;

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
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Percentage { get; set; }
        public string Score { get; set; }
        public string LetterGrade { get; set; }
        public string IncludeInFinalGrade { get; set; }
        public string MaximumScore { get; set; }
        public string Date { get; set; }
        public string Weight { get; set; }
        public string[] Terms { get; set; }
        public bool IsNew { get; set; }
        public FontWeight LargeTextFontWeight { get; set; }
        public FontWeight SmallTextFontWeight { get; set; }
        public SolidColorBrush Color { get; set; }

        public AssignmentItem(JObject data) {
            Name                = data["name"].ToString();
            DisplayName         = data["name"].ToString();
            Date                = data["date"].ToString();
            Date                = Date.Substring(0, Date.IndexOf(" "));
            Percentage          = data["percent"].ToString();
            Score               = data["score"].ToString();
            MaximumScore        = data["pointsPossible"].ToString();
            MaximumScore        = MaximumScore.Substring(0, MaximumScore.IndexOf("."));
            LetterGrade         = data["letterGrade"].ToString();
            Category            = data["category"].ToString();
            IncludeInFinalGrade = data["includeInFinalGrade"].ToString();
            Weight              = data["weight"].ToString();
            Terms               = data["terms"].ToObject<string[]>();
            IsNew               = false;
            LargeTextFontWeight = FontWeights.SemiLight;
            SmallTextFontWeight = FontWeights.Normal;
            Color               = StudentData.GetColor(LetterGrade);
        }

        public bool Equals(AssignmentItem darling) {
            return Name == darling.Name && Percentage == darling.Percentage && Date == darling.Date && Score == darling.Score && LetterGrade == darling.LetterGrade;
        }
    }
}
