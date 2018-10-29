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
    public class PaymentTypesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PaymentTypesController(IConfiguration config)
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

        // GET api/paymenttypes
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string sql = @"
            SELECT
                p.Id,
                p.AcctNumber,
                p.Name,
                p.CustomerId
            FROM PaymentType p
            ";

            using (IDbConnection conn = Connection)
            {

                IEnumerable<PaymentType> paymentTypes = await conn.QueryAsync<PaymentType>(
                    sql
                );
                return Ok(paymentTypes);
            }
        }

        // GET api/paymenttypes/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/paymenttypes
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/paymenttypes/5
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
