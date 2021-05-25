using Microsoft.Data.Sqlite;

namespace TestTaskTelegramBot.Service
{
    class DatabaseHandler
    {
        public static void AddItem(long chatId, string dishId)
        {
            ExecuteSQL("");
        }

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
