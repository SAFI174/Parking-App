using ParkingApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ParkingApp.Classes
{
    public class CustomerDataHandler
    {
        public CustomerDataHandler()
        {
            databaseHelper = new DatabaseHelper();
        }
        private DatabaseHelper databaseHelper;
        /// <summary>
        /// Get Customers Data from database and fill it to list
        /// </summary>
        /// <param name="customers"></param>
        public async void GetCustomers(ObservableCollection<Customer> customers)
        {
            await Task.Run(() =>
            {
                var data = databaseHelper.ExecuteWithTable("GetCustomers");
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    try
                    {
                        customers.Add(new Customer()
                        {
                            Id = Convert.ToInt16(data.Rows[i]["ID"]),
                            Name = data.Rows[i]["Name"].ToString(),
                            HourPrice = Convert.ToDouble(data.Rows[i]["HourPrice"]),
                            HalfHourPrice = Convert.ToDouble(data.Rows[i]["HalfHourPrice"])
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
        }
        /// <summary>
        /// Edit Customer Data on database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="hourPrice"></param>
        public void EditCustomer(int id, string name, double hourPrice, double halfHourPrice)
        {
            SqlParameter[] parameters = new SqlParameter[4];
            parameters[0] = new SqlParameter("@ID", SqlDbType.Int);
            parameters[0].Value = id;
            parameters[1] = new SqlParameter("@Name", SqlDbType.NVarChar);
            parameters[1].Value = name;
            parameters[2] = new SqlParameter("@HourPrice", SqlDbType.Decimal);
            parameters[2].Value = hourPrice;
            parameters[3] = new SqlParameter("@HalfHourPrice", SqlDbType.Decimal);
            parameters[3].Value = halfHourPrice;
            databaseHelper.ExecuteWithoutTable("CustomerEdit", parameters);
        }
        /// <summary>
        /// Add Customer to database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hourPrice"></param>
        public void AddCustomer(string name, double hourPrice, double halfHourPrice)
        {
            SqlParameter[] parameters = new SqlParameter[3];
            parameters[0] = new SqlParameter("@Name", SqlDbType.NVarChar);
            parameters[0].Value = name;
            parameters[1] = new SqlParameter("@HourPrice", SqlDbType.Decimal);
            parameters[1].Value = hourPrice;
            parameters[2] = new SqlParameter("@HalfHourPrice", SqlDbType.Decimal);
            parameters[2].Value = halfHourPrice;
            databaseHelper.ExecuteWithoutTable("CustomerAdd", parameters);
        }
        /// <summary>
        /// Delete Customer from database
        /// </summary>
        /// <param name="id"></param>
        public void DeleteCustomer(int id)
        {
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@ID", SqlDbType.Int);
            parameters[0].Value = id;
            databaseHelper.ExecuteWithoutTable("CustomerDelete", parameters);
        }
        /// <summary>
        /// Get last Customer id to add new item to datagrid
        /// </summary>
        /// <returns></returns>
        internal int GetLastId()
        {
            var parkedCarsTable = databaseHelper.ExecuteQueryWithTable("Select ID From Customers Order By ID Desc");
            return Convert.ToInt32(parkedCarsTable.Rows[0]["ID"]);
        }
    }
}
