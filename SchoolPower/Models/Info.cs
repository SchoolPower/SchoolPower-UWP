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
        public String GPA { get; set; }
        public String ID { get; set; }
        public String Gender { get; set; }
        public String DOB { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String PhotoDate { get; set; }

        public Info (dynamic data) {
            GPA       = data.currentGPA;
            ID        = data.id;
            Gender    = data.gender;
            DOB       = data.dob;
            FirstName = data.middleName;
            LastName  = data.lastName;
            PhotoDate = data.photoDate;
        }
    }
}
