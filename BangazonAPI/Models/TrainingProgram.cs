/*
    Author: Jeremiah Pritchard
    Purpose: Model for TrainingProgram class to represent training programs from the DB.  
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{

    public class TrainingProgram
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxAttendees { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}

