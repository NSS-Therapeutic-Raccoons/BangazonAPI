using System.Collections.Generic;

namespace BangazonAPI.Data
{
    public class ProductTypes
    {
        public int Id { get; set; }
        public string Name { get; set; }

        
       
        public List<ProductTypes> productTypes { get; set; } = new List<ProductTypes>();
    }

}