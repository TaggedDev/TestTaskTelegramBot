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
            DatabaseHandler.ExecuteSQL("CREATE TABLE IF NOT EXISTS items (item_id INTEGER, name VARCHAR(25), description VARCHAR(255), price INTEGER, picture VARCHAR(255), cooking_time VARCHAR(255), category VARCHAR(30))");
            DatabaseHandler.ExecuteSQL("CREATE TABLE IF NOT EXISTS users (chat_id INTEGER, cart VARCHAR(255))");
            //DatabaseHandler.ExecuteSQL("INSERT INTO items VALUES (1, \'плов\', \'пловный плов\', 5000, \'https://avatars.githubusercontent.com/u/45800215?v=4\', \'10 мин\', \'main_course\')");
        }

        /// <summary>
        /// Gets and handles the keyboard
        /// </summary>
        private static void OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            long chatId = e.CallbackQuery.Message.Chat.Id;

            string check = e.CallbackQuery.Data;
            string dishId = "0";

            if (check.Length > 15)
            {
                string cut = e.CallbackQuery.Data.Substring(0, 16);
                if (cut.Equals("menu:add_to_cart"))
                {
                    dishId = check.Substring(16);
                    check = "menu:add_to_cart";
                }
            }
            

            switch (check)
            {
                case "start:menu":
                    Menu.StartMenu(chatId);
                    return;
                case "start:cart":
                    return;
                case "menu:main_course":
                    Menu.SendDishesCategory(chatId, "main_course");
                    return;
                case "menu:dessert":
                    Menu.SendDishesCategory(chatId, "dessert");
                    return;
                case "menu:add_to_cart":
                    Menu.AddToCart(chatId, dishId);
                    return;
                case "cart:overview":
                    Cart.Overview(chatId);
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
