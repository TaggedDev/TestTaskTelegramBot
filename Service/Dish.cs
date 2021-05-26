using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestTaskTelegramBot.Service
{
    class Dish
    {
        private string _name, _description, _link, _cookingTime;
        private int _itemId, _price;

        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public string Link { get => _link; set => _link = value; }
        public string CookingTime { get => _cookingTime; set => _cookingTime = value; }
        public int ItemId{ get => _itemId; set => _itemId = value; }
        public int Price { get => _price; set => _price = value; }

        public Dish(string name, string description, string link, string cookingTime, int itemId, int price)
        {
            Name = name;
            Description = description;
            Link = link;
            CookingTime = cookingTime;
            ItemId = itemId;
            Price = price;
        }

        public Dish(string id)
        {
            using (var connection = new SqliteConnection("Data Source=chef.db"))
            {
                connection.Open();
                string sqlExpression = $"SELECT * FROM items";
                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // If there is data
                        while (reader.Read())   // Reads data by lines
                        {
                            string getId = Convert.ToString(reader["item_id"]);

                            if (getId.Equals(id))
                            {
                                Name = Convert.ToString(reader["name"]);
                                Description = Convert.ToString(reader["description"]);
                                Link = Convert.ToString(reader["picture"]);
                                CookingTime = Convert.ToString(reader["cooking_time"]);
                                Price = Convert.ToInt32(reader["price"]);
                                ItemId = Convert.ToInt32(reader["item_id"]);
                            }
                        }
                }
            }

        }
    }
}
