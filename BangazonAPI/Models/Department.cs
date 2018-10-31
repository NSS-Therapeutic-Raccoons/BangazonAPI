//author: Klaus Hardt
//purpose: Department model. Employee commented out due to not having Employee model
using System.Collections.Generic;

namespace BangazonAPI.Model
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Budget { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }

}