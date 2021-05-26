using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TestTaskTelegramBot.Service;

namespace TestTaskTelegramBot.Commands
{
    class Start
    {
        /// <summary>
        /// A message that sends to user when he starts the bot
        /// </summary>
        /// <param name="message"></param>
        public async static void StartMessage(Message message)
        {
            string messageText = $"Приветствуем вас в ресторане \"Chäf\", {message.Chat.LastName} {message.Chat.FirstName}.";
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                           {
                                new[] { InlineKeyboardButton.WithCallbackData("Меню", "start:menu") },
                                new[] { InlineKeyboardButton.WithCallbackData("Корзина", "cart:overview") }
                            });
            DatabaseHandler.ExecuteSQL($"INSERT OR IGNORE INTO users VALUES ({message.Chat.Id}, \'\')");
            await Bot.Get().SendTextMessageAsync(chatId: message.Chat.Id, text: messageText, replyMarkup: inlineKeyboard);
        }
    }
}
