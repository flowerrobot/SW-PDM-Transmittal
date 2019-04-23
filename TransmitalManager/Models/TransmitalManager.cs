using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using TransmittalManager.SQL_Queries;
using TransmittalManager.ViewModel;

namespace TransmittalManager.Models
{
    internal class TransmitalManager
    {
        internal static string connString = ConfigurationManager.ConnectionStrings["aws"].ConnectionString;// "mydefaultdb.curg5pnvqvyj.ap-southeast-2.rds.amazonaws.com,1433 - mydefaultdb";
        internal static string connPDMString = ConfigurationManager.ConnectionStrings["PDMVault"].ConnectionString;// "mydefaultdb.curg5pnvqvyj.ap-southeast-2.rds.amazonaws.com,1433 - mydefaultdb";

        public TransmitalManager()
        {

        }

        //public CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource();
        public async Task<List<Transmittal>> SearchForAnyAsync(IProgress<Transmittal> e, CancellationToken cancellationToken)
        {
            List<Transmittal> res = new List<Transmittal>();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                string query = SqlScripts.GetAllTransmittals;
                SqlCommand command = new SqlCommand(query, connection);
                await connection.OpenAsync(cancellationToken);

                using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        Transmittal tran = new Transmittal(reader);// LoadTransmittal(reader);
                        res.Add(tran);
                        e.Report(tran);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            reader.Close();
                            connection.Close();
                            return res;
                        }
                    }

                }
            }
            e.Report(null);
            return res;
        }
        public async Task<List<Transmittal>> SearchByIdAsync(int id,CancellationToken cancellationToken)
        {
            List<Transmittal> res = new List<Transmittal>();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                string query1 = "SELECT * FROM Transmittal where Id = " + id.ToString();
                SqlCommand command = new SqlCommand(query1, connection);
                await connection.OpenAsync(cancellationToken);

                using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        Transmittal tran = new Transmittal(reader);// LoadTransmittal(reader);
                        res.Add(tran);
                    }
                }
            }
            return res;
        }
        public async Task<List<Transmittal>> SearchByParametersAsync(SearchViewModel search, IProgress<Transmittal> e,CancellationToken cancellationToken)
        {
            List<Transmittal> res = new List<Transmittal>();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                string query ;

                bool hasFiles = !string.IsNullOrEmpty(search.FileName);

                if (hasFiles)
                {
                    query = string.Format(SqlScripts.GetTransmitalsFileName, search.FileName, AddParameters(search));
                }
                else
                {
                    query = string.Format(SqlScripts.GetTransmittals, AddParameters(search));
                }

                SqlCommand command = new SqlCommand(query, connection);
                await connection.OpenAsync(cancellationToken);

                using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (hasFiles)
                    {
                        Transmittal activeTran = null;
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            var file = new Document(reader) ;//LoadDocument(reader);
                            if (activeTran == null || file.TransmittalId != activeTran.Id)
                            {
                                if (activeTran != null)
                                {
                                    res.Add(activeTran);
                                    e.Report(activeTran);
                                }

                                activeTran = new Transmittal(reader);// LoadTransmittal(reader);
                            }
                            activeTran.Files.Add(file);
                        }
                    }
                    else
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            Transmittal tran = new Transmittal(reader);// LoadTransmittal(reader);
                            res.Add(tran);
                            e.Report(tran);
                            if (cancellationToken.IsCancellationRequested)
                            {
                                reader.Close();
                                connection.Close();
                                return null;
                            }
                        }
                    }

                }
            }
            e.Report(null);
            return res;
        }
        private string AddParameters(SearchViewModel search)
        {
            string query = "";
            if (!string.IsNullOrEmpty(search.ProjectNo))
                query += $"and t.ProjectNo like {search.ProjectNo}";

            if (!string.IsNullOrEmpty(search.IssueBy))
                query += $"and t.IssueBy like {search.IssueBy}";

            if (search.SentByDate != null)
                query += $"and t.SentDate < {search.SentByDate}";

            if (search.SentAfterDate != null)
                query += $"and t.SentDate > {search.SentAfterDate}";

            if (search.IssueType != null && search.IssueType != 0)
                query += $"and t.IssueType  = {(int)search.IssueType}";

            if (search.IssueToWorkshop != null)
                query += $"and t.ToWorkShop = {search.IssueToWorkshop}";

            return query;
        }
        //private static Transmittal LoadTransmittal(SqlDataReader reader)
        //{
        //    Transmittal tran = new Transmittal();
            
        //    tran.Id = (int)reader["Id"];
        //    tran.ProjectNo = reader["ProjectNo"].ToString();
        //    tran.Recipients = reader["Recipients"].ToString();
        //    tran.SentDate = (DateTime)reader["SentDate"];
        //    tran.IssueBy = reader["IssuedBy"].ToString();
        //    tran.CreatedBy = reader["CreatedBy"].ToString();
        //    tran.IssueType = (IssueType)reader["IssueType"];
        //    tran.TransmittalStatus = (TransmittalStatus)reader["Status"];
        //    tran.IssueToWorkshop = (bool)reader["ToWorkShop"];

        //    return tran;
        //}
        //internal static Document LoadDocument (SqlDataReader reader)
        //{
        //    var file = new  Document();
        //    file.IsLoadedFromDb = true;
        //    file.Id = (int) reader["Id"];
        //    file.TransmittalId = (int) reader["TransId"];
        //    file.FileName = (string) reader["FileName"];
        //    file.FileId = (int) reader["FileId"];
        //    file.Version = (int) reader["Version"];
        //    file.Revision = (string) reader["Revision"];
        //    return file;
        //}
    }
}
