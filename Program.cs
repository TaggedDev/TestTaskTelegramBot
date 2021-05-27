using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TestTaskTelegramBot.Commands;
using TestTaskTelegramBot.Service;

namespace TestTaskTelegramBot
{
    class Program
    {
        private static ITelegramBotClient botClient;

        /// <summary>
        /// Start function
        /// </summary>
        static void Main(string[] args)
        {
            botClient = Bot.Get();
            var me = botClient.GetMeAsync().Result;
            LoadDB();

            botClient.OnMessage += OnMessageRecieved;
            botClient.OnCallbackQuery += OnCallbackQuery;
            botClient.StartReceiving();
            Console.ReadKey();
        }

        /// <summary>
        /// Creates tables if they don't exist
        /// </summary>
        private static void LoadDB()
        {
            DatabaseHandler.ExecuteSQL("CREATE TABLE IF NOT EXISTS items (item_id INTEGER UNIQUE, name VARCHAR(25), description VARCHAR(255), price INTEGER, picture VARCHAR(255), cooking_time VARCHAR(255), category VARCHAR(30))");
            DatabaseHandler.ExecuteSQL("CREATE TABLE IF NOT EXISTS users (chat_id INTEGER UNIQUE, cart VARCHAR(255))");
            /*DatabaseHandler.ExecuteSQL("INSERT INTO items VALUES (1, \'плов\', \'сначала укропу, потом кошачьу жопу\', 5000, \'https://www.gastronom.ru/binfiles/images/20170418/b87bb973.jpg', \'10 мин\', \'main_course\')");
            DatabaseHandler.ExecuteSQL("INSERT INTO items VALUES (2, \'курица\', \'нет блин корова\', 300, \'https://lapku.ru/images/44508/kuritsa-nesushka.jpg', \'15 мин\', \'main_course\')");
            DatabaseHandler.ExecuteSQL("INSERT INTO items VALUES (3, \'чекактобыло\', \'я не запомнил название\', 1000, \'https://www.patee.ru/r/x6/14/b8/6d/960m.jpg', \'5 мин\', \'main_course\')");
            DatabaseHandler.ExecuteSQL("INSERT INTO items VALUES (4, \'мороженное\', \'вкусно\', 600, \'https://media-cdn.tripadvisor.com/media/photo-s/18/7c/da/68/bonmot-ice-cream.jpg', \'3 мин\', \'dessert\')");
            DatabaseHandler.ExecuteSQL("INSERT INTO items VALUES (5, \'баббл ти\', \'дорого за чай\', 800, \'https://p0.zoon.ru/preview/F3rFsvKojZhcLFwv7Qsrng/659x440x85/0/1/8/52497a1040c088193a8b4583_5257ae6087e01.jpg', \'5 мин\', \'dessert\')");*/
        }

        /// <summary>
        /// Gets and handles the keyboard
        /// </summary>
        private static void OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            long chatId = e.CallbackQuery.Message.Chat.Id;

            string check = e.CallbackQuery.Data;
            string dishId = "0", removeId = "0";

            if (check.Substring(0, 4).Equals("menu") && check.Length > 15)
            {
                string cut = e.CallbackQuery.Data.Substring(0, 16);
                if (cut.Equals("menu:add_to_cart"))
                {
                    dishId = check.Substring(16);
                    check = "menu:add_to_cart";
                }
            }

            if (check.Substring(0, 4).Equals("cart") && check.Length > 15)
            {
                string cut = e.CallbackQuery.Data.Substring(0, 16);
                if (cut.Equals("cart:delete_item"))
                {
                    removeId = check.Substring(16);
                    check = "cart:delete_item";
                }
            }
            

            switch (check)
            {
                case "start:menu":
                    Menu.StartMenu(chatId);
                    return;
                case "menu:main_course":
                    Menu.SendDishesCategory(chatId, "main_course");
                    return;
                case "menu:dessert":
                    Menu.SendDishesCategory(chatId, "dessert");
                    return;
                case "menu:add_to_cart":
                    Menu.AddToCart(chatId, dishId, e.CallbackQuery.Message.MessageId);
                    return;
                case "cart:overview":
                    Cart.Overview(chatId);
                    return;
                case "cart:empty":
                    Cart.Empty(chatId);
                    return;
                case "cart:delete":
                    Cart.Delete(chatId);
                    return;
                case "cart:finish":
                    Cart.Finish(chatId);
                    return;
                case "cart:delete_item":
                    Cart.DeleteItem(chatId, removeId);
                    return;
            }
        }

        /// <summary>
        /// Gets and handles the messages
        /// </summary>
        private static void OnMessageRecieved(object sender, MessageEventArgs e)
        {
            string message = e.Message.Text;
            switch (message)
            {
                case "/start":
                    Start.StartMessage(e.Message);
                    return;
            }
        }
    }
}
