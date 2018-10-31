/*
    Author: Ricky Bruner
    Purpose: Model for the Customer class to represent products from the DB.
*/

using BangazonAPI.Data;
using System;
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

        public List<PaymentType> Payments { get; set; } = new List<PaymentType>();

        public List<Product> Products { get; set; } = new List<Product>();

    }

}