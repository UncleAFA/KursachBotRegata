using System;
using System.Data;
using Npgsql;

namespace KursachBotRegata.Models
{
    public static class DBWorker
    {
        public static DataTable SelectCommand(string columns, string table, string complement)
        {   
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new NpgsqlConnection(AppSettings.ConnString))
                {
                    conn.Open();
                    using (var command = new NpgsqlCommand(@$"SELECT {columns} FROM {table} {complement}", conn))
                    {
                        NpgsqlDataReader reader = command.ExecuteReader();
                        dt.Load(reader);
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: DataBase - " + ex);
            }
            
            return dt;
        }
    }
}