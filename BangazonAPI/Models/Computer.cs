/*
    Author: Mike Parrish
    Purpose: Model for the Computer class to represent products from the DB
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{

    public class Computer
    {

        public int Id { get; set; }

        public DateTime PurchaseDate { get; set; }

        public DateTime? DecomissionDate { get; set; }

    }

}