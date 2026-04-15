using System.Text.Json;
using System.Text.Json.Serialization;


namespace My_Game

{
    // ========== КЛАСС ИГРОВОГО СОСТОЯНИЯ ==========
    // Хранит общие данные игры: карту, время, методы сохранения/загрузки
    public class GameState
    {
        // ПОЛЕ 
        private static GameState _instance;

        //СВОЙСТВО
        public static GameState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameState();
                }
                return _instance;
            }
            
        }
        // Статическая карта игры (одна на всю игру)
        [JsonIgnore]
        public char[,] map = new char[25, 100];


        // ИГРОВЫЕ ОБЪЕКТЫ
        // 1. Игрок
        public Player Player_1 { get; set; } = new Player();

        // 2. Списки сущностей на карте
        public List<Monster> MonstersList { get; set; } = new List<Monster>();
        public List<Building> Buildings { get; set; } = new List<Building>();

        // 3. Инвентарь
        public List<Item> ItemEquipList { get; set; } = new List<Item>();
        public List<Item> ItemNotEquipList { get; set; } = new List<Item>()
        {
        new Item("EpicHelmet", 'H', "Эпический шлем (+10 защита)", 0,2),
        new Item("RareHelmet", 'H', "Редкий шлем (+5 защита)", 0,2),
        new Item("LegendSword", 'S', "Легендарный меч (+10 урон)", 3, 2),
        new Item("Shield", 'D', "Щит (+3 защита)", 1, 2)
        };

        // 4. Квесты
        public List<Quest> ActiveQuests { get; set; } = new List<Quest>();

        // Игровое время
        public int day { get; set; } = 1;   // Текущий день (1-7)
        public int week { get; set; } = 1;  // Текущая неделя

        [JsonIgnore]
        public Action WeeklyEvent;// поле стат переменной
        public GameState()
        {
            // статическая переменная, которая вызывает метод и навсегда сохраняет внутри себя ту самую лямбда-функцию со счетчиком
            if (WeeklyEvent == null)
            {
                WeeklyEvent = CreateWeeklyEventGenerator();
            }
        }
            
        
        // метод, который создает замыкание
        public Action CreateWeeklyEventGenerator()
        {
            Random rnd = new Random();
            int eventCounter = 0; // Эта переменная будет "замкнута" и сохранит свое значение между вызовами!

            // Возвращаем безымянную функцию (Action), которая имеет доступ к eventCounter и rnd
            return () =>
            {
                eventCounter++; // Увеличиваем замкнутый счетчик

                Console.Clear();
                Console.WriteLine($"\n=== СОБЫТИЕ НЕДЕЛИ (Событие №{eventCounter}) ===");

                int eventType = rnd.Next(1, 4); // Случайное число 1, 2 или 3

                switch (eventType)
                {
                    case 1:
                        GameState.Instance.Player_1.gold += 5;
                        Console.WriteLine("Караван торговцев: Вы нашли оброненный кошель! +5 золота.");
                        break;
                    case 2:
                        GameState.Instance.Player_1.wood += 5;
                        GameState.Instance.Player_1.stone += 5;
                        Console.WriteLine("Удачная находка: Вы наткнулись на заброшенную телегу! +5 дерева, +5 камня.");
                        break;
                    case 3:
                        if (GameState.Instance.Player_1.player_hp <= 90)
                        {
                            GameState.Instance.Player_1.player_hp += 10;
                            Console.WriteLine("Благословение природы: Вы чувствуете прилив сил! +10 HP.");
                        }
                        else
                        {
                            Console.WriteLine("Спокойная неделя: Ничего необычного не произошло.");
                        }
                        break;
                }
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            };
        }

        public char[][] jagged
        {
            get
            {
                char[][] _jagged = new char[25][];
                for (int i=0; i < 25; i++ )
                {
                    _jagged[i] = new char[100];
                    for (int j = 0; j<100;j++)
                    {
                        _jagged[i][j] = map[i,j];
                    }
                }
                return _jagged;
            }
            set
            {
                for (int y = 0; y < 25; y++)
                    for (int x = 0; x < 100; x++)
                        map[y, x] = value[y][x];
            }


        }
        // Метод перехода на следующий день
        public void DayOfWeek()
        {
            Instance.day++; // Увеличиваем день

            // Восстанавливаем шаги игроку в зависимости от его класса
            GameState.Instance.Player_1.PlayerRPGClass_1.InitAvailableStep();

            // Уменьшаем таймер бонуса замка
            Execution.BonusTimerTick();

            // Уменьшаем таймер до следующих ресурсов
            Execution.ResourseTimerTick();

            // Если прошло 7 дней - начинаем новую неделю
            if (Instance.day > 7)
            {
                Instance.week++; // Увеличиваем номер недели
                Instance.day = 1; // Сбрасываем день на первый
                Instance.WeeklyEvent(); //вызов метода для еженельных ивентов(замыкание)
            }
        }

        // Метод сохранения игры в текстовый файл
        public void SaveGame()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IncludeFields = true
                };
                string json = JsonSerializer.Serialize(Instance, options);
                File.WriteAllText("SaveGame.json", json);



                Console.WriteLine("\nИгра сохранена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка сохранения: {ex.Message}");
            }
            Console.ReadKey();



            //UnitSlot.SaveGameUnit(Execution.Player_1.UnitList);// сохранение юнитов
            //Inventory.SaveInventory();//сохраняем инвентарь
            //                          // Открываем файл для записи (false = перезаписать, а не добавить)
            //using (StreamWriter writer = new StreamWriter("SaveGame.txt", false))
            //{
            //    // Сохраняем карту: каждую клетку в отдельную строку
            //    for (int y = 0; y < 25; y++)
            //    {
            //        for (int x = 0; x < 100; x++)
            //        {
            //            writer.WriteLine(Instance.map[y, x]);
            //        }
            //    }

            //    // Сохраняем данные игрока
            //    writer.WriteLine(Execution.Player_1.player_y);      // Координата Y
            //    writer.WriteLine(Execution.Player_1.player_x);      // Координата X
            //    writer.WriteLine(Execution.Player_1.count_step);    // Оставшиеся шаги
            //    writer.WriteLine(Execution.Player_1.gold);          // Золото
            //    writer.WriteLine(Execution.Player_1.wood);          // Дерево
            //    writer.WriteLine(Execution.Player_1.stone);         // Камень

            //    // Сохраняем игровое время
            //    writer.WriteLine(Instance.day);
            //    writer.WriteLine(Instance.week);

            //    // Сохраняем характеристики класса игрока
            //    writer.WriteLine((int)Execution.Player_1.PlayerRPGClass_1.PlayerRPG); // Тип класса
            //    writer.WriteLine(Execution.Player_1.player_hp);        // Здоровье
            //    writer.WriteLine(Execution.Player_1.PlayerRPGClass_1.class_state.speed);        // Скорость
            //    writer.WriteLine(Execution.Player_1.PlayerRPGClass_1.class_state.damage);       // Урон
            //    writer.WriteLine(Execution.Player_1.PlayerRPGClass_1.class_state.unit_quantity); // Лимит слотов
            //    writer.WriteLine(Execution.Player_1.PlayerRPGClass_1.class_state.armor); // броня

            //    // Сохраняем юнитов игрока
            //    foreach (UnitSlot unit in Execution.Player_1.UnitList)
            //    {
            //        writer.WriteLine((int)unit.UnitClass); // Тип юнита
            //        writer.WriteLine(unit.Count);          // Количество
            //    }
            //}

            //Console.WriteLine("Игра сохранена!");
            //Console.ReadKey();
        }

        // Метод загрузки игры из текстового файла
        public static void LoadGame()
        {
            


            try
            {
                var options = new JsonSerializerOptions { IncludeFields = true };
                string json = File.ReadAllText("SaveGame.json");
                
                GameState loadedState = JsonSerializer.Deserialize<GameState>(json, options);

                loadedState.WeeklyEvent = loadedState.CreateWeeklyEventGenerator();
                _instance = loadedState;


                Console.WriteLine("Игра загружена!");


            }
            catch (Exception e) {Console.WriteLine(e.Message);}
            Console.ReadKey();

            //UnitSlot.LoadGameUnit(Execution.Player_1.UnitList); // загрузка юнитов
            //Inventory.LoadInventory();//Загрузка инвентаря
            //                          // Открываем файл для чтения
            //using (StreamReader reader = new StreamReader("SaveGame.txt"))
            //{
            //    // Загружаем карту
            //    for (int y = 0; y < 25; y++)
            //    {
            //        for (int x = 0; x < 100; x++)
            //        {
            //            // Читаем строку и берём первый символ
            //            Instance.map[y, x] = (char)reader.ReadLine()[0];
            //        }
            //    }

            //    // Загружаем данные игрока
            //    Execution.Player_1.player_y = int.Parse(reader.ReadLine());
            //    Execution.Player_1.player_x = int.Parse(reader.ReadLine());
            //    Execution.Player_1.count_step = int.Parse(reader.ReadLine());
            //    Execution.Player_1.gold = int.Parse(reader.ReadLine());
            //    Execution.Player_1.wood = int.Parse(reader.ReadLine());
            //    Execution.Player_1.stone = int.Parse(reader.ReadLine());

            //    // Загружаем игровое время
            //    Instance.day = int.Parse(reader.ReadLine());
            //    Instance.week = int.Parse(reader.ReadLine());

            //    // Загружаем характеристики класса игрока
            //    Execution.Player_1.PlayerRPGClass_1.PlayerRPG = (PlayerRPGClass.PlayerRPG_class)int.Parse(reader.ReadLine());
            //    Execution.Player_1.player_hp = int.Parse(reader.ReadLine());
            //    Execution.Player_1.PlayerRPGClass_1.class_state.speed = int.Parse(reader.ReadLine());
            //    Execution.Player_1.PlayerRPGClass_1.class_state.damage = int.Parse(reader.ReadLine());
            //    Execution.Player_1.PlayerRPGClass_1.class_state.unit_quantity = int.Parse(reader.ReadLine());
            //    Execution.Player_1.PlayerRPGClass_1.class_state.armor = int.Parse(reader.ReadLine());
            //}

            //Console.WriteLine("Игра загружена!");
            //Console.ReadKey();
        }

        // Метод окончания игры
        public static void GameOver()
        {
            while (true) // Бесконечный цикл, пока игрок не сделает правильный выбор
            {
                Console.Clear();
                Console.WriteLine("=======================================");
                Console.WriteLine("             ИГРА ОКОНЧЕНА             ");
                Console.WriteLine("=======================================");
                Console.WriteLine("Вы доблестно сражались, но пали в бою...");
                Console.WriteLine();
                Console.WriteLine("Что будем делать дальше?");
                Console.WriteLine("1 - Загрузить последнее сохранение");
                Console.WriteLine("2 - Выйти из игры");

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
                {
                    // Пытаемся загрузить игру
                    try
                    {
                        LoadGame();
                        return;     // Выходим из метода GameOver, чтобы продолжился основной игровой цикл
                    }
                    catch (Exception)
                    {
                        // Защита от вылета: если файла SaveGame.txt нет (игрок ни разу не сохранялся)
                        Console.WriteLine("\n[Ошибка] Файл сохранения не найден или поврежден!");
                        Console.WriteLine("Нажмите любую клавишу...");
                        Console.ReadKey();
                    }
                }
                else if (key == ConsoleKey.D2 || key == ConsoleKey.NumPad2)
                {
                    
                    Environment.Exit(0);
                }
            }
        }
    }
}