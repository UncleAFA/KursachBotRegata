using System;
using System.Data;
using Npgsql;

namespace KursachBotRegata.Models
{
    public static class DBWorker
    {
        public static DataTable SelectCommand(string columns, string table, string complement)
        {   
            System.Console.WriteLine(@$"SELECT {columns} FROM {table} {complement}");
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

        public static void UpdateCommand(string columns, string table, string complement)
        {
            try
            {
                using (var conn = new NpgsqlConnection(AppSettings.ConnString))
                {
                    conn.Open();
                    using (var command = new NpgsqlCommand(@$"UPDATE {table} SET {columns} WHERE {complement}", conn))
                    {
                        command.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: DataBase - " + ex);
            }
        }

        public static void InsertCommand(string columns, string table, string values)
        {   System.Console.WriteLine(@$"INSERT INTO {table} {columns} VALUES ({values})");
            
            try
            {
                using (var conn = new NpgsqlConnection(AppSettings.ConnString))
                {
                    conn.Open();
                    using (var command = new NpgsqlCommand(@$"INSERT INTO {table} {columns} VALUES ({values})", conn))
                    {
                        command.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: DataBase - " + ex);
            }

            return;
        }

        public static void DeleteCommand(string table, string complement)
        {
            System.Console.WriteLine(@$"DELETE FROM {table} WHERE {complement}");
            
            try
            {
                using (var conn = new NpgsqlConnection(AppSettings.ConnString))
                {
                    conn.Open();
                    using (var command = new NpgsqlCommand(@$"DELETE FROM {table} WHERE {complement}", conn))
                    {
                        command.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: DataBase - " + ex);
            }

            return;
        }
    }
}