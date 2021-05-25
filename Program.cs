using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TestTaskTelegramBot.Commands;

namespace TestTaskTelegramBot
{
    class Program
    {
        private static ITelegramBotClient botClient;
        static void Main(string[] args)
        {
            botClient = Bot.Get();
            var me = botClient.GetMeAsync().Result;

            botClient.OnMessage += OnMessageRecieved;
            botClient.OnCallbackQuery += OnCallbackQuery;
            botClient.StartReceiving();
            Console.ReadKey();
        }

        /// <summary>
        /// Принимает и обрабатывает клавиатуру под сообщениями
        /// </summary>
        private static void OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            long chatId = e.CallbackQuery.Message.Chat.Id;

            string check = e.CallbackQuery.Data;
            int dishId = 0;

            if (e.CallbackQuery.Message.Text.Contains("menu:add_to_cart"))
            {
                check = "menu:add_to_cart";
                dishId = 1;
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
