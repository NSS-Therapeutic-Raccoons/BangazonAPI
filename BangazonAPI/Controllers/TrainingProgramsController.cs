using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Http;
using BangazonAPI.Models;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingProgramsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
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

        // GET api/training
        [HttpGet]
        public async Task<IActionResult> Get(bool? completed)
        {
            string sql;
           if (completed == false) {
                    sql = @"
            SELECT
                tp.Id,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees,
                e.Id,
                e.FirstName,
                e.LastName
            FROM TrainingProgram tp
            LEFT JOIN EmployeeTraining et ON tp.Id = et.TrainingProgramId
            LEFT JOIN Employee e ON e.Id = et.EmployeeId
            WHERE StartDate >=  CONVERT(DATETIME, {fn CURDATE()});
            ";

            }
            else
            {
                sql = @"
            SELECT
                tp.Id,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees,
                e.Id,
                e.FirstName,
                e.LastName
            FROM TrainingProgram tp
            LEFT JOIN EmployeeTraining et ON tp.Id = et.TrainingProgramId
            LEFT JOIN Employee e ON e.Id = et.EmployeeId
            WHERE 1=1
            ";

            }
            using (IDbConnection conn = Connection)
            {
                Dictionary<int, TrainingProgram> employeeTrain = new Dictionary<int, TrainingProgram>();
                IEnumerable<TrainingProgram> customers = await conn.QueryAsync<TrainingProgram, Employee, TrainingProgram>(
                sql,
                (trainingprogram, employee) =>
                {
                    if (!employeeTrain.ContainsKey(trainingprogram.Id))
                    {
                        employeeTrain[trainingprogram.Id] = trainingprogram;
                    }
                    employeeTrain[trainingprogram.Id].Employees.Add(employee);
                    return trainingprogram;
                });
                return Ok(employeeTrain.Values);
            }
        }

        // GET api/trainingProgram/5
        [HttpGet("{id}", Name = "GetTraining")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $@"
            SELECT
                tp.Id,
                tp.StartDate,
                tp.EndDate,
                tp.MaxAttendees,
                e.Id,
                e.FirstName,
                e.LastName
            FROM TrainingProgram tp
            LEFT JOIN EmployeeTraining et ON tp.Id = et.TrainingProgramId
            LEFT JOIN Employee e ON e.Id = et.EmployeeId
            WHERE tp.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                Dictionary<int, TrainingProgram> employeeTrain = new Dictionary<int, TrainingProgram>();
                IEnumerable<TrainingProgram> customers = await conn.QueryAsync<TrainingProgram, Employee, TrainingProgram>(
                sql,
                (trainingprogram, employee) =>
                {
                    if (!employeeTrain.ContainsKey(trainingprogram.Id))
                    {
                        employeeTrain[trainingprogram.Id] = trainingprogram;
                    }
                    employeeTrain[trainingprogram.Id].Employees.Add(employee);
                    return trainingprogram;
                });
                return Ok(employeeTrain.Values);
            }
        }

        // POST api/trainingPrograms
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TrainingProgram trainingProgram)
        {
            string sql = $@"INSERT INTO TrainingProgram 
            (StartDate, EndDate, MaxAttendees)
            VALUES
            (
                '{trainingProgram.StartDate}'
                ,'{trainingProgram.EndDate}'
                ,'{trainingProgram.MaxAttendees}'
            );
            SELECT SCOPE_IDENTITY();";

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                trainingProgram.Id = newId;
                return CreatedAtRoute("GetTraining", new { id = newId }, trainingProgram);
            }
        }

        // PUT api/paymenttypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PaymentType paymentType)
        {
            string sql = $@"
            UPDATE PaymentType
            SET AcctNumber = '{paymentType.AcctNumber}',
                Name = '{paymentType.Name}',
                CustomerId = '{paymentType.CustomerId}'
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
                if (!PaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/paymenttypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string sql = $@"DELETE FROM PaymentType WHERE Id = {id}";

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

        private bool PaymentTypeExists(int id)
        {
            string sql = $"SELECT Id FROM PaymentType WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<PaymentType>(sql).Count() > 0;
            }
        }
    }
}
