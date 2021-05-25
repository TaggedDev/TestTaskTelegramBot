using Microsoft.Data.Sqlite;
using System;

namespace TestTaskTelegramBot.Service
{
    class DatabaseHandler
    {
        /// <summary>
        /// Updates user's shopping cart
        /// </summary>
        /// <param name="chatId">User's chat id</param>
        /// <param name="dishId">Id of the dish</param>
        public static void AddItem(long chatId, string dishId)
        {
            string userCart = GetCart(chatId);
            userCart = userCart + ";" + dishId;
            ExecuteSQL($"UPDATE users SET cart = \'{userCart}\'");
        }

        /// <summary>
        /// Returns the cart from the users in chef.DB
        /// </summary>
        /// <param name="chatId">User's chat id</param>
        private static string GetCart(long chatId)
        {
            using (var connection = new SqliteConnection("Data Source=chef.db"))
            {
                connection.Open();
                string sqlExpression = $"SELECT * FROM users";
                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                        while (reader.Read())   // построчно считываем данные
                        {
                            string getId = Convert.ToString(reader[$"{chatId}"]);

                            if (getId.Equals(chatId))
                                return Convert.ToString(reader["cart"]);
                        }
                }
            }
            return null;
        }

        /// <summary>
        /// Executes command to the chef.DB
        /// </summary>
        /// <param name="cmd">Command to execute</param>
        public static void ExecuteSQL(string cmd)
        {
            using (var connection = new SqliteConnection("Data Source=chef.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand
                {
                    Connection = connection,
                    CommandText = cmd
                };
                command.ExecuteNonQuery();
            }
        }
    }
}
