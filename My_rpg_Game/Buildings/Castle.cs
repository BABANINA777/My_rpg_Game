using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

namespace My_Game

{
    // ========== КЛАСС ЗАМКА ==========
    // Здание для получения временных бонусов к характеристикам
    public class Castle : Building
    {
        static public int CastleCounter = 0;
        // Конструктор замка

        // Таймер бонуса замка (в днях)
        public static int bonusTimer = 0;
        public Castle(int y, int x)
        {
            PosY = y;      // Устанавливаем координату Y
            PosX = x;      // Устанавливаем координату X
            Symbol = 'C';  // Символ замка на карте
        }

        // Метод активации бонуса (вызывается при нажатии Space в замке)
        public static void ActivateBonus()
        {
            // Проверяем, не активирован ли уже бонус
            if (Castle.bonusTimer <= 0)
            {
                // Устанавливаем таймер на 7 дней
                Castle.bonusTimer = 7;

                // Применяем бонусы
                Execution.Player_1.PlayerRPGClass_1.class_state.speed += 2;
                Execution.Player_1.PlayerRPGClass_1.class_state.damage += 2;
                Execution.Player_1.PlayerRPGClass_1.class_state.unit_quantity += 1;

                Console.WriteLine("Бонус активирован на 7 дней!");
            }
            else
            {
                Console.WriteLine($"Бонус уже активен! Осталось дней: {Castle.bonusTimer}");
            }
        }

        // Метод снятия бонуса (вызывается когда таймер заканчивается)
        public static void DeactivateBonus()
        {
            // Убираем бонусы
            Execution.Player_1.PlayerRPGClass_1.class_state.speed -= 2;
            Execution.Player_1.PlayerRPGClass_1.class_state.damage -= 2;
            Execution.Player_1.PlayerRPGClass_1.class_state.unit_quantity -= 1;

            Console.WriteLine("Бонус замка закончился!");
        }

        // UI замка
        public override void BuildingUI()
        {
            Console.WriteLine();
            Console.WriteLine("=== ЗАМОК ===");
            Console.WriteLine("Добро пожаловать в замок!");
            Console.WriteLine("Нажмите Space чтобы получить бонус:");
            Console.WriteLine("+2 скорость, +2 урон, +1 слот юнитов (на 7 дней)");
            Console.WriteLine($"Осталось дней бонуса: {Castle.bonusTimer}");
            Console.WriteLine("Любая другая клавиша - выход");

            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.Spacebar)
            {
                ActivateBonus();
            }

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}