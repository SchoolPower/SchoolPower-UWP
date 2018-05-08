using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class AttendanceItem { 

        public String Code { get; set; }
        public String Description { get; set; }
        public String Date { get; set; }
        public String Peroid { get; set; }
        public String Name { get; set; } 

        public AttendanceItem(dynamic data) {
            Code        = data.code;
            Description = data.description;
            Date        = data.date;
            Date        = Date.Substring(0, Date.IndexOf(" "));
            Peroid      = data.peroid;
            Name        = data.name;
        }
    }
}
