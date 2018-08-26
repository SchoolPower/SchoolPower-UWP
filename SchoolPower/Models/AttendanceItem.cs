using Newtonsoft.Json.Linq;
using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;

/*
Sample:
    {
        "code": "E",
        "description": "Excused Absent",
        "date": "2017-10-16T16:00:00.000Z",
        "period": "3(B,D)",
        "name": "Chinese Social Studies 11"
    }
*/

namespace SchoolPower.Models {
    public class AttendanceItem : IEquatable<AttendanceItem> { 

        public string Code { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Peroid { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IsNew { get; set; }
        public SolidColorBrush Color { get; set; }
        public FontWeight SmallTextFontWeight { get; set; }

        public AttendanceItem(JObject data) {
            Code        = data["code"].ToString();
            Description = data["description"].ToString();
            Date        = data["date"].ToString();
            Date        = Date.Substring(0, Date.IndexOf(" "));
            Peroid      = data["period"].ToString();
            Name        = data["name"].ToString();
            DisplayName = Name;
            Color       = StudentData.GetColor(Code);
            SmallTextFontWeight = FontWeights.Normal;
        }

        public bool Equals(AttendanceItem darling) {
            return Name == darling.Name && Code == darling.Code && Date == darling.Date;
        }
    }
}
