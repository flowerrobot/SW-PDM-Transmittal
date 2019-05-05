using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransmittalManager.SQL_Queries;
using TransmittalManager.ViewModel;

namespace TransmittalManager.Models
{
    public class Recipient
    {
        static List<Recipient> _allRecipients;
        public static List<Recipient> AllRecipients
        {
            get {
                if (_allRecipients != null) return _allRecipients;
                CancellationTokenSource ctk = new CancellationTokenSource();
                LoadRecipients(ctk.Token).Wait();
                return _allRecipients;
            }
        }

     static   Dictionary<int, CompanyViewModel> _allCompanies;
        public static Dictionary<int, CompanyViewModel> AllCompanies
        {
            get {
                if (_allCompanies != null) return _allCompanies;
                CancellationTokenSource ctk = new CancellationTokenSource();
                LoadCompanies(ctk.Token).Wait();
                return _allCompanies;
            }
        }


        internal static async Task LoadRecipients(CancellationToken ct)
        {
            //CancellationTokenSource ctk = new CancellationTokenSource();

            string query = string.Format(SqlScripts.GetAllRecipients);
            using (SqlConnection connection = new SqlConnection(TransmitalManager.connString))
            {
                var t = connection.OpenAsync(ct);
                _allRecipients = new List<Recipient>();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await t;
                    var reader = await command.ExecuteReaderAsync(ct);
                    while (await reader.ReadAsync(ct) && !ct.IsCancellationRequested)
                    {
                        int id = (int)reader["Id"];
                        var grp = (string)reader["CompanyName"];
                        Recipient cvp = new Recipient();
                        cvp.Id = id;
                       if(AllCompanies.TryGetValue((int)reader["CompanyId"], out CompanyViewModel tt))
                        {
                            cvp.Company = tt;
                        }
                        cvp.Name = (string)reader["Name"];
                        cvp.Email = (string)reader["Email"];



                        _allRecipients.Add(cvp);
                    }
                    return;
                }
            }
        }
       internal static async Task LoadCompanies(CancellationToken ct)
        {
            //CancellationTokenSource ctk = new CancellationTokenSource();
            
            string query = string.Format(SqlScripts.GetAllCompanies);
            using (SqlConnection connection = new SqlConnection(TransmitalManager.connString))
            {
                var t = connection.OpenAsync(ct);
                _allCompanies = new Dictionary<int, CompanyViewModel>();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await t;
                    var reader = await command.ExecuteReaderAsync(ct);
                    while (await reader.ReadAsync(ct) && !ct.IsCancellationRequested)
                    {
                        int id = (int)reader["Id"];
                        var grp = (string)reader["CompanyName"];
                        CompanyViewModel cvp = new CompanyViewModel() { Id = id, Name = grp };
                        _allCompanies.Add(id, cvp);
                    }
                    return;
                } 
            }
        }


        public int Id { get; set; }
        public CompanyViewModel Company { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }        
    }
}
