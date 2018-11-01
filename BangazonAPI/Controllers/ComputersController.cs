/*
Author: Klaus Hardt
Purpose: Controller for Computer including GET, GET by id, POST, PUT and DELETE 
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
    public class ComputersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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

        // GET api/computers
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string sql = @"
            SELECT
                c.Id,
                c.PurchaseDate,
                c.DecomissionDate
            FROM Computer c
            ";


            using (IDbConnection conn = Connection)
            {

                IEnumerable<Computer> computers = await conn.QueryAsync<Computer>(
                    sql);
               
                return Ok(computers);
            }
        }
        
        // GET api/computers/5
        [HttpGet("{id}", Name = "GetComputer")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $@"
            SELECT
                c.Id,
                c.PurchaseDate,
                c.DecomissionDate
            FROM Computer c
            WHERE c.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Computer> computers = await conn.QueryAsync<Computer>(sql);
                return Ok(computers.Single());
            }
        }
        
        // POST api/Computers
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Computer computers)
        {
            string sql = $@"INSERT INTO Computer
            (PurchaseDate)
            VALUES
            (
                '{computers.PurchaseDate}'
               
            );
            SELECT SCOPE_IDENTITY();";
            Console.WriteLine(sql);
            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                computers.Id = newId;
                return CreatedAtRoute("GetComputer", new { id = newId }, computers);
            }
        }
        
        
        // PUT api/computers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Computer computers)
        {
            string sql = $@"
            UPDATE Computer
            SET PurchaseDate = '{computers.PurchaseDate}',
            DecomissionDate = '{computers.DecomissionDate}'
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
                if (!ComputerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        
        // DELETE api/computers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string sql = $@"DELETE FROM Computer WHERE Id = {id}";

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
        
        private bool ComputerExists(int id)
        {
            string sql = $"SELECT Id FROM Computer WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<Computer>(sql).Count() > 0;
            }
        }

    
    }

}