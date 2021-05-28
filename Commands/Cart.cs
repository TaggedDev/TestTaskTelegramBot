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

            string textMessage = "<b>Ваша корзина:</b>\n";
            textMessage += GetCartMessage(chatId);

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                               {
                                new[] { InlineKeyboardButton.WithCallbackData("📖 В меню", "start:menu") },
                                new[] { InlineKeyboardButton.WithCallbackData("📭 Очистить корзину", "cart:empty") },
                                new[] { InlineKeyboardButton.WithCallbackData("✏️ Редактировать заказ", "cart:delete") },
                                new[] { InlineKeyboardButton.WithCallbackData("✅ Оформить заказ", "cart:finish") }
                            });

            await Bot.Get().SendTextMessageAsync(chatId: chatId, text: textMessage, replyMarkup: inlineKeyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        /// <summary>
        /// Used to generate and return shopping cart message
        /// </summary>
        /// <param name="chatId">Users' chat ID</param>
        /// <returns>Returns finished message with cart</returns>
        private static string GetCartMessage(long chatId)
        {
            string[] itemsid = DatabaseHandler.GetCart(Convert.ToString(chatId)).Split(';'); // Splits the shopping cart into IDs

            // Gets every Id and creates dish out of it
            List<Dish> dishes = GetDishesList(itemsid);

            string textMessage = "";
            int price = 0;

            // Generating the output message
            foreach (Dish dish in dishes)
            {
                textMessage += $"\n<b>{dish.Name}</b> - {dish.Price} [{dish.Amount}]";
                price += dish.Price * dish.Amount;
            }
            textMessage += $"\n\nИтого: <b>{price}₽.</b>";
            return textMessage;
        }

        /// <summary>
        /// Uses shopping cart to return List of Dish
        /// </summary>
        /// <param name="itemsid">shopping cart from SQL table</param>
        /// <returns>returns the list of Dish objects</returns>
        public static List<Dish> GetDishesList(string[] itemsid)
        {
            List<Dish> dishes = new List<Dish>();
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

            return dishes;
        }

        /// <summary>
        /// Deletes item from the cart
        /// </summary>
        /// <param name="chatId"></param>
        public async static void Delete(long chatId)
        {
            string textMessage = "Чтобы удалить элемент, нажмите на кнопку с номером блюда под сообщением\n";

            // Получаем список всех товаров как в корзину (с амоунтом)
            string[] itemsid = DatabaseHandler.GetCart(Convert.ToString(chatId)).Split(';'); // Splits the shopping cart into IDs
            List<Dish> dishes = GetDishesList(itemsid);

            for (int i = 0; i < dishes.Count; i++)
            {
                Dish dish = dishes[i];
                textMessage += $"\n<b>{i+1}. {dish.Name}</b> - {dish.Price} [{dish.Amount}]";
            }
            

            textMessage += $"\n\nЧтобы увидеть конечную корзину, нажмите на кнопку \"Корзина\"";

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(GetInlineKeyboard(dishes));

            await Bot.Get().SendTextMessageAsync(chatId, textMessage, replyMarkup: inlineKeyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        /// <summary>
        /// Generates keyboard to send
        /// </summary>
        /// <param name="dishes">List of Dishes </param>
        /// <returns></returns>
        private static InlineKeyboardButton[][] GetInlineKeyboard(List<Dish> dishes)
        {
            // https://stackoverflow.com/questions/39884961/create-dynamic-keyboard-telegram-bot-in-c-sharp-mrroundrobin-api
            int amount;
            if (dishes.Count < 5)
                amount = 1;
            else
                amount = (dishes.Count / 4) + 1;

            var keyboardInline = new InlineKeyboardButton[amount+1][]; // lines
            var keyboardButtons = new InlineKeyboardButton[4]; // rows

            int counter = 0;

            for (int j = 0; j < amount; j++)
            {
                for (int i = 0; i < 4 /*&& counter < dishes.Count*/; i++)
                {
                    
                    if (counter < dishes.Count)
                    {
                        keyboardButtons[i] = new InlineKeyboardButton
                        {
                            Text = $"{Convert.ToString(counter + 1)}",
                            CallbackData = $"cart:delete_item{dishes[counter].ItemId}",
                        };
                        counter++;
                    } 
                    else
                    {
                        keyboardButtons[i] = new InlineKeyboardButton
                        {
                            Text = " ",
                            CallbackData = "none",
                        };
                    }
                    
                }
                keyboardInline[j] = keyboardButtons;
                keyboardButtons = new InlineKeyboardButton[4];
            }

            // Add the last line to the keyboard
            keyboardButtons[0] = new InlineKeyboardButton
            {
                Text = "🛒 Корзина",
                CallbackData = "cart:overview",
            };
            keyboardButtons[1] = new InlineKeyboardButton
            {
                Text = "📖 В меню",
                CallbackData = "start:menu",
            };
            keyboardButtons[2] = new InlineKeyboardButton
            {
                Text = "✅ Оформить заказ",
                CallbackData = "cart:finish",
            };
            keyboardButtons[3] = new InlineKeyboardButton
            {
                Text = "📭 Опустошить корзину",
                CallbackData = "cart:empty",
            };
            keyboardInline[amount] = keyboardButtons;

            return keyboardInline;
        }

        /// <summary>
        /// Function removes item from the shopping cart
        /// </summary>
        /// <param name="chatId">User chat Id</param>
        /// <param name="removeId">Remove item Id</param>
        public static void DeleteItem(long chatId, string removeId)
        {
            string[] cart = DatabaseHandler.GetCart(Convert.ToString(chatId)).Split(';');
            cart = cart.Where(obj => obj != removeId).ToArray();
            string newCart = string.Join(";", cart);
            DatabaseHandler.ExecuteSQL($"UPDATE users SET cart = '{newCart}' WHERE chat_id={chatId}");
        }

        /// <summary>
        /// Finishes the order, adds to order table, sends message and empties the cart
        /// </summary>
        /// <param name="chatId">User's chat ID</param>
        public async static void Finish(long chatId)
        {
            DatabaseHandler.ExecuteSQL($"INSERT INTO orders (order_time, chat_id, cart) VALUES ('{DateTime.Now}', {chatId}, '{DatabaseHandler.GetCart(Convert.ToString(chatId))}')");
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
