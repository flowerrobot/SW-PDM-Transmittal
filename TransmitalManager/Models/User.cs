using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TransmittalManager.SQL_Queries;
using System.Linq;

namespace TransmittalManager.Models
{
    /// <summary>
    /// User permisions should be derived via PDM permisions?
    /// </summary>
    public class User
    {

        static string PDMDesigner = Properties.Settings.Default.PDMUserRights;
        static string PDMIssuer = Properties.Settings.Default.PDMIssuerRights;



        static User _activeUser;
        public static User ActiveUser
        {
            get {
                if (_activeUser != null) return _activeUser;

                Task.Run(() => AllUsersAsync());
                _activeUser = _allUsers?.Values.FirstOrDefault(t => t.UserName == Environment.UserName) ?? null;
                if (_activeUser != null) return _activeUser;

                _activeUser = new User(0) { UserName = Environment.UserName, Group = Groups.NoPermisions };

                return _activeUser;
            }
        }

        static Dictionary<int, User> _allUsers;
        public static async Task<Dictionary<int, User>> AllUsersAsync()
        {
                        if (_allUsers != null) return _allUsers;

            //Load Users from database
            string query = string.Format(SqlScripts.GetAllUsers, PDMDesigner, PDMIssuer);
            using (SqlConnection connection = new SqlConnection(TransmitalManager.connPDMString))
            {
                var t = connection.OpenAsync();
                _allUsers = new Dictionary<int, User>();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await t;
                    var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        int id = (int)reader["UserId"];
                        var grp = (string)reader["GroupName"];
                        User user;
                        if (_allUsers.ContainsKey(id))
                        {
                            user = _allUsers[id];
                        }
                        else
                        {
                            user = new User(id);
                            user.UserName = (string)reader["Username"];
                            user.Initials = (string)reader["Initials"];
                            user.Email = (string)reader["Email"];
                            user.FullName = (string)reader["FullName"];
                        }

                        if (grp == PDMDesigner)
                        {
                            user.Group = user.Group & Groups.Designer;
                        }
                        else if (grp == PDMIssuer)
                        {
                            user.Group = user.Group & Groups.ProjectManager;
                        }
                    }
                }
            }
            return _allUsers;
        }
        public static Dictionary<int, User> AllUsers()
        {
            return AllUsersAsync().Result;
        }



        public User(int id)
        {
            Id = id;
        }
        public User(string UserName) { }

        public int Id { get; internal set; }
        public Groups Group { get; internal set; }
        public string UserName { get; internal set; }
        public string Initials { get; internal set; }
        public string FullName { get; internal set; }
        public string Email { get; internal set; }

    }


}
