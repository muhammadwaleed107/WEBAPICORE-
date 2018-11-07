using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace BolNMS.Mobile.Repository
{
    public class Repository
    {
        protected delegate T TFromDataRow<T>(SqlDataReader dr);
        protected static async Task<List<T>> CollectionFromDataSet<T>(SqlDataReader reader, TFromDataRow<T> action)
        {
            List<T> list = null;
            if (reader.HasRows)
            {
                list = new List<T>();
                while (await reader.ReadAsync())
                {
                    list.Add(action(reader));
                }
            }


            return list;
        }

        protected async Task<List<T>> ExecuteDataSet<T>(string connectionString, CommandType type, TFromDataRow<T> action, string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandType = type;
                command.CommandText = query;
                if (parameters != null)
                {
                    foreach (var paramter in parameters)
                    {
                        command.Parameters.Add(paramter);
                    }

                }
                await connection.OpenAsync();
                var reader = await command.ExecuteReaderAsync();

                return await CollectionFromDataSet<T>(reader, action);
            }

        }


        public static async Task<object> ExecuteScalar(string connectionString, CommandType type, string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandType = type;
                command.CommandText = query;
                if (parameters != null)
                {
                    foreach (var paramter in parameters)
                    {
                        command.Parameters.Add(paramter);
                    }

                }
                await connection.OpenAsync();
                var ret =await command.ExecuteScalarAsync();
                return ret;

            }
        }

        public static async Task<int> ExecuteNonQuery(string connectionString, CommandType type, string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandType = type;
                command.CommandText = query;
                if (parameters != null)
                {
                    foreach (var paramter in parameters)
                    {
                        command.Parameters.Add(paramter);
                    }

                }
                await connection.OpenAsync();
                var ret = await command.ExecuteNonQueryAsync();
                return ret;

            }
        }
    }
}
