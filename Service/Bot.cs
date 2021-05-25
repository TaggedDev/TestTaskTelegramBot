using System;
using System.IO;
using System.Threading;
using Telegram.Bot;
namespace TestTaskTelegramBot
{
    class Bot
    {
        private static string Token { get; set; }
        private static TelegramBotClient botClient;

        /// <summary>
        /// Возвращает экземляр TelegramBotClient. Токен бота берётся из файла tokenfile.txt
        /// </summary>
        /// <returns></returns>
        public static TelegramBotClient Get()
        {
            // If it was already created
            if (botClient != null)
                return botClient;

            // Else 
            else
            {
                using (StreamReader sr = new StreamReader("tokenfile.txt", System.Text.Encoding.Default))
                {
                    Token = sr.ReadLine();
                    Console.WriteLine(Token);
                }
                botClient = new TelegramBotClient(Token) { Timeout = TimeSpan.FromSeconds(2) };
            }
            return botClient;
        }
    }
}
