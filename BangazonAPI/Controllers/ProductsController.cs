/*
    Author: Ricky Bruner
    Purpose: API Controller that allows a client to: 
            GET all products from DB, 
            GET a single Product, 
            POST a new Product to the DB, 
            PUT (edit) and existing Product in the DB, and 
            DELETE a Product from the DB 
*/

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

        // GET api/Products returns all products
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string sql = @"
            SELECT
                p.Id,
                p.ProductTypeId,
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

        // GET api/Products/1 returns product with given Id
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $@"
            SELECT
                p.Id,
                p.ProductTypeId,
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
                IEnumerable<Product> products = await conn.QueryAsync<Product>(sql);
                return Ok(products.Single());
            }
        }

        // POST api/products adds a new Product
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            string sql = $@"INSERT INTO Product 
            (ProductTypeId, CustomerId, Price, Title, Description, Quantity)
            VALUES
            (
                '{product.ProductTypeId}',
                '{product.CustomerId}',
                '{product.Price}',
                '{product.Title}',
                '{product.Description}',
                '{product.Quantity}'
            );
            SELECT SCOPE_IDENTITY();";

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                product.Id = newId;
                return CreatedAtRoute("GetProduct", new { id = newId }, product);
            }
        }

        // PUT api/Products/5 replaces a Product with the given Id
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Product product)
        {
            string sql = $@"
            UPDATE Product
            SET ProductTypeId = '{product.ProductTypeId}',
                CustomerId = '{product.CustomerId}',
                Price = '{product.Price}',
                Title = '{product.Title}',
                Description = '{product.Description}',
                Quantity = '{product.Quantity}'
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
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/products/5 removes the product with the given Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string sql = $@"DELETE FROM Product WHERE Id = {id}";

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
        private bool ProductExists(int id)
        {
            string sql = $"SELECT Id FROM Product WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<Product>(sql).Count() > 0;
            }
        }
    }
}
