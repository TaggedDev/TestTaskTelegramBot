using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TestTaskTelegramBot.Commands
{
    class Start
    {
        public async static void StartMessage(Message message)
        {
            string messageText = $"Приветствуем вас в ресторане \"Chäf\", {message.Chat.LastName} {message.Chat.FirstName}.";
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                           {
                                new[] { InlineKeyboardButton.WithCallbackData("Меню", "start:menu") },
                                new[] { InlineKeyboardButton.WithCallbackData("Корзина", "start:cart") }
                            });
            await Bot.Get().SendTextMessageAsync(chatId: message.Chat.Id, text: messageText, replyMarkup: inlineKeyboard);
        }
    }
}
