/*
    Author: Jeremiah Pritchard
    Purpose: API Controller that allows a client to: 
            GET all Customers  from DB, 
            GET a single Customer, 
            POST a new Customer to the DB, 
            PUT (edit) and existing Customerin the DB, and 
            DELETE a Customer from the DB,
            User can also query names of Customers with q= and _include=payment or products
                will show payment types or products in information presented.
 */
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
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _config;
        public CustomersController(IConfiguration config)
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
        // GET api/customers?q=Taco
        [HttpGet]
        public async Task<IActionResult> Get(string q, string _include)
        {
            string sql;
            if (_include != null && _include == "payments")
            {
                sql = @"
            SELECT
                c.Id,
                c.FirstName,
                c.LastName,
                p.Id,
                p.CustomerId,
                p.Name    
            FROM Customer c
            LEFT JOIN PaymentType p ON p.CustomerId = c.Id          
            WHERE 1=1
            ";
                if (q != null)
                {
                    string isQ = $@"
                    AND c.FirstName LIKE '%{q}%'
                    OR c.LastName LIKE '%{q}%'
                ";
                    sql = $"{sql} {isQ}";
                }
            }
            else if (_include != null && _include == "products")
            {
                sql = $@"
                SELECT
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    pr.Id,
                    pr.Description,
                    pr.Title,
                    pr.Price,
                    pr.Quantity
                FROM Customer c
                LEFT JOIN Product pr ON pr.CustomerId = c.Id
                WHERE 1=1
                ";
                if (q != null)
                {
                    string isQ = $@"
                    AND c.FirstName LIKE '%{q}%'
                    OR c.LastName LIKE '%{q}%'
                ";
                    sql = $"{sql} {isQ}";
                }
            }
            else
            {
                sql = @"
            SELECT
                c.Id,
                c.FirstName,
                c.LastName
            FROM Customer c
            WHERE 1=1
            ";
                if (q != null)
                {
                    string isQ = $@"
                    AND c.FirstName LIKE '%{q}%'
                    OR c.LastName LIKE '%{q}%'
                ";
                    sql = $"{sql} {isQ}";
                }
            }
            using (IDbConnection conn = Connection)
            {
                if (_include != null && _include == "payments")
                {
                    Dictionary<int, Customer> customerPay = new Dictionary<int, Customer>();
                    IEnumerable<Customer> customers = await conn.QueryAsync<Customer, PaymentType, Customer>(
                sql,
                (customer, payment) =>
                {
                    if (!customerPay.ContainsKey(customer.Id))
                    {
                        customerPay[customer.Id] = customer;
                    }
                    customerPay[customer.Id].Payments.Add(payment);
                    return customer;
                }

                );
                    return Ok(customerPay.Values);
                }
                else if (_include != null && _include == "products")
                {
                    Dictionary<int, Customer> customerProduct = new Dictionary<int, Customer>();
                    IEnumerable<Customer> customers = await conn.QueryAsync<Customer, Product, Customer>(
                    sql,
                    (customer, product) =>
                    {
                        if (!customerProduct.ContainsKey(customer.Id))
                        {
                            customerProduct[customer.Id] = customer;
                        }
                        customerProduct[customer.Id].Products.Add(product);
                        return customer;
                    });
                    return Ok(customerProduct.Values);
                }
                else
                {
                    IEnumerable<Customer> customers = await conn.QueryAsync<Customer>(
                            sql
                          );
                    return Ok(customers);
                }
            }
        }
        // GET api/customers/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> Get([FromRoute]int id, string _include)
        {
            string sql;
            if (_include != null && _include == "payments")
            {
                sql = $@"
            SELECT
                c.Id,
                c.FirstName,
                c.LastName,
                p.Id,
                p.CustomerId,
                p.Name    
            FROM Customer c
            LEFT JOIN PaymentType p ON p.CustomerId = c.Id          
            WHERE c.Id = {id}
            ";

            }
            else if (_include != null && _include == "products")
            {
                sql = $@"
                SELECT
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    pr.Id,
                    pr.Description,
                    pr.Title,
                    pr.Price,
                    pr.Quantity
                FROM Customer c
                LEFT JOIN Product pr ON pr.CustomerId = c.Id
                WHERE c.Id = {id}
                ";
            }
            else
            {
                sql = $@"
            SELECT
                c.Id,
                c.FirstName,
                c.LastName
            FROM Customer c
            WHERE c.Id = {id}
            ";
            }
            using (IDbConnection conn = Connection)
            {
                if (_include != null && _include == "payments")
                {
                    Dictionary<int, Customer> customerPay = new Dictionary<int, Customer>();
                    IEnumerable<Customer> customers = await conn.QueryAsync<Customer, PaymentType, Customer>(
                sql,
                (customer, payment) =>
                {
                    if (!customerPay.ContainsKey(customer.Id))
                    {
                        customerPay[customer.Id] = customer;
                    }
                    customerPay[customer.Id].Payments.Add(payment);
                    return customer;
                }
                );
                    return Ok(customerPay.Values);
                }
                else if (_include != null && _include == "products")
                {
                    Dictionary<int, Customer> customerProduct = new Dictionary<int, Customer>();
                    IEnumerable<Customer> customers = await conn.QueryAsync<Customer, Product, Customer>(
                    sql,
                    (customer, product) =>
                    {
                        if (!customerProduct.ContainsKey(customer.Id))
                        {
                            customerProduct[customer.Id] = customer;
                        }
                        customerProduct[customer.Id].Products.Add(product);
                        return customer;
                    });
                    return Ok(customerProduct.Values);
                }
                else
                {
                    IEnumerable<Customer> customers = await conn.QueryAsync<Customer>(
                            sql
                          );

                    return Ok(customers);

                }
            }
        }
        // POST api/customers
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            string sql = $@"INSERT INTO Customer 
            (FirstName, LastName)
            VALUES
            (
                '{customer.FirstName}'
                ,'{customer.LastName}'
            );
            SELECT SCOPE_IDENTITY();";

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                customer.Id = newId;
                return CreatedAtRoute("GetCustomer", new { id = newId }, customer);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Customer customer)
        {
            string sql = $@"
            UPDATE Customer
                SET FirstName = '{customer.FirstName}',
                LastName = '{customer.LastName}'
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
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        private bool CustomerExists(int id)
        {
            string sql = $"SELECT Id FROM Customer WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<Customer>(sql).Count() > 0;
            }
        }
    }
}

