/*
    Author: Mike Parrish
    Purpose: Model for the Employee class to represent products from the DB.
    
     ***Contains two foreign keys; DepartmentId, and ComputerId.
*/

using System.Collections.Generic;

namespace BangazonAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DepartmentId { get; set; }
        public bool IsSuperVisor { get; set; }
    }
}