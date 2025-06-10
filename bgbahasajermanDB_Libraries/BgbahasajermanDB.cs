using MySql.Data.MySqlClient;
using System.Data;

namespace bgbahasajermanDB_Libraries
{
        public class bgbahasajermanDB
        {
            private MySqlConnection connection;
            private static bgbahasajermanDB instance;

            // Connection string as a private field
            public static string connectionString = "Server=localhost;Database=bgbahasajerman;User=root;Password=Burungnuri1212;";

            private bgbahasajermanDB()
            {
                connection = new MySqlConnection(connectionString);
            }

            public static bgbahasajermanDB GetInstance()
            {
                if (instance == null)
                {
                    instance = new bgbahasajermanDB();
                }
                return instance;
            }

            public MySqlConnection GetConnection()
            {
                return connection;
            }


            public MySqlConnection OpenConnection()
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                return connection;
            }

            public void CloseConnection()
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            public DataTable GetDataWithOpenConnection(string query)
            {
                OpenConnection();
                DataTable table = new DataTable();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                }

                CloseConnection();
                return table;
            }

            public DataTable GetData(string query)
            {
                DataTable table = new DataTable();

                using (MySqlConnection conn = OpenConnection())
                {
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            adapter.Fill(table);
                        }
                    }
                }

                return table;
            }

            // Execute a query that returns a SINGLE value of the specified data type
            public T ExecuteScalar<T>(string query)
            {
                using (MySqlConnection conn = OpenConnection())
                {
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            try
                            {
                                // Convert the result to the specified data type using generics
                                return (T)Convert.ChangeType(result, typeof(T));
                            }
                            catch (InvalidCastException)
                            {
                                // Handle invalid cast
                                throw new InvalidCastException($"Cannot cast result to {typeof(T).Name}");
                            }
                        }

                        // Return default value for reference types or 0 for value types if result is null
                        return default(T);
                    }
                }
            }

        public List<T> ExecuteScalarList<T>(string query)
        {
            List<T> resultList = new List<T>();

            using (MySqlConnection conn = OpenConnection())
            {
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                T value = reader.GetFieldValue<T>(0);
                                resultList.Add(value);
                            }
                        }
                    }
                }
            }

            return resultList;
        }


        public static T ExecuteScalarStatic<T>(string query)
            {
                //string connectionString = Global.CS; // Access the connection string
                using (MySqlConnection conn = new MySqlConnection(bgbahasajermanDB.connectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            try
                            {
                                // Convert the result to the specified data type using generics
                                return (T)Convert.ChangeType(result, typeof(T));
                            }
                            catch (InvalidCastException)
                            {
                                // Handle invalid cast
                                throw new InvalidCastException($"Cannot cast result to {typeof(T).Name}");
                            }
                        }

                        // Return default value for reference types or 0 for value types if result is null
                        return default(T);
                    }
                }
            }

            // Method to read SQL file and return its contents as a string
            public string ReadSqlFile(string filePath)
            {
                try
                {
                    // Check if the file exists
                    if (File.Exists(filePath))
                    {
                        // Read the contents of the file
                        string sqlContent = File.ReadAllText(filePath);
                        return sqlContent;
                    }
                    else
                    {
                        throw new FileNotFoundException("SQL file not found.");
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions (e.g., file not found, permissions, etc.)
                    throw new Exception($"Error reading SQL file: {ex.Message}");
                }
            }

            public void ExecuteWithConnection(Action<MySqlConnection> action)
            {
                using (MySqlConnection conn = OpenConnection())
                {
                    action(conn); // Execute the action with the opened connection
                }
            }

            // Example usage:
            // db.ExecuteWithConnection(connection =>
            // {
            //     using (MySqlCommand command = new MySqlCommand(query, connection))
            //     {
            //         // Execute your query or commands here
            //     }
            // });

            public void ExecuteNonQuery(string sql)
            {
                ExecuteWithConnection(conn =>
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                });
            }

            public List<T> GetDataAsList<T>(string query, Func<DataRow, T> mapper)
            {
                List<T> resultList = new List<T>();

                using (MySqlConnection conn = OpenConnection())
                {
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                T mappedObject = mapper(row);
                                resultList.Add(mappedObject);
                            }
                        }
                    }
                }

                return resultList;
            }


    }


}

