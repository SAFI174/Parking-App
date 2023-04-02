using ParkingApp.Models;
using ParkingApp.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel.Contacts;

namespace ParkingApp.Classes
{
    public class ParkedCarsDataHandler
    {
        public ParkedCarsDataHandler()
        {
            _databaseHelper = new DatabaseHelper();
        }
        private DatabaseHelper _databaseHelper;

        // get all parked cars from database
        public async void GetAllParkedCars(ObservableCollection<ParkedCar> parkedCars, ReportsViewModel parkedCarsViewModel)
        {
            await Task.Run(() =>
            {
                // clear the list before add items
                parkedCars.Clear();
                // get datatable from query database
                var parkedCarsTable = _databaseHelper.ExecuteWithTable("GetAllParkedCars", null);
                // add values to list from database
                for (int i = 0; i < parkedCarsTable.Rows.Count; i++)
                {
                    string totalTime = "";
                    DateTime? endDateTime;
                    // convert dbnull value to valid value
                    if (parkedCarsTable.Rows[i]["Cost"] == DBNull.Value)
                    {
                        parkedCarsTable.Rows[i]["Cost"] = 0;
                    }
                    if (parkedCarsTable.Rows[i]["EndDate"] == DBNull.Value)
                    {
                        endDateTime = null;
                    }
                    else
                    {
                        endDateTime = Convert.ToDateTime(parkedCarsTable.Rows[i]["EndDate"]);
                    }

                    if (parkedCarsTable.Rows[i]["TotalTime"] != DBNull.Value)
                    {
                        // Convert from minutes to days and hours and minutes
                        TimeSpan totalTimeSpan = TimeSpan.FromMinutes(Convert.ToDouble(parkedCarsTable.Rows[i]["TotalTime"]));
                        if (totalTimeSpan.Days <= 0 && totalTimeSpan.Hours <= 0)
                        {
                            totalTime = ($"{totalTimeSpan.Minutes} دقيقة");
                        }
                        else if (totalTimeSpan.Days <= 0 && totalTimeSpan.Hours != 0)
                        {
                            // update text of the total time
                            totalTime = ($"{totalTimeSpan.Hours} ساعة {totalTimeSpan.Minutes} دقيقة");
                        }
                        else if (totalTimeSpan.Days > 0)
                        {
                            // update text of the total time
                            totalTime = ($"{totalTimeSpan.Days} يوم  {totalTimeSpan.Hours} ساعة {totalTimeSpan.Minutes} دقيقة");
                        }
                    }

                    if (parkedCarsTable.Rows[i]["UserID"] == DBNull.Value)
                    {
                        parkedCarsTable.Rows[i]["UserID"] = 0;
                        parkedCarsTable.Rows[i]["Username"] = "غير محدد";
                    }
                    parkedCars.Add(new ParkedCar()
                    {
                        ID = Convert.ToInt32(parkedCarsTable.Rows[i]["ID"]),
                        Number = Convert.ToInt32(parkedCarsTable.Rows[i]["Number"]),
                        Barcode = parkedCarsTable.Rows[i]["BarCode"].ToString(),
                        StartDate = Convert.ToDateTime(parkedCarsTable.Rows[i]["StartDate"]),
                        Status = (Convert.ToBoolean(parkedCarsTable.Rows[i]["isActive"]) ? "نشط" : "غير نشط"),
                        EndDate = endDateTime,
                        Cost = Convert.ToDouble(parkedCarsTable.Rows[i]["Cost"]),
                        CustomerName = parkedCarsTable.Rows[i]["Name"].ToString(),
                        CustomerHourPrice = Convert.ToDouble(parkedCarsTable.Rows[i]["CustomerHourPrice"]),
                        CustomerHalfHourPrice = Convert.ToDouble(parkedCarsTable.Rows[i]["CustomerHalfHourPrice"]),
                        Customer = new Customer()
                        {
                            Name = parkedCarsTable.Rows[i]["Name"].ToString(),
                            HourPrice = Convert.ToDouble(parkedCarsTable.Rows[i]["HourPrice"])
                        },
                        User = new Users()
                        {
                            ID = Convert.ToInt32(parkedCarsTable.Rows[i]["UserID"]),
                            Username = parkedCarsTable.Rows[i]["Username"].ToString()
                        },
                        TotalTime = totalTime,
                    });
                }
            });

            // Call CalcTotalEarning to update totalearning value from viewmodel
            parkedCarsViewModel.CalcTotalEarnings();
        }

        // get active parked cars from database
        public void GetActiveParkingCars(ObservableCollection<ParkedCar> parkedCars)
        {
            var parkedCarsTable = _databaseHelper.ExecuteWithTable("GetParkedCars");
            // add values to list from database
            for (int i = 0; i < parkedCarsTable.Rows.Count; i++)
            {
                parkedCars.Add(new ParkedCar()
                {
                    ID = Convert.ToInt32(parkedCarsTable.Rows[i]["ID"]),
                    Number = Convert.ToInt32(parkedCarsTable.Rows[i]["Number"]),
                    Barcode = parkedCarsTable.Rows[i]["BarCode"].ToString(),
                    StartDate = Convert.ToDateTime(parkedCarsTable.Rows[i]["StartDate"]),
                    CustomerName = parkedCarsTable.Rows[i]["Name"].ToString(),
                    CustomerHourPrice = Convert.ToDouble(parkedCarsTable.Rows[i]["CustomerHourPrice"]),
                    Customer = new Customer()
                    {
                        Name = parkedCarsTable.Rows[i]["Name"].ToString(),
                        HourPrice = Convert.ToDouble(parkedCarsTable.Rows[i]["HourPrice"])
                    },
                    CustomerHalfHourPrice = Convert.ToDouble(parkedCarsTable.Rows[i]["CustomerHalfHourPrice"]),
                });
            }
        }

        // get last parked car id
        public int GetLastestParkedCarID()
        {
            var parkedCarsTable = _databaseHelper.ExecuteQueryWithTable("Select ID From ParkedCars Order By ID Desc");
            return Convert.ToInt32(parkedCarsTable.Rows[0]["ID"]);
        }

        // Get lastest car number from database
        public int GetCarNumber()
        {
            var data = _databaseHelper.ExecuteQueryWithTable("SELECT current_value FROM sys.sequences WHERE name = 'CarNumber'");
            return Convert.ToInt32(data.Rows[0]["current_value"]);
        }

        // reset car number from database
        public void ResetCarNumber()
        {
            _databaseHelper.ExecuteQuery("ALTER SEQUENCE dbo.CarNumber Restart");
        }
        // remove a parked car row from database
        public async void RemoveParkedCar(int ID)
        {
            await Task.Run(() =>
            {
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@ID", SqlDbType.Int);
                parameters[0].Value = ID;
                _databaseHelper.ExecuteWithoutTable("DeleteParkedCar", parameters);
            });
        }

        // add new parked car as active
        public async void AddParkedCar(string barcode, int customerID)
        {
            
            try
            {
                await Task.Run(() =>
                {
                    SqlParameter[] parameters = new SqlParameter[2];
                    parameters[0] = new SqlParameter("@Barcode", SqlDbType.NVarChar);
                    parameters[0].Value = barcode;
                    parameters[1] = new SqlParameter("@CustomerID", SqlDbType.Int);
                    parameters[1].Value = customerID;
                    _databaseHelper.ExecuteWithoutTable("AddParkedCar", parameters);
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        // dis active car and make it not active
        public async void DisActiveCar(int ID, string totaltime, double cost)
        {
            await Task.Run(() =>
            {
                SqlParameter[] parameters = new SqlParameter[4];
                parameters[0] = new SqlParameter("@ID", SqlDbType.Int);
                parameters[0].Value = ID;
                parameters[1] = new SqlParameter("@TotalTime", SqlDbType.NVarChar);
                parameters[1].Value = totaltime;
                parameters[2] = new SqlParameter("@Cost", SqlDbType.Decimal);
                parameters[2].Value = cost;
                parameters[3] = new SqlParameter("@UserID", SqlDbType.Int);
                parameters[3].Value = App.Users.ID;
                _databaseHelper.ExecuteWithoutTable("DisActiveCar", parameters);
            });
        }
    }
}
