﻿using Syncfusion.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransmittalManager.SQL_Queries;
using TransmittalManager.ViewModel;

namespace TransmittalManager.Models
{
    public class Transmittal
    {
        public Transmittal()
        {
            Comments = "Document Transmital";
            Comments += "\n\nThe following files have been sent to you for <status>";
            Comments += "\nFiles issued by <issuer> on <issueDate>";

            CreatedBy = User.ActiveUser;
        }
        public Transmittal(SqlDataReader reader)
        {
            try
            {
                Id = (int)reader["Id"];
                //Project = reader["ProjectNo"].ToString();
                int proId = (int)reader["ProjectNo"];
                if (proId != 0)
                {
                    Project = TransmittalManager.Models.Project.AllProjects.FirstOrDefault(t => t.Number == proId);
                }
                //Recipients = reader["Recipients"].ToString();
                //SentDate = reader["SentDate"] as DateTime; 
                object test = reader["SentDate"] ; 
                //TODO fix sent date

                int crById = (int)reader["CreatedBy"];
                CreatedBy = User.AllUsers()[crById];

                object issById = reader["IssueBy"];
                if (issById != null && issById is int iss)
                {
                    IssueBy = User.AllUsers()[(int)iss];
                }

                Comments = reader["Comments"]?.ToString();
                IssueType = (IssueType)reader["IssueType"];
                TransmittalStatus = (TransmittalStatus)reader["Status"];
                IssueToWorkshop = (int)reader["ToWorkShop"] == 1;
            }
            catch (Exception ex)
            {
#if  DEBUG
                Debugger.Break();

#endif
            }
        }


        public bool IsLoadedFromDb => Id != null;
        public bool ExtendedDataLoaded { get; set; }


        public int? Id { get; set; }


        public bool IssueToWorkshop { get; set; }
        public string WorkShopJtsk { get; set; }


        public List<Recipient> Recipients { get; }
        public DateTime? SentDate { get; set; }
        public IssueType IssueType { get; set; }
        public TransmittalStatus TransmittalStatus { get; set; }
        public string Comments { get; set; }
        public User IssueBy { get; set; }
        public User CreatedBy { get; set; }
        public Project Project { get; set; }


        public List<Document> Files { get; set; } = new List<Document>();
        public async Task<bool> LoadExtendedDataAsync(IProgress<Document> e, CancellationToken cancellationToken)
        {
            if (IsLoadedFromDb != true) return true; //its not from the database so nothing to load

            List<Document> docs = new List<Document>();
            using (SqlConnection connection = new SqlConnection(TransmitalManager.connString))
            {
                string query = string.Format(SqlScripts.GetTransmittalFiles, Id);
                SqlCommand command = new SqlCommand(query, connection);
                await connection.OpenAsync();

                //***** Load recipients *****
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Document file = new Document(reader);// TransmitalManager.LoadDocument(reader);
                        e?.Report(file);
                        docs.Add(file);
                        if (cancellationToken.IsCancellationRequested)
                        {
                            reader.Close();
                            connection.Close();
                            return false;
                        }
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    connection.Close();
                    return false;
                }

                //***** Get Recipients *****
                query = string.Format(SqlScripts.GetTransmitalRecipients, Id);
                command = new SqlCommand(query, connection);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int Id = (int)reader["RecipientId"];
                        Recipient file = Recipient.AllRecipients.FirstOrDefault(t => t.Id == Id);
                        if (file != null)
                            Recipients.Add(file);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            reader.Close();
                            connection.Close();
                            return false;
                        }
                    }
                }


            }

            ExtendedDataLoaded = true;
            return true;
        }


         
        public async Task<bool> Save(TransmittalViewModel vm,bool UpdateToIssued)
        {
            if (UpdateToIssued)
            {
                TransmittalStatus = TransmittalStatus.Issued;
                SentDate = DateTime.Today;
            }

            using (SqlConnection connection = new SqlConnection(TransmitalManager.connString))
            {
                Task cmd = connection.OpenAsync();
                Task<int> t;
                

                if (IsLoadedFromDb)///update
                {
                    string updates = "";
                    if (vm.IssueToWorkshop != IssueToWorkshop) updates += $"ToWorkShop = {Convert.ToInt32(vm.IssueToWorkshop)}";
                    if (vm.IssueType != IssueType) updates += $"IssueType = {vm.IssueType}";
                    if (vm.TransmittalStatus != TransmittalStatus) updates += $"Status = {vm.TransmittalStatus}";
                    if (vm.Comments != Comments) updates += $"Comments = {vm.Comments}";
                    if (vm.Project.Number != Project.Number) updates += $"ProjectNo = {vm.Project.Number}";
                    if (vm.IssueBy?.Id != IssueBy.Id) updates += $"IssuedBy = {vm.IssueBy}";
                    if (vm.CreatedBy?.Id != CreatedBy.Id) updates += $"CreatedBy= {vm.CreatedBy.Id}";


                    string query = string.Format(SqlScripts.UpdateTransmittal, Id, updates);
                    SqlCommand command = new SqlCommand(query, connection);
                    await cmd;
                    t = command.ExecuteNonQueryAsync();


                    ///**** Update Transmittal Recipients *****
                    
                    //TODO Not remove all recipients, check if they need to be removed to stop thrashing.
                    query = string.Format(SqlScripts.DeleteTransmitalRecipients, Id);  //Remove all recipients  
                    Task<int> rr =  new SqlCommand(query, connection).ExecuteNonQueryAsync();
                    await rr;                    
                    //Add them
                    Parallel.ForEach(Recipients, async rec => await new SqlCommand(string.Format(SqlScripts.AddTransmitalRecipients, Id, rec.Id), connection).ExecuteNonQueryAsync());

                    //**** Update Files Lists ****
                    List<Document> toRemove = Files.ToArray().ToList();
                    List<FileDataViewModel> toAdd = new List<FileDataViewModel>();

                    //Files to add
                    foreach (FileDataViewModel fileVm in vm.Files)
                    {
                        Document fnd = Files.FirstOrDefault(a => fileVm.Id == a.Id);
                        if (fnd == null)
                            toAdd.Add(fileVm);
                    }
                    //Files to remove
                    foreach (Document file in Files)
                    {
                        FileDataViewModel fnd = vm.Files.FirstOrDefault(a => file.Id == a.Id);
                        if (fnd == null)
                            toRemove.Remove(file);
                    }

                    //remove files
                    string removeId = "";
                    toRemove.ForEach(r => removeId += (r.Id + ","));
                    string remove = string.Format(SqlScripts.RemoveFiles, removeId.TrimEnd(','));
                    Task<int> t4 = new SqlCommand(remove, connection).ExecuteNonQueryAsync();

                    //Add files
                    Parallel.ForEach(toAdd, async doc => await doc.Model.dbQueryInsert(connection));
                    return true;
                }
                else //add new
                {
                    try
                    {
                        Dictionary<string, string> updates = new Dictionary<string, string>();


                        updates.Add("ToWorkShop", Convert.ToInt32(vm.IssueToWorkshop).ToString());
                        updates.Add("IssueType", ((int)vm.IssueType).ToString());
                        updates.Add("Status", ((int)vm.TransmittalStatus).ToString());
                        if (!string.IsNullOrEmpty(vm.Comments)) updates.Add("Comments", "'" + vm.Comments + "'");
                        if (vm.Project != null) updates.Add("ProjectNo", vm.Project.Number.ToString());
                        if (vm.IssueBy != null) updates.Add("IssueBy", "'" + vm.IssueBy + "'");
                        if (vm.CreatedBy?.Id != CreatedBy.Id) updates.Add("CreatedBy", vm.CreatedBy?.Id.ToString() ?? User.ActiveUser.Id.ToString());



                        string header = "", values = "";
                        foreach (KeyValuePair<string, string> v in updates)
                        {
                            header += v.Key + ",";
                            values += v.Value + ",";
                        }

                        header = header.Trim(',');
                        values = values.Trim(',');

                        string query = string.Format(SqlScripts.NewTransmittal, header, values);
                        SqlCommand command = new SqlCommand(query, connection);
                        await cmd;

                        //Get id from new added record
                        object id = await command.ExecuteScalarAsync();

                        if (id is decimal d)
                            Id = Convert.ToInt32(d);
                        else if (id is int i)
                            Id = i;

                        if (Id != null)
                        {


                            ///**** Add Transmittal Recipients *****                                              
                            Parallel.ForEach(Recipients, async rec => await new SqlCommand(string.Format(SqlScripts.AddTransmitalRecipients, Id, rec.Id), connection).ExecuteNonQueryAsync());

                             //**** Update Files Lists ****
                            foreach (var doc in vm.Files)
                            {
                                doc.Model.TransmittalId = Id ?? 1;
                                await doc.Model.dbQueryInsert(connection);
                            }

                            //Add files
                            //    Parallel.ForEach(vm.Files, doc =>
                            //{
                            //    doc.Model.TransmittalId = Id ?? 1;
                            //    doc.Model.dbQueryInsert(connection);
                            //    //    string header2 = "TransId, FileName, FileId, Version, Revision";
                            //    //    string values2 = $"{Id}, {doc.Name}, {doc.Id}, {doc.Version}, {doc.Revision}";
                            //    //    string query2 = string.Format(SqlScripts.AddFilesToTransmittal, header2, values2);
                            //    //    SqlCommand command2 = new SqlCommand(query2, connection);
                            //    //    command2.ExecuteNonQueryAsync();
                            //});
                        }

                        return true;
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debugger.Break();
#endif
                        return false;
                    }

                }
            }
        }

    }
}
