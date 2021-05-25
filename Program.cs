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

        private static void LoadDB()
        {
            DatabaseHandler.ExecuteSQL("CREATE TABLE items (item_id INTEGER, name VARCHAR(25), description VARCHAR(255), price INTEGER, picture VARCHAR(255), cooking_time VARCHAR(255), category VARCHAR(30))");
            DatabaseHandler.ExecuteSQL("CREATE TABLE users (chat_id INTEGER, cart VARCHAR(255))");
        }

        /// <summary>
        /// Принимает и обрабатывает клавиатуру под сообщениями
        /// </summary>
        private static void OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            long chatId = e.CallbackQuery.Message.Chat.Id;

            string check = e.CallbackQuery.Data;
            string dishId = "0";

            if (e.CallbackQuery.Message.Text.Contains("menu:add_to_cart"))
            {
                dishId = check.Substring(16);
                check = "menu:add_to_cart";
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
            }
        }

        /// <summary>
        /// Принимает и обрабатывает сообщения 
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
