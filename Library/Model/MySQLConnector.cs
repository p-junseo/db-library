using MySql.Data.MySqlClient;
using System.Text;
using DotNetEnv;

namespace LibraryWithAPI
{
    internal class MySQLConnector
    {
        private static string GetConnectionAddress()
        {
            Env.Load();
            string _server = Environment.GetEnvironmentVariable("DATABASE_SERVER");
            int _port = int.Parse(Environment.GetEnvironmentVariable("DATABASE_PORT"));
            string _database = Environment.GetEnvironmentVariable("DATABASE_DATABASE");
            string _id = Environment.GetEnvironmentVariable("DATABASE_ID");
            string _pw = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");

            return $"Server={_server};Port={_port};Database={_database};Uid={_id};Pwd={_pw};Charset=utf8mb4;";
        }

        public static int InsertInto(string tableName, string[] value)
        {
            
            MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionAddress());

            mySqlConnection.Open();

            string valueQuery = ConvertToQuery(value);
            string insertQuery = $"INSERT INTO {tableName} VALUE ({valueQuery})";

            MySqlCommand command = new MySqlCommand(insertQuery, mySqlConnection);

            int result = command.ExecuteNonQuery();

            mySqlConnection.Close();

            return result;
        }

        public static int UpdateSetWhere(string tableName, string[] attribute, string[] value, string conditionQuery)
        {
            MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionAddress());

            mySqlConnection.Open();

            string setQuery = ConvertToQuery(attribute, value);
            string updateQuery = $"UPDATE {tableName} SET {setQuery} {conditionQuery}";

            MySqlCommand command = new MySqlCommand(updateQuery, mySqlConnection);

            int result = command.ExecuteNonQuery();

            mySqlConnection.Close();

            return result;
        }

        public static int DeleteFromWhere(string tableName, string conditionQuery)
        {
            MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionAddress());

            mySqlConnection.Open();

            string deleteQuery = $"DELETE FROM {tableName} {conditionQuery}";
            MySqlCommand command = new MySqlCommand(deleteQuery, mySqlConnection);

            int result = command.ExecuteNonQuery();

            mySqlConnection.Close();

            return result;
        }

        public static List<string[]> SelectFromWhere(string tableName, string[] attribute, string conditionQuery)
        {
            List<string[]> content = new List<string[]>();

            MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionAddress());

            mySqlConnection.Open();

            string attributeQuery = ConvertToQuery(attribute);
            string selectQuery = $"SELECT {attributeQuery} FROM {tableName} {conditionQuery}";

            MySqlCommand command = new MySqlCommand(selectQuery, mySqlConnection);

            MySqlDataReader table = command.ExecuteReader();

            while (table.Read())
            {
                string[] row = new string[attribute.Length];
                for (int i = 0; i < attribute.Length; i++)
                {
                    row[i] = table[attribute[i]].ToString();
                }
                content.Add(row);
            }

            table.Close();

            mySqlConnection.Close();

            return content;
        }

        public static int SetUpdateSet(string tableName, string attribute)
        {
            MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionAddress());

            mySqlConnection.Open();

            string setQuery = $"SET @count = 0; UPDATE {tableName} SET {attribute} = @count := @count + 1";
            MySqlCommand command = new MySqlCommand(setQuery, mySqlConnection);

            int result = command.ExecuteNonQuery();

            mySqlConnection.Close();

            return result;
        }

        private static string ConvertToQuery(string[] attribute)
        {
            StringBuilder query = new StringBuilder();

            for (int i = 0; i < attribute.Length - 1; i++)
            {
                query.Append($"{attribute[i]}, ");
            }
            query.Append(attribute[attribute.Length - 1]);

            return query.ToString();
        }

        private static string ConvertToQuery(string[] conditionAttribute, string[] conditionValue)
        {
            StringBuilder query = new StringBuilder();

            for (int i = 0; i < conditionAttribute.Length - 1; i++)
            {
                query.Append($"{conditionAttribute[i]} = {conditionValue[i]}, ");
            }
            query.Append($"{conditionAttribute[conditionAttribute.Length - 1]} = {conditionValue[conditionValue.Length - 1]}");

            return query.ToString();
        }
    }
}