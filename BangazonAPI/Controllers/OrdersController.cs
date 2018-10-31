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
        public async Task<IActionResult> Get(bool? _completed, string _include)
        {
            string sql = "";

            if (_completed == null && _include == null)
            {
                sql = @"
                    SELECT
                        o.Id,
                        o.PaymentTypeId,
                        o.CustomerId
                    FROM [Order] o
                ";
            }
            else if (_completed != null && _include == null)
            {
                sql = @"
                    SELECT
                        o.Id,
                        o.PaymentTypeId,
                        o.CustomerId
                    FROM [Order] o
                    WHERE 1=1
                ";

                if (_completed == false)
                {
                    string isComplete = $@"
                        AND o.PaymentTypeId IS NULL
                    ";
                    sql = $"{sql} {isComplete}";
                }
                else
                {
                    string isComplete = $@"
                        AND o.PaymentTypeId IS NOT NULL
                    ";
                    sql = $"{sql} {isComplete}";
                }
            }
            else if (_completed == null && _include != null)
            {
                if (_include == "products")
                {
                    sql = @"
                        SELECT
                            p.Id,
                            p.ProductTypeId,
                            p.CustomerId,
                            p.Price,
                            p.Title,
                            p.Description,
                            p.Quantity,
                            o.Id,
                            o.PaymentTypeId,
                            o.CustomerId
                        FROM Product p
                        JOIN OrderProduct op ON p.Id = op.ProductId
                        JOIN [Order] o ON o.Id = op.OrderId
                    ";
                }
                else if (_include == "customer")
                {
                    sql = @"
                        SELECT
                            o.Id,
                            o.PaymentTypeId,
                            o.CustomerId,
                            c.Id,
                            c.FirstName,
                            c.LastName
                        FROM [Order] o
                        JOIN Customer c ON c.Id = o.CustomerId
                    ";
                }
                else
                {
                    sql = @"
                        SELECT
                            o.Id,
                            o.PaymentTypeId,
                            o.CustomerId
                        FROM [Order] o
                    ";
                }
            }
            else if (_completed != null && _include != null)
            {
                if (_include == "products")
                {
                    sql = @"
                        SELECT
                            p.Id,
                            p.ProductTypeId,
                            p.CustomerId,
                            p.Price,
                            p.Title,
                            p.[Description],
                            p.Quantity,
                            o.Id,
                            o.PaymentTypeId,
                            o.CustomerId
                        FROM Product p
                        JOIN OrderProduct op ON p.Id = op.ProductId
                        JOIN [Order] o ON o.Id = op.OrderId
                        WHERE 1=1
                    ";

                    if (_completed == false)
                    {
                        string isComplete = $@"
                            AND o.PaymentTypeId IS NULL
                        ";

                        sql = $"{sql} {isComplete}";

                    }
                    else
                    {
                        string isComplete = $@"
                            AND o.PaymentTypeId IS NOT NULL
                        ";

                        sql = $"{sql} {isComplete}";

                    }
                }
                else if (_include == "customer")
                {
                    sql = @"
                        SELECT
                            o.Id,
                            o.PaymentTypeId,
                            o.CustomerId,
                            c.Id,
                            c.FirstName,
                            c.LastName
                        FROM [Order] o
                        JOIN Customer c ON c.Id = o.CustomerId
                        WHERE 1=1
                    ";

                    if (_completed == false)
                    {
                        string isComplete = $@"
                            AND o.PaymentTypeId IS NULL
                        ";

                        sql = $"{sql} {isComplete}";

                    }
                    else
                    {
                        string isComplete = $@"
                            AND o.PaymentTypeId IS NOT NULL
                        ";

                        sql = $"{sql} {isComplete}";

                    }
                }
                else
                {
                    sql = @"
                        SELECT
                            o.Id,
                            o.PaymentTypeId,
                            o.CustomerId
                        FROM [Order] o
                        WHERE 1 = 1
                    ";

                    if (_completed == false)
                    {
                        string isComplete = $@"
                            AND o.PaymentTypeId IS NULL
                        ";

                        sql = $"{sql} {isComplete}";

                    }
                    else
                    {
                        string isComplete = $@"
                            AND o.PaymentTypeId IS NOT NULL
                        ";

                        sql = $"{sql} {isComplete}";
                    }
                }
            }


            using (IDbConnection conn = Connection)
            {
                Dictionary<int, Order> completeOrders = new Dictionary<int, Order>();

                if (_include != null && _include == "products")
                {
                    IEnumerable<Order> orders = await conn.QueryAsync<Product, Order, Order>(
                        sql,
                        (product, order) =>
                        {
                            if (!completeOrders.ContainsKey(order.Id))
                            {
                                completeOrders[order.Id] = order;
                            }
                            completeOrders[order.Id].Products.Add(product);
                            return order;
                        }
                    );
                }
                else if (_include != null && _include == "customer")
                {
                    IEnumerable<Order> orders = await conn.QueryAsync<Order, Customer, Order>(
                        sql,
                        (order, customer) =>
                        {
                            order.Customer = customer;
                            completeOrders[order.Id] = order;
                            return order;
                        }
                    );
                }
                else if (_include == null)
                {
                    IEnumerable<Order> orders = await conn.QueryAsync<Order>(sql);
                    foreach (Order order in orders) {
                        completeOrders[order.Id] = order;
                    };
                
                }

                List<Order> finalOrders = new List<Order>();
                foreach (KeyValuePair<int, Order> order in completeOrders)
                {
                    finalOrders.Add(order.Value);
                }

                return Ok(finalOrders);
            }
        }

        // GET api/Orders/1 returns order with given Id
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get([FromRoute] int id, string _include)
        {
            string sql = "";
            if (_include == null)
            {
                sql = $@"
                    SELECT
                        o.Id,
                        o.PaymentTypeId,
                        o.CustomerId
                    FROM [Order] o
                    WHERE o.Id = {id}
                ";
            }
            else if (_include == "customer")
            {
                sql = $@"
                    SELECT
                        o.Id,
                        o.PaymentTypeId,
                        o.CustomerId,
                        c.Id,
                        c.FirstName,
                        c.LastName
                    FROM [Order] o
                    JOIN Customer c ON c.Id = o.CustomerId
                    WHERE o.Id = {id}
                ";
            }
            else if (_include == "products")
            {
                sql = $@"
                    SELECT
                        p.Id,
                        p.ProductTypeId,
                        p.CustomerId,
                        p.Price,
                        p.Title,
                        p.[Description],
                        p.Quantity,
                        o.Id,
                        o.PaymentTypeId,
                        o.CustomerId
                    FROM Product p
                    JOIN OrderProduct op ON p.Id = op.ProductId
                    JOIN [Order] o ON o.Id = op.OrderId
                    WHERE o.Id = {id}
                ";
            }


            using (IDbConnection conn = Connection)
            {
                
                Dictionary<int, Order> completeOrder = new Dictionary<int, Order>();

                if (_include == null)
                {
                    IEnumerable<Order> orders = await conn.QueryAsync<Order>(sql);
                    foreach (Order order in orders)
                    {
                        completeOrder[order.Id] = order;
                    }
                }
                else if (_include == "customer")
                {
                    IEnumerable<Order> orders = await conn.QueryAsync<Order, Customer, Order>(
                        sql,
                        (order, customer) =>
                        {
                            order.Customer = customer;
                            completeOrder[order.Id] = order;
                            return order;
                        }
                    );
                }
                else if (_include == "products")
                {
                    IEnumerable<Order> orders = await conn.QueryAsync<Product, Order, Order>(
                        sql,
                        (product, order) =>
                        {
                            if (!completeOrder.ContainsKey(order.Id))
                            {
                                completeOrder[order.Id] = order;
                            }
                            
                            completeOrder[order.Id].Products.Add(product);
                            
                            return order;

                        }
                    );
                }


                return Ok(completeOrder.Values);
            }
        }

        // POST api/Orders adds a new Order
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            string sql = $@"INSERT INTO [Order] 
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
            UPDATE [Order]
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
            string sql = $@"DELETE FROM [Order] WHERE Id = {id}";

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
            string sql = $"SELECT Id FROM [Order] WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<Order>(sql).Count() > 0;
            }
        }
    }
}
