﻿//author: Klaus Hardt
//purpose: Department model. Employee commented out due to not having Employee model

using BangazonAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Budget { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }

}