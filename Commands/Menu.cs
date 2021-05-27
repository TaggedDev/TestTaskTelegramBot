using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TestTaskTelegramBot.Service;

namespace TestTaskTelegramBot.Commands
{
    class Menu
    {
        /// <summary>
        /// Main restaurant menu message function
        /// </summary>
        public async static void StartMenu(long chatId)
        {
            string textMessage = "Используйте кнопки для навигации по меню.";
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                           {
                                new[] { InlineKeyboardButton.WithCallbackData("Основные блюда", "menu:main_course") },
                                new[] { InlineKeyboardButton.WithCallbackData("Десерты", "menu:dessert") }
                            });
            await Bot.Get().SendTextMessageAsync(chatId: chatId, text: textMessage, replyMarkup: inlineKeyboard);
        }

        /// <summary>
        /// Function sends user the list of all the dishes in the category 
        /// </summary>
        public async static void SendDishesCategory(ChatId chatId, string name)
        {
            List<Dish> dishes = GetDishes(name);
            
            for (int i = 0; i < dishes.Count; i++)
            {
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                               {
                                new[] { InlineKeyboardButton.WithCallbackData("Добавить в корзину", $"menu:add_to_cart{dishes[i].ItemId}") },
                                new[] { InlineKeyboardButton.WithCallbackData("Назад", $"start:menu") },
                                new[] { InlineKeyboardButton.WithCallbackData("Посмотреть корзину", $"cart:overview") }
                            });

                await Bot.Get().SendPhotoAsync(
                    chatId: chatId,
                    photo: dishes[i].Link,
                    caption: $"<b>{dishes[i].Name}</b>\n{dishes[i].Description}\n\nПриблизительное время приготовления: <i>{dishes[i].CookingTime}</i>\nЦена: <i>{dishes[i].Price}</i>",
                    replyMarkup: inlineKeyboard,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
                    );
            }
        }

        /// <summary>
        /// Being used to create a list of dishes in one category
        /// </summary>
        /// <param name="name">Case-sensitive category name</param>
        /// <returns>Returns List of Dish and if finds nothing, returns empty list</returns>
        public static List<Dish> GetDishes(string name)
        {
            List<Dish> dishes = new List<Dish>();
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
                            string category = Convert.ToString(reader["category"]);

                            if (category.Equals(name))
                            {
                                string itemName = Convert.ToString(reader["name"]),
                                    description = Convert.ToString(reader["description"]),
                                           link = Convert.ToString(reader["picture"]),
                                    cookingTime = Convert.ToString(reader["cooking_time"]);
                                int  price = Convert.ToInt32(reader["price"]),
                                    itemId = Convert.ToInt32(reader["item_id"]);
                                Dish dish = new Dish(itemName, description, link, cookingTime, itemId, price);
                                dishes.Add(dish);
                            }       
                        }
                }
            }
            return dishes;
        }

        /// <summary>
        /// Adds dish to the cart by ID
        /// </summary>
        /// <param name="chatId">User's chat id</param>
        /// <param name="dishId">Id of the dish</param>
        public static void AddToCart(long chatId, string dishId, int messageId)
        {
            DatabaseHandler.AddItem(chatId, dishId);

            string[] itemsid = DatabaseHandler.GetCart(Convert.ToString(chatId)).Split(';'); // Splits the shopping cart into IDs
            List<Dish> dishes = Cart.GetDishesList(itemsid);

            int amount = 0;
            foreach (Dish item in dishes)
            {
                if (Convert.ToString(item.ItemId).Equals(dishId))
                    amount++;                
            }

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                               {
                                new[] { InlineKeyboardButton.WithCallbackData($"Добавить в корзину ({amount})", $"menu:add_to_cart{dishId}") },
                                new[] { InlineKeyboardButton.WithCallbackData("В меню", $"start:menu") },
                                new[] { InlineKeyboardButton.WithCallbackData("Посмотреть корзину", $"cart:overview") }
                            });

            Bot.Get().EditMessageReplyMarkupAsync(chatId, messageId, inlineKeyboard);
        }
    }
}
