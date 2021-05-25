using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

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
            string textMessage = "Ваша корзина: ";
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                               {
                                new[] { InlineKeyboardButton.WithCallbackData("В меню", $"menu:add_to_cart") },
                                new[] { InlineKeyboardButton.WithCallbackData("Очистить корзину", $"start:menu") },
                                new[] { InlineKeyboardButton.WithCallbackData("Удалить блюдо", $"cart:overview") },
                                new[] { InlineKeyboardButton.WithCallbackData("Оформить заказ", $"cart:overview") }
                            });

            await Bot.Get().SendTextMessageAsync(chatId: chatId, text: textMessage, replyMarkup: inlineKeyboard);
        }
    }
}
