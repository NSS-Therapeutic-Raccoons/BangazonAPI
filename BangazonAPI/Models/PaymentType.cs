/*
    Author: Mike Parrish
    Purpose: Model for the PaymentType class to represent products from the DB.
    
     ***Contains one foreign key CustomerId.
*/

using System.Collections.Generic;

namespace BangazonAPI.Data
{
    public class PaymentType
    {
        public int Id { get; set; }
        public int AcctNumber { get; set; }
        public string Name { get; set; }
        public int CustomerId { get; set; }
    }

}