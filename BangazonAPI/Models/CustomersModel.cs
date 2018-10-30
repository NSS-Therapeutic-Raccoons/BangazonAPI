﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
