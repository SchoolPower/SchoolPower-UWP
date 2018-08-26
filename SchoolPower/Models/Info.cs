using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    Sample:
        {
            "currentGPA": null,
            "currentMealBalance": "0.0",
            "currentTerm": null,
            "dcid": "10000",
            "dob": "2001-01-01T16:00:00.000Z",
            "ethnicity": null,
            "firstName": "John",
            "gender": "M",
            "gradeLevel": "10",
            "id": "10000",
            "lastName": "Doe",
            "middleName": "English Name",
            "photoDate": "2016-01-01T16:10:05.699Z",
            "startingMealBalance": "0.0"
        }
*/
namespace SchoolPower.Models {
    public class Info {
        public string GPA { get; set; }
        public string ID { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoDate { get; set; }

        public Info (JObject data) {
            GPA       = data["currentGPA"].ToString();
            ID        = data["id"].ToString();
            Gender    = data["gender"].ToString();
            DOB       = data["dob"].ToString();
            FirstName = data["middleName"].ToString();
            LastName  = data["lastName"].ToString();
            PhotoDate = data["photoDate"].ToString();
        }
    }
}
