/*
    Author: Ricky Bruner
    Purpose: Model for the Order class to represent products from the DB.
    
     ***Contains two foreign keys, CustomerId and PaymentTypeId.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Order
    {
        
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int PaymentTypeId { get; set; }

        public Customer Customer { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
