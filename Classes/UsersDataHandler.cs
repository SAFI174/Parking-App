using ParkingApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

namespace ParkingApp.Classes
{
    public class UsersDataHandler
    {
        public UsersDataHandler()
        {
            databaseHelper = new DatabaseHelper();
        }

        private DatabaseHelper databaseHelper;

        // get all users from database
        public ObservableCollection<Users> GetUsers(ObservableCollection<Users> users)
        {
            // get data
            var usersDataTable = databaseHelper.ExecuteWithTable("GetUsers", null);
            // add data to the list
            for (int i = 0; i < usersDataTable.Rows.Count; i++)
            {
                users.Add(new Users()
                {
                    ID = Convert.ToInt32(usersDataTable.Rows[i]["ID"]),
                    FullName = usersDataTable.Rows[i]["FullName"].ToString(),
                    Username = usersDataTable.Rows[i]["Username"].ToString(),
                    Password = usersDataTable.Rows[i]["Password"].ToString(),
                    isAdmin = (bool)usersDataTable.Rows[i]["isAdmin"]
                });
            }
            return users;
        }

        // delete user data from database
        public void RemoveUser(int userId)
        {
            SqlParameter[] parameters = new SqlParameter[1];

            parameters[0] = new SqlParameter("@ID", SqlDbType.Int);
            parameters[0].Value = userId;

            databaseHelper.ExecuteWithoutTable("UserDelete", parameters);

        }

        // edit user data on database
        public void EditUser(int userId, string fullname, string username, string password, bool isAdmin)
        {
            SqlParameter[] parameters = new SqlParameter[5];

            parameters[0] = new SqlParameter("@ID", SqlDbType.Int);
            parameters[0].Value = userId;

            parameters[1] = new SqlParameter("@FullName", SqlDbType.NVarChar);
            parameters[1].Value = fullname;

            parameters[2] = new SqlParameter("@Username", SqlDbType.NVarChar);
            parameters[2].Value = username;

            parameters[3] = new SqlParameter("@Password", SqlDbType.NVarChar);
            parameters[3].Value = password;

            parameters[4] = new SqlParameter("@isAdmin", SqlDbType.Bit);
            parameters[4].Value = isAdmin;

            databaseHelper.ExecuteWithoutTable("UserEdit", parameters);
        }

        // add user to database
        public void AddUser(string fullname, string username, string password, bool isAdmin)
        {
            SqlParameter[] parameters = new SqlParameter[4];

            parameters[0] = new SqlParameter
            {
                Value = fullname,
                ParameterName = "@FullName",
                SqlDbType = SqlDbType.NVarChar
            };

            parameters[1] = new SqlParameter("@Username", SqlDbType.NVarChar);
            parameters[1].Value = username;

            parameters[2] = new SqlParameter("@Password", SqlDbType.NVarChar);
            parameters[2].Value = password;

            parameters[3] = new SqlParameter("@isAdmin", SqlDbType.Bit);
            parameters[3].Value = isAdmin;

            databaseHelper.ExecuteWithoutTable("UserAdd", parameters);
        }

        // check if user already exist in the database to help not duplicate users
        public bool CheckifUsersExist(string username)
        {
            var data = databaseHelper.ExecuteQueryWithTable($"SELECT * FROM USERS WHERE Username = '{username}'");

            if (data.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // check login data
        // if return a 1 row then the username and password are valid
        public bool CheckLoginData(Users user, string username, string password)
        {
            var data = databaseHelper.ExecuteQueryWithTable($"SELECT * FROM USERS WHERE Username = '{username}' and Password = '{password}'");

            if (data.Rows.Count > 0)
            {
                user.ID = Convert.ToInt32(data.Rows[0]["ID"]);
                user.FullName = data.Rows[0]["FullName"].ToString();
                user.Username = data.Rows[0]["Username"].ToString();
                user.isAdmin = (bool)data.Rows[0]["isAdmin"];
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
