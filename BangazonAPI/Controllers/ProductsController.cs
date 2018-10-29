using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.AspNetCore.Http;


namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IConfiguration _config;

        public ProductsController(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET api/Products
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string sql = @"
            SELECT
                p.Id,
                p.PaymentTypeId,
                p.CustomerId,
                p.Price,
                p.Title,
                p.Description,
                p.Quantity
            FROM Product p
            ";

            using (IDbConnection conn = Connection)
            {

                IEnumerable<Product> products = await conn.QueryAsync<Product>(sql);
                return Ok(products);
            }
        }

        // GET api/Products/1
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $@"
            SELECT
                p.Id,
                p.PaymentTypeId,
                p.CustomerId,
                p.Price,
                p.Title,
                p.Description,
                p.Quantity
            FROM Product p
            WHERE p.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Product> students = await conn.QueryAsync<Product>(sql);
                return Ok(students);
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
