/*
Author: Klaus Hardt
Purpose: Controller for Product Types including GET, GET by id, POST, PUT and DELETE 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using BangazonAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductTypesController(IConfiguration config)
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

        // GET api/productTypes
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string sql = @"
            SELECT
                p.Id,
                p.Name
            FROM ProductType p
            ";


            using (IDbConnection conn = Connection)
            {

                IEnumerable<ProductType> productTypes = await conn.QueryAsync<ProductType>(
                    sql);
               
                return Ok(productTypes);
            }
        }
        
        // GET api/productTypes/5
        [HttpGet("{id}", Name = "GetProductType")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $@"
            SELECT
                p.Id,
                p.Name
            FROM ProductType p
            WHERE p.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<ProductType> productTypes = await conn.QueryAsync<ProductType>(sql);
                return Ok(productTypes.Single());
            }
        }
        
        // POST api/ProductTypes
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductType productTypes)
        {
            string sql = $@"INSERT INTO ProductType
            (Name)
            VALUES
            (
                '{productTypes.Name}'
            );
            SELECT SCOPE_IDENTITY();";

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                productTypes.Id = newId;
                return CreatedAtRoute("GetProductType", new { id = newId }, productTypes);
            }
        }

        
        // PUT api/productTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductType productTypes)
        {
            string sql = $@"
            UPDATE ProductType
            SET Name = '{productTypes.Name}'
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
                if (!ProductTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        
        // DELETE api/productTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string sql = $@"DELETE FROM ProductType WHERE Id = {id}";

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
        
        private bool ProductTypeExists(int id)
        {
            string sql = $"SELECT Id FROM ProductType WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<ProductType>(sql).Count() > 0;
            }
        }

    
    }

}