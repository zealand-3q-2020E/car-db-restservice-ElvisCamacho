using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using WebApiCar.Model;

namespace WebApiCar.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        string link = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CarDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public static List<Car> carList = new List<Car>()
        {
            new Car() {Id = 1, Model = "x3", Vendor = "Tesla", Price = 400000},
            new Car() {Id = 2, Model = "x2", Vendor = "Tesla", Price = 600000},
            new Car() {Id = 3, Model = "x1", Vendor = "Tesla", Price = 800000},
            new Car() {Id = 4, Model = "x0", Vendor = "Tesla", Price = 1400000},
        };



        /// <summary>
        /// Method for get all the cars from the static list
        /// </summary>
        /// <returns>List of cars</returns>
        // GET: api/Cars
        [HttpGet]
        public IEnumerable<Car> Get()
        {
            List<Car> listOfCars = new List<Car>();

            String selectAllCars = "Select id, vendor, model, price from Car";

            using (SqlConnection dataBaseConnection = new SqlConnection(link))
            {
                dataBaseConnection.Open();
                using (SqlCommand selectCommand = new SqlCommand(selectAllCars, dataBaseConnection))
                {
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string vendor = reader.GetString(1);
                            string model = reader.GetString(2);
                            int price = reader.GetInt32(3);

                            Car newCar = new Car(id, vendor, model, price);
                            listOfCars.Add(newCar);
                        }
                    }
                }
            }

            return listOfCars;
        }

        [HttpGet("byvendor/{vendor}")]
        public IEnumerable<Car> GetByVendor(string vendor)
        {
            return carList;
        }

        // GET: api/Cars/5
        [HttpGet("{id}", Name = "Get")]
        public Car Get(int id)
        {
            String selectById = "select id, vendor, model, price from car where id=@id";

            using (SqlConnection dataBaseConnection = new SqlConnection(link))
            {
                using (SqlCommand selectedCommand = new SqlCommand(selectById, dataBaseConnection))
                {
                    selectedCommand.Parameters.AddWithValue("@id", id);
                    dataBaseConnection.Open();
                    using (SqlDataReader reader = selectedCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id1 = reader.GetInt32(0);
                            string vendor = reader.GetString(1);
                            string model = reader.GetString(2);
                            int price = reader.GetInt32(3);

                            Car newCar = new Car(id1, vendor, model, price);
                            return newCar;

                        }
                    }
                }
            }
            return null;

            //return carList.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Post a new car to the static list
        /// </summary>
        /// <param name="value"></param>
        // POST: api/Cars
        [HttpPost]
        public void Post([FromBody] Car value)
        {
            string insertSql =
           "insert into car(id, model, vendor, price) values(@id, @model, @vendor, @price)";

            using (SqlConnection dataBaseConnection = new SqlConnection(link))
            {
                dataBaseConnection.Open();
                using (SqlCommand insertCommand = new SqlCommand(insertSql, dataBaseConnection))
                {
                    insertCommand.Parameters.AddWithValue("@id", value.Id);
                    insertCommand.Parameters.AddWithValue("@model", value.Model);
                    insertCommand.Parameters.AddWithValue("@vendor", value.Vendor);
                    insertCommand.Parameters.AddWithValue("@Price", value.Price);

                    var rowsAffected = insertCommand.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");

                }
            }


            Car newcar = new Car() { Id = GetId(), Model = value.Model, Vendor = value.Vendor, Price = value.Price };
            carList.Add(newcar);
        }



        // PUT: api/Cars/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Car car)
        {
            string query = "Update car set id=@id ,vendor=@vendor, model=@model, price=@price  where id=@id";
            using (SqlConnection conn = new SqlConnection(link))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@id", car.Id);
                command.Parameters.AddWithValue("@model", car.Model);
                command.Parameters.AddWithValue("@vendor", car.Vendor);
                command.Parameters.AddWithValue("@price", car.Vendor);
                int affectedRows = command.ExecuteNonQuery();
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            string query = "delete from car where id=@id";
            using (SqlConnection dataBaseConnection = new SqlConnection(link))
            {
                dataBaseConnection.Open();
                using (SqlCommand insertCommand = new SqlCommand(query, dataBaseConnection))
                {
                    insertCommand.Parameters.AddWithValue("@id", id);

                    var rowsAffected = insertCommand.ExecuteNonQuery();

                }
            }
            //carList.Remove(Get(id));
        }


        int GetId()
        {
            int max = carList.Max(x => x.Id);
            return max + 1;
        }

    }
}
