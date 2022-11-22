using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Nodes;

namespace FoodPenguinAPI
{
    public class Database
    {
        //Connection
        MySqlConnection con = new MySqlConnection("SERVER=LOCALHOST; DATABASE=foodpenguin; UID=root;CHARSET=utf8");

        public (string data, int row) query_select(string sql, Boolean singleValue = false)
        {
            try
            {
                con.Open();
                MySqlCommand query = new MySqlCommand(sql, con);
                MySqlDataReader reader = query.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                con.Close();

                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(dataTable);

                int dataAmount = Convert.ToInt32(dataTable.Rows.Count);

                if (dataAmount == 1 && singleValue)
                {
                    return (data: JSONString.Substring(1, JSONString.Length - 2), row: dataAmount);
                }
                return (data: JSONString, row: dataAmount);
            }
            catch (Exception ex)
            {
                var json = new JsonObject();
                json.Add("API_Message", ex.Message);
                Console.WriteLine(json.ToString());
                return (data: json.ToString(), row: 0);
            }
        }

        public (Boolean status, string data) query_update(string sql)
        {
            try
            {
                con.Open();
                MySqlCommand query = new MySqlCommand(sql, con);
                MySqlDataReader reader = query.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                con.Close();

                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(dataTable);

                return (status: true, data: JSONString);
            }
            catch (Exception ex)
            {
                var json = new JsonObject();
                json.Add("API_Message", ex.Message);
                Console.WriteLine(json.ToString());
                return (status: false, data: json.ToString());
            }
        }

        public (Boolean status, string data) query_insert(string sql)
        {
            try
            {
                con.Open();
                MySqlCommand dbcmd = con.CreateCommand();
                dbcmd.CommandText = sql;
                dbcmd.ExecuteNonQuery();
                long imageId = dbcmd.LastInsertedId;
                con.Close();

                return (status: true, data: imageId.ToString());
            }
            catch (Exception ex)
            {
                var json = new JsonObject();
                json.Add("API_Message", ex.Message);
                Console.WriteLine(json.ToString());
                return (status: false, data: json.ToString());
            }
        }

        public (Boolean status, string data) query_delete(string sql)
        {
            try
            {
                con.Open();
                MySqlCommand query = new MySqlCommand(sql, con);
                MySqlDataReader reader = query.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                con.Close();

                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(dataTable);

                return (status: true, data: JSONString);
            }
            catch (Exception ex)
            {
                var json = new JsonObject();
                json.Add("API_Message", ex.Message);
                Console.WriteLine(json.ToString());
                return (status: false, data: json.ToString());
            }
        }
    }
}
