using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using TransmittalManager.SQL_Queries;

namespace TransmittalManager.Models
{
    /// <summary>
    /// This represents a document as the raw data
    /// </summary>
    public class Document
    {
        public Document() { }

        public Document(string fileName)
        {
            FileName = fileName;
        }
        internal Document(SqlDataReader reader)
        {
            IsLoadedFromDb = true;
            Id = (int)reader["Id"];
            TransmittalId = (int)reader["TransId"];
            FileName = (string)reader["FileName"];
            FileId = (int)reader["FileId"];
            Version = (int)reader["Version"];
            Revision = (string)reader["Revision"];
        }

#if BlockPDM

#endif

        public int Id { get; set; }
        public int TransmittalId { get; set; }
        public string FileName { get; set; }
        public int FileId { get; set; }
        public int Version { get; set; }
        public string Revision { get; set; }
        public string FileState { get; set; }
        public string Description { get; set; }
        public int TotalQty { get; set; }
        public bool IsLoadedFromDb { get; set; }
        public bool RemoveFromDb { get; set; }



        public async Task dbQueryUpdate(SqlConnection connection)
        {

            //{
            //    string updates = $"FileName = {FileName}" +
            //                    $"Version = {Version}" +
            //                    $"Revision = {Revision}";
            //    string id = $"TransId = {TransmittalId} and FileId = {FileId}";


            string query = string.Format(SqlScripts.UpdateFile, Id.ToString(), TransmittalId.ToString());
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FileName", FileName);
                command.Parameters.AddWithValue("@id", FileId);
                command.Parameters.AddWithValue("@Ver", Version);
                command.Parameters.AddWithValue("@Rev", Revision);

                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task dbQueryInsert(SqlConnection connection)
        {
            try
            {
                //    string header2 = "TransId, FileName, FileId, Version, Revision";
                //    string values2 = $"{TransmittalId},'{FileName}',{FileId},  {Version}, {Revision}";
                string query = string.Format(SqlScripts.AddFilesToTransmittal);


                //string query = $"insert into TransFiles(TransId, FileName, FileId, Version, Revision)" +
                //               $"values({TransmittalId}, '{FileName ?? "-"}', {FileId}, {Version}, {Revision ?? "-"})" +
                //               $"SELECT SCOPE_IDENTITY();";



                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Tid",  TransmittalId);
                    command.Parameters.AddWithValue("@FileName", FileName ?? "-");
                    command.Parameters.AddWithValue("@fileId", FileId);
                    command.Parameters.AddWithValue("@Ver", Version);
                    command.Parameters.AddWithValue("@Rev", Revision ?? "-");
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }else if (connection.State == ConnectionState.Open)
                    {
#if DEBUG
                        Debugger.Break();               
#endif
         
                    }
                    object res = await command.ExecuteScalarAsync();
                    if (res is decimal d)
                        Id = Convert.ToInt32(d);
                    else if (res is int i)
                        Id = i;

                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
            }
 
        }
    }
}
