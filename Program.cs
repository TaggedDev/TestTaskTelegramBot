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

            switch (e.CallbackQuery.Data)
            {
                case "start:menu":
                    Menu.StartMenu(chatId);
                    return;
                case "start:cart":
                    return;
                case "menu:main_course":
                    Menu.SendMainCourse(chatId);
                    return;
                case "menu:dessert":
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
