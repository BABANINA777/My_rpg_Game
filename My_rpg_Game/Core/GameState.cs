using System;

namespace My_Game

{
    // ========== КЛАСС ИГРОВОГО СОСТОЯНИЯ ==========
    // Хранит общие данные игры: карту, время, методы сохранения/загрузки
    public class GameState
    {
        // Статическая карта игры (одна на всю игру)
        public static char[,] map = new char[25, 100];

        // Игровое время
        public static int day = 1;   // Текущий день (1-7)
        public static int week = 1;  // Текущая неделя

        // Метод перехода на следующий день
        public static void DayOfWeek()
        {
            day++; // Увеличиваем день

            // Восстанавливаем шаги игроку в зависимости от его класса
            Execution.Player_1.PlayerRPGClass_1.InitAvailableStep();

            // Уменьшаем таймер бонуса замка
            Execution.BonusTimerTick();

            // Уменьшаем таймер до следующих ресурсов
            Execution.ResourseTimerTick();

            // Если прошло 7 дней - начинаем новую неделю
            if (day > 7)
            {
                week++; // Увеличиваем номер недели
                day = 1; // Сбрасываем день на первый
            }
        }

        // Метод сохранения игры в текстовый файл
        public static void SaveGame()
        {
            UnitSlot.SaveGameUnit(Execution.Player_1.UnitList);// сохранение юнитов
            Inventory.SaveInventory();//сохраняем инвентарь
                                      // Открываем файл для записи (false = перезаписать, а не добавить)
            using (StreamWriter writer = new StreamWriter("SaveGame.txt", false))
            {
                // Сохраняем карту: каждую клетку в отдельную строку
                for (int y = 0; y < 25; y++)
                {
                    for (int x = 0; x < 100; x++)
                    {
                        writer.WriteLine(map[y, x]);
                    }
                }

                // Сохраняем данные игрока
                writer.WriteLine(Execution.Player_1.player_y);      // Координата Y
                writer.WriteLine(Execution.Player_1.player_x);      // Координата X
                writer.WriteLine(Execution.Player_1.count_step);    // Оставшиеся шаги
                writer.WriteLine(Execution.Player_1.gold);          // Золото
                writer.WriteLine(Execution.Player_1.wood);          // Дерево
                writer.WriteLine(Execution.Player_1.stone);         // Камень

                // Сохраняем игровое время
                writer.WriteLine(day);
                writer.WriteLine(week);

                // Сохраняем характеристики класса игрока
                writer.WriteLine((int)Execution.Player_1.PlayerRPGClass_1.PlayerRPG); // Тип класса
                writer.WriteLine(Execution.Player_1.player_hp);        // Здоровье
                writer.WriteLine(Execution.Player_1.PlayerRPGClass_1.class_state.speed);        // Скорость
                writer.WriteLine(Execution.Player_1.PlayerRPGClass_1.class_state.damage);       // Урон
                writer.WriteLine(Execution.Player_1.PlayerRPGClass_1.class_state.unit_quantity); // Лимит слотов
                writer.WriteLine(Execution.Player_1.PlayerRPGClass_1.class_state.armor); // броня

                // Сохраняем юнитов игрока
                foreach (UnitSlot unit in Execution.Player_1.UnitList)
                {
                    writer.WriteLine((int)unit.UnitClass); // Тип юнита
                    writer.WriteLine(unit.Count);          // Количество
                }
            }

            Console.WriteLine("Игра сохранена!");
            Console.ReadKey();
        }

        // Метод загрузки игры из текстового файла
        public static void LoadGame()
        {
            UnitSlot.LoadGameUnit(Execution.Player_1.UnitList); // загрузка юнитов
            Inventory.LoadInventory();//Загрузка инвентаря
                                      // Открываем файл для чтения
            using (StreamReader reader = new StreamReader("SaveGame.txt"))
            {
                // Загружаем карту
                for (int y = 0; y < 25; y++)
                {
                    for (int x = 0; x < 100; x++)
                    {
                        // Читаем строку и берём первый символ
                        map[y, x] = (char)reader.ReadLine()[0];
                    }
                }

                // Загружаем данные игрока
                Execution.Player_1.player_y = int.Parse(reader.ReadLine());
                Execution.Player_1.player_x = int.Parse(reader.ReadLine());
                Execution.Player_1.count_step = int.Parse(reader.ReadLine());
                Execution.Player_1.gold = int.Parse(reader.ReadLine());
                Execution.Player_1.wood = int.Parse(reader.ReadLine());
                Execution.Player_1.stone = int.Parse(reader.ReadLine());

                // Загружаем игровое время
                day = int.Parse(reader.ReadLine());
                week = int.Parse(reader.ReadLine());

                // Загружаем характеристики класса игрока
                Execution.Player_1.PlayerRPGClass_1.PlayerRPG = (PlayerRPGClass.PlayerRPG_class)int.Parse(reader.ReadLine());
                Execution.Player_1.player_hp = int.Parse(reader.ReadLine());
                Execution.Player_1.PlayerRPGClass_1.class_state.speed = int.Parse(reader.ReadLine());
                Execution.Player_1.PlayerRPGClass_1.class_state.damage = int.Parse(reader.ReadLine());
                Execution.Player_1.PlayerRPGClass_1.class_state.unit_quantity = int.Parse(reader.ReadLine());
                Execution.Player_1.PlayerRPGClass_1.class_state.armor = int.Parse(reader.ReadLine());
            }

            Console.WriteLine("Игра загружена!");
            Console.ReadKey();
        }
    }
}