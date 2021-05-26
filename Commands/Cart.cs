using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using TestTaskTelegramBot.Service;

namespace TestTaskTelegramBot.Commands
{
    class Cart
    {
        /// <summary>
        /// Function sends user his shopping cart list with the list of each item and 4 buttons
        /// </summary>
        /// <param name="chatId">User chatId</param>
        public async static void Overview(long chatId)
        {
            List<Dish> dishes = new List<Dish>();
            string[] itemsid = DatabaseHandler.GetCart(Convert.ToString(chatId)).Split(';'); // Splits the shopping cart into IDs

            // Gets every Id and creates dish out of it
            foreach (string item in itemsid)
            {
                if (item != "")
                {
                    Dish dish = new Dish(item);
                    if (dishes.Any(obj => Convert.ToString(obj.ItemId).Equals(item)))
                        dishes.Find(obj => Convert.ToString(obj.ItemId).Equals(item)).Amount += 1; // if there is an object like that one, adds amount instead of creating new item
                    else
                        dishes.Add(dish);
                }
                
            }

            string textMessage = "Ваша корзина: \n";
            int price = 0;

            // Generating the output message
            foreach (Dish dish in dishes)
            {
                textMessage += $"\n*{dish.Name}* - {dish.Price} [`{dish.Amount}`]";
                price += dish.Price * dish.Amount;
            }
            textMessage += $"\n\nИтого: *{price}₽.*";

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                               {
                                new[] { InlineKeyboardButton.WithCallbackData("В меню", "start:menu") },
                                new[] { InlineKeyboardButton.WithCallbackData("Очистить корзину", "cart:empty") },
                                new[] { InlineKeyboardButton.WithCallbackData("Удалить блюдо", "cart:delete") },
                                new[] { InlineKeyboardButton.WithCallbackData("Оформить заказ", "cart:finish") }
                            });

            await Bot.Get().SendTextMessageAsync(chatId: chatId, text: textMessage, replyMarkup: inlineKeyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }

        public async static void Delete(long chatId)
        {
            string textMessage = "Выберите элемент для удаления\n";
            
            List<Dish> dishes = new List<Dish>();
            string[] itemsid = DatabaseHandler.GetCart(Convert.ToString(chatId)).Split(';');

            foreach (string item in itemsid)
            {
                Dish dish = new Dish(item);
                dishes.Add(dish);
            }

            for (int i = 0; i < dishes.Count/8; i++)
            {
                textMessage += $"{i+1}. {dishes[i].Name}\n";
            }
            textMessage += $"\nНажмите кнопку *закончить*, чтобы закончить редактировать заказ";

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                               {
                                new[] { InlineKeyboardButton.WithCallbackData("1", "cart"),
                                        InlineKeyboardButton.WithCallbackData("2", "start:menu")},
                                new[] { InlineKeyboardButton.WithCallbackData("3", "cart:empty"),
                                        InlineKeyboardButton.WithCallbackData("4", "cart:empty")},
                                new[] { InlineKeyboardButton.WithCallbackData("5", "cart:delete"),
                                        InlineKeyboardButton.WithCallbackData("6", "cart:delete")},
                                new[] { InlineKeyboardButton.WithCallbackData("7", "cart:finish"),
                                        InlineKeyboardButton.WithCallbackData("8", "cart:finish")},
                                new[] { InlineKeyboardButton.WithCallbackData("Назад", "cart:finish"),
                                        InlineKeyboardButton.WithCallbackData("Вперёд", "cart:finish")},
                                new[] { InlineKeyboardButton.WithCallbackData("Закончить", "cart:finish") }
                            });

            await Bot.Get().SendTextMessageAsync(chatId, textMessage, replyMarkup: inlineKeyboard);
        }

        /// <summary>
        /// Finishes the order, sends message and empties the cart
        /// </summary>
        /// <param name="chatId">User's chat ID</param>
        public async static void Finish(long chatId)
        {
            DatabaseHandler.ExecuteSQL($"UPDATE users SET cart = \'\' WHERE chat_id={chatId}");
            string messageText = "Заказ оформлен";
            await Bot.Get().SendTextMessageAsync(chatId: chatId, text: messageText);
        }

        /// <summary>
        /// Empties the shopping cart
        /// </summary>
        /// <param name="chatId">User's chat ID</param>
        public async static void Empty(long chatId)
        {
            DatabaseHandler.ExecuteSQL($"UPDATE users SET cart = \'\' WHERE chat_id={chatId}");
            string messageText = "Корзина очищена.";
            await Bot.Get().SendTextMessageAsync(chatId: chatId, text: messageText);
        }
    }
}
