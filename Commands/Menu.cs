using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TestTaskTelegramBot.Service;

namespace TestTaskTelegramBot.Commands
{
    class Menu
    {
        /// <summary>
        /// Функция для открытия главной страницы меню
        /// </summary>
        public async static void StartMenu(long chatId)
        {
            string textMessage = "Используйте кнопки для навигации по меню.";
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                           {
                                new[] { InlineKeyboardButton.WithCallbackData("Основные блюда", "menu:main_course") },
                                new[] { InlineKeyboardButton.WithCallbackData("Десерты", "menu:dessert") }
                            });
            await Bot.Get().SendTextMessageAsync(chatId: chatId, text: textMessage, replyMarkup: inlineKeyboard);
        }

        /// <summary>
        /// Функция для отправки блюд 
        /// </summary>
        public async static void SendDishesCategory(ChatId chatId, string name)
        {
            List<Dish> dishes = new List<Dish>();

            for (int i = 0; i < dishes.Count; i++)
            {
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                               {
                                new[] { InlineKeyboardButton.WithCallbackData("Добавить в корзину", $"menu:add_to_chart{dishes[i].ItemId}") }
                            });

                await Bot.Get().SendPhotoAsync(
                    chatId: chatId,
                    photo: dishes[i].Link,
                    caption:  $"<b>{dishes[i].Name}</b>\n{dishes[i].Description}\n\nПриблизительное время приготовления: <i>{dishes[i].CookingTime}<i>\nЦена: <i>{dishes[i].Price}<i>",
                    replyMarkup: inlineKeyboard
                    );
            }
        }

        public async static void AddToCart(long chatId, string dishId)
        {
            DatabaseHandler.AddItem(chatId, dishId);
        }
    }
}
