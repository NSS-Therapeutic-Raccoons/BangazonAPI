/*
Author: Klaus Hardt
Purpose: Controller for Department including GET, GET by id, POST
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using BangazonAPI.Model;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
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

        // GET api/departments/This includes the greater than and equals to filter. Any number should work
        [HttpGet]
        public async Task<IActionResult> Get(int? _filter)
        {
            string sql = @"
            SELECT
                d.Id,
                d.Name,
                d.Budget
            FROM Department d
            WHERE 1=1
            ";


            if (_filter != null)
            {
                string isQ = $@"
                    AND d.Budget >= {_filter}
                ";
                sql = $"{sql} {isQ}";
            }

            using (IDbConnection conn = Connection)
            {

                IEnumerable<Department> departments = await conn.QueryAsync<Department>(
                    sql);

                return Ok(departments);
            }
        }

        // GET api/departments/5
        [HttpGet("{id}", Name = "GetDepartment")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $@"
            SELECT
                d.Id,
                d.Name,
                d.Budget
            FROM Department d
            WHERE d.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Department> departments = await conn.QueryAsync<Department>(sql);
                return Ok(departments.Single());
            }
        }

        // POST api/Department
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Department departments)
        {
            string sql = $@"INSERT INTO Department
            (Name, Budget)
            VALUES
            (
                '{departments.Name}',
                '{departments.Budget}'
            );
            SELECT SCOPE_IDENTITY();";

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                departments.Id = newId;
                return CreatedAtRoute("GetDepartment", new { id = newId }, departments);
            }
        }


        // PUT api/departments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Department departments)
        {
            string sql = $@"
            UPDATE Department
            SET Name = '{departments.Name}',
            Budget = '{departments.Budget}'
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
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        /*
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
        */
        private bool DepartmentExists(int id)
        {
            string sql = $"SELECT Id FROM Department WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<Department>(sql).Count() > 0;
            }
        }


    }

}