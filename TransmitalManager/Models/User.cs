using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TransmittalManager.SQL_Queries;
using System.Linq;
using System.Threading;

namespace TransmittalManager.Models
{
    /// <summary>
    /// User permisions should be derived via PDM permisions?
    /// </summary>
    public class User
    {

        static string PDMDesigner = Properties.Settings.Default.PDMUserRights;
        static string PDMIssuer = Properties.Settings.Default.PDMIssuerRights;

        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static User _activeUser;
        public static User ActiveUser
        {
            get {
                if (_activeUser != null) return _activeUser;

                CancellationTokenSource ctk = new CancellationTokenSource();
                Task.Run(() => AllUsersAsync(ctk.Token));
                _activeUser = _allUsers?.Values.FirstOrDefault(t => t.UserName == Environment.UserName) ?? null;
                if (_activeUser != null) return _activeUser;
#if DEBUG
                       _activeUser = new User(0) { UserName = Environment.UserName, Group = Groups.Designer | Groups.ProjectManager };
#else
                _activeUser = new User(0) { UserName = Environment.UserName, Group = Groups.NoPermisions };
#endif

                return _activeUser;
            }
        }

        static Dictionary<int, User> _allUsers;
        public static async Task<Dictionary<int, User>> AllUsersAsync(CancellationToken ctk)
        {
            if (_allUsers != null) return _allUsers;
            try
            {
                //var ctk = new  CancellationTokenSource();
                //Load Users from database
                string query = string.Format(SqlScripts.GetAllUsers, PDMDesigner, PDMIssuer);
                using (SqlConnection connection = new SqlConnection(TransmitalManager.connPDMString))
                {
                    var t = connection.OpenAsync(ctk);
                    _allUsers = new Dictionary<int, User>();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await t;
                        var reader = await command.ExecuteReaderAsync(ctk);

                        while (await reader.ReadAsync(ctk) && !ctk.IsCancellationRequested)
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
            }
            catch (Exception ex)
            {
#if DEBUG
                 System.Diagnostics.Debugger.Break();
#endif
                logger.Error(ex);
            }
              _allUsers.Add(0,new User(0) { UserName = Environment.UserName, Group = Groups.Designer | Groups.ProjectManager });
            return _allUsers;
        }
        public static Dictionary<int, User> AllUsers()
        {
            CancellationTokenSource ctk = new CancellationTokenSource();
            return AllUsersAsync(ctk.Token).Result;
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
