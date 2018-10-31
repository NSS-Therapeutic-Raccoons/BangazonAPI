/*
    Author: Mike Parrish
    Purpose: API Controller that allows a client to: 
            GET all employees from DB, 
            GET a single employee type, 
            POST a new employee to the DB, 
            PUT (edit) and existing employee in the DB, and 
            DELETE an employee from the DB 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using BangazonAPI.Data;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
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

        // GET api/employees
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string sql = @"
            SELECT
                p.Id,
                p.FirstName,
                p.LastName,
                p.DepartmentId
                p.IsSuperVisor
            FROM Employee p
            ";

            using (IDbConnection conn = Connection)
            {

                IEnumerable<Employee> employees = await conn.QueryAsync<Employee>(
                    sql
                );
                return Ok(employees);
            }
        }

        // GET api/employees/5
        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $@"
            SELECT
                p.Id,
                p.FirstName,
                p.LastName,
                p.DepartmentId
                p.IsSuperVisor
            FROM Employee p
            WHERE p.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Employee> employees = await conn.QueryAsync<Employee>(sql);
                return Ok(employees.Single());
            }
        }

        // POST api/employees
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee employees)
        {
            string sql = $@"INSERT INTO Employee 
            (FirstName, LastName, DepartmentId, ComputerId, IsSuperVisor)
            VALUES
            (
                '{employees.FirstName}'
                ,'{employees.LastName}'
                ,'{employees.DepartmentId}'
                ,'{employees.ComputerId}'
                ,'{employees.IsSuperVisor}'
            );
            SELECT SCOPE_IDENTITY();";

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                employees.Id = newId;
                return CreatedAtRoute("GetEmployee", new { id = newId }, employees);
            }
        }

        // PUT api/employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Employee employees)
        {
            string sql = $@"
            UPDATE Employee
            SET FirstName = '{employees.FirstName}',
                LastName = '{employees.LastName}',
                DepartmentId = '{employees.DepartmentId}',
                ComputerId = '{employees.ComputerId}',
                IsSuperVisor = '{employees.IsSuperVisor}'
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
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        /* **Delete is not needed based on requirements of ticket**
        // DELETE api/employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string sql = $@"DELETE FROM Employee WHERE Id = {id}";

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

        private bool EmployeeExists(int id)
        {
            string sql = $"SELECT Id FROM Employee WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<Employee>(sql).Count() > 0;
            }
        }
    }
}