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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {

        private readonly IConfiguration _config;

        public OrdersController(IConfiguration config)
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

        // GET api/Orders returns all products
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string sql = @"
            SELECT
                o.Id,
                o.PaymentTypeId,
                o.CustomerId,
            FROM Order o
            ";

            using (IDbConnection conn = Connection)
            {

                IEnumerable<Order> products = await conn.QueryAsync<Order>(sql);
                return Ok(products);
            }
        }

        // GET api/Orders/1 returns order with given Id
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $@"
            SELECT
                o.Id,
                o.PaymentTypeId,
                o.CustomerId,
            FROM Order o
            WHERE o.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Product> products = await conn.QueryAsync<Product>(sql);
                return Ok(products.Single());
            }
        }

        // POST api/Orders adds a new Order
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            string sql = $@"INSERT INTO Order 
            (CustomerId, PaymentTypeId)
            VALUES
            (
                '{order.CustomerId}',
                '{order.PaymentTypeId}'
            );
            SELECT SCOPE_IDENTITY();";

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                order.Id = newId;
                return CreatedAtRoute("GetOrder", new { id = newId }, order);
            }
        }

        // PUT api/Orders/5 replaces a Order with the given Id
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Order order)
        {
            string sql = $@"
            UPDATE Order
            SET CustomerId = '{order.CustomerId}',
                PaymentTypeId= '{order.PaymentTypeId}',
            WHERE Id = {id}";

            try
            {
                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);
                    if (rowsAffected > 0)
                    {
                        return new StatusCodeResult(StatusCodes.Status204NoContent);
                    }
                    throw new Exception("No rows affected");
                }
            }
            catch (Exception)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/Orders/5 removes the Order with the given Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string sql = $@"DELETE FROM Order WHERE Id = {id}";

            using (IDbConnection conn = Connection)
            {
                int rowsAffected = await conn.ExecuteAsync(sql);
                if (rowsAffected > 0)
                {
                    return new StatusCodeResult(StatusCodes.Status204NoContent);
                }
                throw new Exception("No rows affected");
            }

        }

        // bool for try/catch
        private bool OrderExists(int id)
        {
            string sql = $"SELECT Id FROM Order WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<Order>(sql).Count() > 0;
            }
        }
    }
}
