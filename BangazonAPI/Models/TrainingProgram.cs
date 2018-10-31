/*
<<<<<<< HEAD
    Author: Jeremiah Pritchard
    Purpose: Model for TrainingProgram class to represent training programs from the DB.
    
=======
    Author: Mike Parrish
    Purpose: Model for the TrainingProgram class to represent products from the DB
>>>>>>> master
*/

using BangazonAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{

    public class TrainingProgram
    {
<<<<<<< HEAD
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxAttendees { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
=======

        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MaxAttendees { get; set; }

    }

}
>>>>>>> master
