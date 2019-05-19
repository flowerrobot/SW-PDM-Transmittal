using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace TransmittalManager.Models
{
    /// <summary>
    /// The class is a project, and a list of all projects.
    /// </summary>
    public class Project
    {
       static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static List<Project> _projects;
        /// <summary>
        /// Will load and cache all projects avalible in the list.
        /// </summary>
        public static List<Project> AllProjects
        {
            get {
                if (_projects != null) return _projects;

                CancellationTokenSource ctk = new CancellationTokenSource();
               var a= AllProjectsAsync(ctk.Token);
                a.Wait();
                return _projects;
            }
        }

        public static async Task<List<Project>> AllProjectsAsync(CancellationToken cts)
        {
            if (_projects != null) return _projects;
            try
            {



                string query = string.Format(SQL_Queries.SqlScripts.GetAllProjects);
                using (SqlConnection connection = new SqlConnection(TransmitalManager.connString))
                {
                    var t = connection.OpenAsync(cts);
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        _projects = new List<Project>();
#if DEBUG 
                        _projects.Add(new Project() { Name = "Test Proj", Number = 001234 });
                        _projects.Add(new Project() { Name = "Another Project", Number = 005678 });
#endif
                        await t;
                        var reader = await command.ExecuteReaderAsync(cts);

                        while (await reader.ReadAsync(cts) && !cts.IsCancellationRequested)
                        {
                            _projects.Add(new Project() { Number = (int)reader["ProjNumber"], Name = (string)reader["ProjName"] });
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
            return _projects;
        }

        public int Number { get; set; }
        /// <summary>
        /// Project number padded out to suite stds.
        /// </summary>
        public string NumberStr => Number.ToString("000000");
        public string Name { get; set; }

        public override string ToString() => $"{NumberStr} {Name}";
    }
}
