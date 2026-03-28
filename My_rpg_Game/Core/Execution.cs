using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

namespace My_Game

{
    // ========== КЛАСС ВЫПОЛНЕНИЯ ИГРЫ ==========
    // Главный класс управления игрой: инициализация, игровой цикл, отрисовка, управление
    class Execution
    {
        // Делегат: передаём координаты куда идём + флаг "отменить движение"
        public delegate void StepHandler(int targetY, int targetX, ref bool cancelMove);
        // Событие
        public static event StepHandler OnStep;
        // Константы размеров карты
        const int height = 25;  // Высота карты
        const int width = 100;  // Ширина карты

        // Временные координаты для обработки движения
        public static int cordx = 1; // Новая координата X (куда пытается пойти игрок)
        public static int cordy = 1; // Новая координата Y

        // Единственный объект игрока (static = один на всю игру)
        static public Player Player_1 = new Player();

        // Главный метод программы (точка входа)
        static void Main()
        {
            Player_1.PlayerRPGClass_1.ChouseClass(); // Выбор класса в начале игры
            InitMap();                                // Создание карты
            // ПОДПИСЫВАЕМСЯ НА СОБЫТИЕ
            OnStep += Resourses.OnPlayerStep;   // проверка ресурса, стоит первой т.к всегда разрешает перемещение
            OnStep += IsWall;                  // проверка на стену
            OnStep += Building.OnPlayerStep;   // проверка здания
            OnStep += Monster.OnPlayerStep;

            // Основной игровой цикл (выполняется бесконечно)
            while (true)
            {
                DrawMap();        // Отрисовка карты
                DrawStats();      // Отрисовка статистики
                ImputKey();       // Обработка нажатия клавиши
                AvailableSteps(); // Уменьшение шагов если игрок стоял на месте
            }
        }

        // Метод инициализации карты (вызывается один раз в начале)
        static void InitMap()
        {
            // Проходим по всем клеткам карты
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Границы карты делаем стенами (#)
                    if ((y == 0) || (y == 24) || (x == 0) || (x == 99))
                    {
                        GameState.map[y, x] = '#';
                    }
                    else // Всё остальное - пустое пространство
                    {
                        GameState.map[y, x] = ' ';
                    }
                }
            }

            // Размещаем игрока на карте
            GameState.map[1, 1] = '@'; // Игрок

            // Размещаем ресурсы
            GameState.map[1, 4] = 'G'; // Золото
            GameState.map[6, 2] = 'G'; // Золото
            GameState.map[8, 5] = 'W'; // Дерево
            GameState.map[8, 8] = 'S'; // Камень
            GameState.map[9, 8] = 'B'; // Камень

            // СОЗДАНИЕ СТАРТОВОЙ КАЗАРМЫ
            Barac startBarac = new Barac(2, 2);
            Player_1.BuildingList.Add(startBarac);
            GameState.map[2, 2] = 'H'; // Символ казармы на карте

            //Создание и добавление монстров
            Monster Goblin = new Monster(30, 5, 1, "Goblin", 8, 1);
            Monster Skeleton = new Monster(20, 10, 3, "Skeleton", 5, 3);
            Monster.MonsterList.Add(Goblin);
            Monster.MonsterList.Add(Skeleton);
            foreach (var i in Monster.MonsterList) { GameState.map[i.monster_cordy, i.monster_cordx] = 'M'; }
        }

        // Метод отрисовки карты в консоли
        static void DrawMap()
        {
            Console.Clear(); // Очищаем консоль перед новой отрисовкой
            StringBuilder sbmap = new StringBuilder(25 * (100 + 2)); // переменная для записи в нее масива символов карты с заданым capasity

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sbmap.Append(GameState.map[y, x]); // добавляем символы в sbmap 
                }
                sbmap.AppendLine();
            }
            Console.Write(sbmap.ToString());
        }

        // Метод отрисовки статистики игрока (под картой)
        static void DrawStats()
        {
            Console.WriteLine("_______________________");
            Console.WriteLine("Ресурсы:");
            Console.Write($"Золото = {Player_1.gold}; ");
            Console.Write($"Дерево = {Player_1.wood}; ");
            Console.WriteLine($"Камень = {Player_1.stone}");

            Console.WriteLine("_______________________");
            Console.WriteLine("Время:");
            Console.WriteLine($"День: {GameState.day}  Неделя: {GameState.week}");
            if (Castle.bonusTimer > 0)
            {
                Console.WriteLine($"Бонус замка активен! Осталось дней: {Castle.bonusTimer}");
            }

            Console.WriteLine("_______________________");
            Console.WriteLine("Информация об игроке:");
            Console.WriteLine($"Оставшиеся шаги = {Player_1.count_step}");
            Console.WriteLine($"Класс: {Player_1.PlayerRPGClass_1.PlayerRPG}");
            Console.WriteLine($"Здоровье: {Player_1.player_hp}");
            Console.WriteLine($"Скорость: {Player_1.PlayerRPGClass_1.class_state.speed}");
            Console.WriteLine($"Урон: {Player_1.PlayerRPGClass_1.class_state.damage}");
            Console.WriteLine($"Броня: {Player_1.PlayerRPGClass_1.class_state.armor}");
            Console.WriteLine($"Слотов юнитов: {Player_1.PlayerRPGClass_1.class_state.unit_quantity}");

            // Выводим список юнитов игрока
            Console.WriteLine("_______________________");
            Console.WriteLine("Ваши воины:");
            if (Player_1.UnitList.Count == 0)
            {
                Console.WriteLine("  Нет юнитов");
            }
            else
            {
                foreach (UnitSlot unit in Player_1.UnitList)
                {
                    Console.WriteLine($"  {unit.UnitClass}: {unit.Count} шт.");
                }
            }

            // Подсказки управления
            Console.WriteLine("_______________________");
            Console.WriteLine("Управление: Стрелки - движение, C - строить, Enter - новый день, F5 - сохранить, F9 - загрузить, I - инвентарь");
        }

        // Метод обработки нажатия клавиши
        static void ImputKey()
        {
            // Сохраняем текущие координаты игрока
            cordx = Player_1.player_x;
            cordy = Player_1.player_y;

            // Читаем нажатую клавишу
            var key = Console.ReadKey();

            // Обрабатываем клавишу
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow: cordx--; PlayerWASD(); break; // Влево
                case ConsoleKey.RightArrow: cordx++; PlayerWASD(); break; // Вправо
                case ConsoleKey.UpArrow: cordy--; PlayerWASD(); break; // Вверх
                case ConsoleKey.DownArrow: cordy++; PlayerWASD(); break; // Вниз
                case ConsoleKey.F5: GameState.SaveGame(); break;  // Сохранить
                case ConsoleKey.F9: GameState.LoadGame(); break;  // Загрузить
                case ConsoleKey.Enter: GameState.DayOfWeek(); break; // Следующий день
                case ConsoleKey.C: Building.BuildBuilding(); break; // Строить здание
                case ConsoleKey.I: Inventory.InventoryUI(); break; // Открыть инвентарь
            }
        }

        // Метод перемещения игрока (вызывается после нажатия стрелок)
        static void PlayerWASD()
        {
            // Проверяем, остались ли у игрока шаги
            if (Player_1.count_step == 0)
            {
                Console.WriteLine("У вас закончились шаги! Нажмите Enter для нового дня.");
                Console.ReadKey();
                return;
            }

            // ВЫЗЫВАЕМ СОБЫТИЕ: пусть подписчики проверят клетку и решат, двигаться ли
            bool cancelMove = false;
            OnStep?.Invoke(cordy, cordx, ref cancelMove);
            // Если кто-то из подписчиков сказал "не двигаться" (зашли в здание и т.п.) — выходим
            if (cancelMove)
            {
                return;
            }

            // === БАЗОВАЯ ЛОГИКА ДВИЖЕНИЯ ===
            // Стираем игрока с текущей позиции
            GameState.map[Player_1.player_y, Player_1.player_x] = ' ';

            // Перемещаем игрока на новые координаты
            Player_1.player_x = cordx;
            Player_1.player_y = cordy;

            // Рисуем игрока на новой позиции
            GameState.map[Player_1.player_y, Player_1.player_x] = '@';

            // Уменьшаем количество шагов
            Player_1.count_step--;
        }

        // Метод проверки: свободна ли клетка (не стена ли)
        public static void IsWall(int y, int x, ref bool cancel)
        {
            if (GameState.map[y, x] == '#')
            {
                cancel = true; // Возвращаем true если стена
            }
            else { cancel = false; }
        }

        // Метод уменьшения шагов если игрок стоял на месте
        static void AvailableSteps()
        {
            // Если координаты не изменились - игрок стоял на месте
            // (например, упёрся в стену или зашёл в здание)
            if ((cordx == Player_1.player_x) && (cordy == Player_1.player_y))
            {
                // Не уменьшаем шаги - это уже сделано в PlayerWASD или не нужно
            }
        }

        // Метод уменьшения таймера бонуса (вызывается каждый день)
        public static void BonusTimerTick()
        {
            if (Castle.bonusTimer > 0)
            {
                Castle.bonusTimer--; // Уменьшаем таймер

                // Если таймер закончился - снимаем бонус
                if (Castle.bonusTimer == 0)
                {
                    Castle.DeactivateBonus();
                }
            }
        }

        //создает и уменьшает таймер для ресурсного здания
        public static void ResourseTimerTick()
        {
            if (ResourceBuilding.ResourceBuildingCounter > 0 && ResourceBuilding.NewResurseTimer != 0)
            {
                ResourceBuilding.NewResurseTimer -= 1;
            }
            else if (ResourceBuilding.ResourceBuildingCounter > 0)
            {
                ResourceBuilding.NewResurseTimer = 7;
                Player_1.gold += 1 * ResourceBuilding.ResourceBuildingCounter;
                Player_1.wood += 1 * ResourceBuilding.ResourceBuildingCounter;
                Player_1.stone += 1 * ResourceBuilding.ResourceBuildingCounter;
            }
        }
    }
}
