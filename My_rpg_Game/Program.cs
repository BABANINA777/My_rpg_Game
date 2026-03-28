
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
namespace My_Game
{
    // ========== ИНТЕРФЕЙС ДЛЯ РЕСУРСОВ ==========
    // ========== КЛАСС С РЕАЛИЗАЦИЕЙ ИНТЕРФЕЙСА ==========
    public interface ICollectable
    {
        char Symbol { get; }
        string Name { get; }
    }
    public class Resourses : ICollectable
    {
        public enum ResourceType
        {
            Gold,
            Wood,
            Stone,
            Bonus
        }
        public ResourceType Type { get; }    // Тип ресурса

        // Символ вычисляется каждый раз из Type
        public char Symbol
        {
            get
            {
                switch (Type)
                {
                    case ResourceType.Gold: return 'G';
                    case ResourceType.Wood: return 'W';
                    case ResourceType.Stone: return 'S';
                    case ResourceType.Bonus: return 'B';
                    default: return '?';
                }
            }
        }

        // Имя вычисляется из Type
        public string Name
        {
            get
            {
                switch (Type)
                {
                    case ResourceType.Gold: return "Золото";
                    case ResourceType.Wood: return "Дерево";
                    case ResourceType.Stone: return "Камень";
                    case ResourceType.Bonus: return "Бонус";
                    default: return "Неизвестно";
                }
            }
        }

        public Resourses(ResourceType resourceType)
        {
            Type = resourceType;
        }

        static List<Resourses> ResourseList = new()
        {new Resourses((ResourceType)0), new Resourses((ResourceType)0), new Resourses((ResourceType)1), new Resourses((ResourceType)2), new Resourses((ResourceType)3) };
        public static void OnPlayerStep(int y, int x, ref bool cancel)
        {
            cancel = true; // всегда разрешает перемещение
            char cell = GameState.map[y, x]; // что на клетке

            // Быстрый фильтр: если это не ресурс — выходим
            if (cell != 'G' && cell != 'W' && cell != 'S' && cell != 'B')
                return;

            // Находим ресурс, который соответствует символу клетки
            Resourses found = null;
            foreach (var res in ResourseList)
            {
                if (res.Symbol == cell)
                {
                    found = res;
                    break; // нашли — дальше не ищем
                }
            }
            // Выдаём эффект в зависимости от символа клетки
            if (cell == 'G') { Execution.Player_1.gold++; Console.WriteLine($"Получено +1{found.Name}"); Console.ReadKey(); }
            else if (cell == 'W') { Execution.Player_1.wood++; Console.WriteLine($"Получено +1{found.Name}"); Console.ReadKey(); }
            else if (cell == 'S') { Execution.Player_1.stone++; Console.WriteLine($"Получено +1{found.Name}"); Console.ReadKey(); }
            else if (cell == 'B') { Execution.Player_1.PlayerRPGClass_1.class_state.damage += 2; Console.WriteLine($"Получен бонус +2 к урону"); Console.ReadKey(); }// бонус к урону

            // Удаляем ресурс с карты
            GameState.map[Execution.cordy, Execution.cordx] = ' ';

            //убераем объект из списка
            ResourseList.Remove(found);
        }
    }

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

    // ========== КЛАСС ИГРОКА ==========
    // Хранит все данные конкретного игрока: позицию, ресурсы, юнитов, класс
    public class Player
    {
        public int player_hp = 100; // хп игрока
        // Координаты игрока на карте
        public int player_y = 1; // Начальная позиция по Y
        public int player_x = 1; // Начальная позиция по X

        // Игровые характеристики
        public int count_step;   // Количество оставшихся шагов в текущий день

        // Ресурсы игрока
        public int gold = 0;  // Золото
        public int wood = 0;  // Дерево
        public int stone = 0; // Камень

        // Список слотов с юнитами
        public List<UnitSlot> UnitList = new();

        // Список зданий игрока
        public List<Building> BuildingList = new();

        // Объект RPG-класс игрока (Рыцарь/Маг/Рейнджер)
        public PlayerRPGClass PlayerRPGClass_1 = new PlayerRPGClass();

        // Метод поиска индекса слота с заданным типом юнита
        // Возвращает индекс слота или -1 если такого типа нет
        public int FindUnitIndex(UnitSlot.UnitRPG_class type)
        {
            for (int i = 0; i < UnitList.Count; i++)
            {
                if (UnitList[i].UnitClass == type)
                {
                    return i; // Найден слот с нужным типом
                }
            }
            return -1; // Слот не найден
        }

        // Метод найма юнитов с проверками всех условий
        // Возвращает true если найм успешен, false если нет
        public bool HireUnits(UnitSlot.UnitRPG_class type, int amount)
        {
            // Проверка 1: количество должно быть положительным
            if (amount <= 0)
            {
                Console.WriteLine("Количество должно быть больше нуля");
                return false;
            }

            // Проверка 2: нельзя нанять больше 99 юнитов за раз
            if (amount > 99)
            {
                Console.WriteLine("Превышен лимит 99 юнитов в слоте");
                return false;
            }

            // Ищем, есть ли уже слот с таким типом юнита
            int idx = FindUnitIndex(type);

            if (idx == -1) // Слота с таким типом ещё нет
            {
                // Проверка 3: проверяем лимит слотов для данного класса игрока
                if (UnitList.Count >= PlayerRPGClass_1.class_state.unit_quantity)
                {
                    Console.WriteLine("Нет свободных слотов для новых типов юнитов");
                    return false;
                }

                // Создаём новый слот и добавляем в список
                UnitSlot newUnit = new UnitSlot(type, amount);

                // Проверка 4: убедимся что юниты реально добавились (конструктор мог отклонить)
                if (newUnit.Count == 0)
                {
                    Console.WriteLine("Не удалось создать слот юнитов");
                    return false;
                }

                UnitList.Add(newUnit);
                Console.WriteLine($"Нанято {newUnit.Count} юнитов типа {type}");
                return true;
            }
            else // Слот с таким типом уже существует
            {
                // Получаем копию структуры из списка
                UnitSlot unit = UnitList[idx];

                // Проверка 5: не превысит ли добавление лимит 99
                if (unit.Count + amount > 99)
                {
                    Console.WriteLine($"В слоте уже {unit.Count} юнитов, можно добавить максимум {99 - unit.Count}");
                    return false;
                }

                // Добавляем юнитов
                if (!unit.AddUnit(amount))
                {
                    Console.WriteLine("Не удалось добавить юнитов");
                    return false;
                }

                // ВАЖНО: записываем изменённую копию обратно в список (т.к. struct)
                UnitList[idx] = unit;
                Console.WriteLine($"Добавлено {amount} юнитов типа {type}, всего в слоте: {unit.Count}");
                return true;
            }
        }
    }

    // ========== КЛАСС RPG-ХАРАКТЕРИСТИК ==========
    // Управляет типом класса персонажа (Рыцарь/Маг/Рейнджер) и его характеристиками
    public class PlayerRPGClass
    {
        // Перечисление доступных классов персонажа
        public enum PlayerRPG_class
        {
            Knight = 1,  // Рыцарь
            Magic = 2,   // Маг
            Ranger = 3,  // Рейнджер
        }

        public PlayerRPG_class PlayerRPG; // Выбранный класс персонажа

        // Характеристики класса: скорость, урон, количество слотов для юнитов
        public (int speed, int damage, int unit_quantity, int armor) class_state = (0, 0, 0, 1);

        // Метод выбора класса игроком (вызывается в начале игры)
        public void ChouseClass()
        {
            Console.WriteLine("Выберите класс: 1. Рыцарь 2. Маг 3. Рейнджер");
            try
            {
                int choice = int.Parse(Console.ReadLine()); // Читаем выбор игрока
                PlayerRPG = (PlayerRPG_class)choice;         // Преобразуем число в enum
                InitAvailableStep();                         // Инициализируем шаги
                InitClassState();                            // Инициализируем характеристики
            }
            catch (Exception)
            {
                Console.WriteLine("Некорректный ввод...");
                Console.ReadKey();
                ChouseClass();
            }
        }

        // Метод установки количества шагов в зависимости от класса
        public void InitAvailableStep()
        {
            switch ((int)PlayerRPG)
            {
                case 1: Execution.Player_1.count_step = 6; break; // Рыцарь: средняя скорость
                case 2: Execution.Player_1.count_step = 5; break; // Маг: низкая скорость
                case 3: Execution.Player_1.count_step = 8; break; // Рейнджер: высокая скорость
                default: ChouseClass(); break; // Если некорректный выбор - повтор
            }
        }

        // Метод инициализации характеристик класса
        void InitClassState()
        {
            switch ((int)PlayerRPG)
            {
                case 1: // Рыцарь: медленный, сильный, мало слотов
                    class_state.speed = 6;
                    class_state.damage = 10;
                    class_state.unit_quantity = 2;
                    class_state.armor = 5;
                    break;
                case 2: // Маг: средний во всём
                    class_state.speed = 6;
                    class_state.damage = 8;
                    class_state.unit_quantity = 3;
                    class_state.armor = 2;
                    break;
                case 3: // Рейнджер: быстрый, слабый, много слотов
                    class_state.speed = 8;
                    class_state.damage = 6;
                    class_state.unit_quantity = 5;
                    class_state.armor = 3;
                    break;
            }
        }
    }

    // ========== СТРУКТУРА СЛОТА ЮНИТОВ ==========
    // Хранит информацию об одном типе юнитов и их количестве
    public struct UnitSlot
    {
        const int MaxUnit = 99; // Максимальное количество юнитов в одном слоте

        // Перечисление возможных классов юнитов
        public enum UnitRPG_class
        {
            barbarian = 1, // Варвар
            knight = 2,    // Рыцарь
            magic = 3,     // Маг
        }

        // Свойство типа юнита (только для чтения, задаётся при создании)
        public UnitRPG_class UnitClass { get; }

        // Свойство количества юнитов (чтение всем, изменение только внутри структуры)
        public int Count { get; private set; }

        // Конструктор: создаёт слот с заданным типом и количеством
        public UnitSlot(UnitRPG_class unitClass, int count)
        {
            UnitClass = unitClass; // Задаём тип юнита
            Count = 0;             // Обязательная инициализация перед использованием
            AddUnit(count);        // Добавляем юнитов через метод с проверками
        }

        // Метод добавления юнитов в слот с проверкой лимита
        // Возвращает true если удалось добавить, false если превышен лимит
        public bool AddUnit(int amount)
        {
            // Проверка: количество должно быть положительным и не превышать лимит
            if (amount <= 0 || (Count + amount) > MaxUnit)
            {
                return false; // Добавление не удалось
            }

            Count += amount; // Увеличиваем количество юнитов
            return true;     // Добавление успешно
        }

        //метод сохранения юнитов
        public static void SaveGameUnit(List<UnitSlot> unitList)
        {
            using (StreamWriter writer = new StreamWriter("SaveGameUnit.txt", false)) // Открываем файл для записи
            {
                writer.WriteLine(unitList.Count);// Записываем количество слотов
                foreach (UnitSlot unit in unitList)
                {
                    writer.WriteLine((int)unit.UnitClass); // Тип юнита
                    writer.WriteLine(unit.Count);          // Количество
                }
            }
        }
        //метод загрузки юнитов
        public static void LoadGameUnit(List<UnitSlot> unitList)
        {
            unitList.Clear();  // Очищаем текущий список перед загрузкой
            using (StreamReader reader = new StreamReader("SaveGameUnit.txt")) // Открываем файл для записи
            {
                int slotCount = int.Parse(reader.ReadLine());
                for (int i = 0; i < slotCount; i++) // Читаем каждый слот
                {

                    string unitType = reader.ReadLine();// Читаем тип юнита
                    string count = reader.ReadLine();// Читаем количество юнитов
                                                     // Создаём слот и добавляем в список
                    UnitSlot unit = new UnitSlot((UnitRPG_class)int.Parse(unitType), int.Parse(count));
                    unitList.Add(unit);
                }
            }
        }
    }

    // ========== БАЗОВЫЙ КЛАСС ЗДАНИЯ ==========
    // Родительский класс для всех типов зданий (казарма, замок, ресурсное здание)
    public abstract class Building
    {
        public int PosY { get; protected set; } // Координата Y здания
        public int PosX { get; protected set; } // Координата X здания
        public char Symbol { get; protected set; } // Символ здания на карте

        // Абстрактный метод UI здания (переопределяется в дочерних классах)
        public abstract void BuildingUI();

        // Статический метод строительства нового здания рядом с игроком
        public static void BuildBuilding()
        {
            // Проверяем, свободна ли клетка справа от игрока
            if (GameState.map[Execution.cordy, Execution.cordx + 1] == ' ')
            {
                Console.WriteLine("Выберите постройку:");
                Console.WriteLine("1. Построить казарму (2 gold)");
                Console.WriteLine("2. Построить ресурсное здание (1 gold)");
                Console.WriteLine("3. Построить замок (3 gold)");
                Console.WriteLine("Любая другая клавиша - отмена");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": // Строим казарму
                        if (Execution.Player_1.gold >= 2)
                        {
                            Execution.Player_1.gold -= 2; // Списываем ресурсы
                            Barac newBarac = new Barac(Execution.cordy, Execution.cordx + 1);
                            GameState.map[Execution.cordy, Execution.cordx + 1] = 'H';
                            Execution.Player_1.BuildingList.Add(newBarac);
                            Console.WriteLine("Казарма построена!");
                        }
                        else
                        {
                            Console.WriteLine("У вас недостаточно золота");
                        }
                        break;

                    case "2": // Строим ресурсное здание
                        if (Execution.Player_1.gold >= 1)
                        {
                            Execution.Player_1.gold -= 1; // Списываем ресурсы
                            ResourceBuilding newResource = new ResourceBuilding(Execution.cordy, Execution.cordx + 1);
                            GameState.map[Execution.cordy, Execution.cordx + 1] = 'R';
                            Execution.Player_1.BuildingList.Add(newResource);
                            ResourceBuilding.ResourceBuildingCounter += 1;
                            Console.WriteLine("Ресурсное здание построено!");
                        }
                        else
                        {
                            Console.WriteLine("У вас недостаточно золота");
                        }
                        break;

                    case "3": // Строим замок
                        if (Execution.Player_1.gold >= 3)
                        {
                            Execution.Player_1.gold -= 3; // Списываем ресурсы
                            Castle newCastle = new Castle(Execution.cordy, Execution.cordx + 1);
                            GameState.map[Execution.cordy, Execution.cordx + 1] = 'C';
                            Execution.Player_1.BuildingList.Add(newCastle);
                            Console.WriteLine("Замок построен!");
                        }
                        else
                        {
                            Console.WriteLine("У вас недостаточно золота");
                        }
                        break;

                    default:
                        Console.WriteLine("Строительство отменено");
                        break;
                }

                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Клетка справа занята! Нельзя построить здание.");
                Console.ReadKey();
            }
        }

        // метод проверки постройки на клетке шага и вызова UI
        public static void OnPlayerStep(int y, int x, ref bool cancel)
        {
            Building found = null;
            // Проходим по всем зданиям игрока
            foreach (Building building in Execution.Player_1.BuildingList)
            {
                // Если координаты совпадают - возвращаем здание
                if (building.PosY == y && building.PosX == x)
                {
                    found = building; break;
                }
            }
            if (found != null)
            {
                found.BuildingUI();
                cancel = true;
            }
        }
    }

    // ========== КЛАСС КАЗАРМЫ ==========
    // Здание для найма юнитов
    public class Barac : Building
    {
        static public int BaracCounter = 0;
        // Конструктор казармы
        public Barac(int y, int x)
        {
            PosY = y;      // Устанавливаем координату Y
            PosX = x;      // Устанавливаем координату X
            Symbol = 'H';  // Символ казармы на карте
        }

        // Метод UI казармы (меню найма юнитов)
        public override void BuildingUI()
        {
            Console.WriteLine();
            Console.WriteLine("=== КАЗАРМА ===");
            Console.WriteLine("Вы вошли в казарму, выберите тип нанимаемого юнита:");
            Console.WriteLine("1. Нанять варваров");
            Console.WriteLine("2. Нанять рыцарей");
            Console.WriteLine("3. Нанять магов");
            Console.WriteLine("Любая другая клавиша - выход из казармы");

            string choice = Console.ReadLine(); // Читаем выбор игрока

            // Если выбор некорректный - выход
            if (choice != "1" && choice != "2" && choice != "3")
            {
                return;
            }

            // Преобразуем выбор в тип юнита
            UnitSlot.UnitRPG_class unitType = (UnitSlot.UnitRPG_class)int.Parse(choice);

            // Спрашиваем количество
            Console.WriteLine("Количество нанимаемых воинов (макс в слоте 99):");
            int count = int.Parse(Console.ReadLine());

            // Вызываем метод найма через объект игрока
            Execution.Player_1.HireUnits(unitType, count);

            // Пауза для чтения сообщения
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }

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

    // ========== КЛАСС РЕСУРСНОГО ЗДАНИЯ ==========
    // Здание, приносящее ресурсы каждую неделю
    public class ResourceBuilding : Building
    {
        static public int NewResurseTimer = 0;
        static public int ResourceBuildingCounter = 0;

        // Конструктор ресурсного здания
        public ResourceBuilding(int y, int x)
        {
            PosY = y;      // Устанавливаем координату Y
            PosX = x;      // Устанавливаем координату X
            Symbol = 'R';  // Символ ресурсного здания на карте
        }

        // UI ресурсного здания
        public override void BuildingUI()
        {
            Console.WriteLine();
            Console.WriteLine("=== РЕСУРСНОЕ ЗДАНИЕ ===");
            Console.WriteLine("Это ресурсная постройка");
            Console.WriteLine("Она приносит +1 gold, +1 wood, +1 stone каждую неделю");
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }

    // ========== КЛАСС ПРЕДМЕТОВ ==========
    public class Item
    {
        public string Name { get; set; }
        public char Symbol { get; set; }
        public string Description { get; set; }
        public int HudIndex { get; set; }

        public Item(string name, char symbol, string description, int hudindex)
        {
            Name = name;
            Symbol = symbol;
            Description = description;
            HudIndex = hudindex;
        }

    }

    // ========== КЛАСС ИНВЕНТАРЯ ==========
    // Управляет сохранением и загрузкой юнитов игрока
    public class Inventory
    {
        public static List<Item> ItemEquipList = new();
        public static List<Item> ItemNotEquipList = new()
    {
        new Item("EpicHelmet", 'H', "Эпический шлем (+10 защита)", 0),
        new Item("RareHelmet", 'H', "Редкий шлем (+5 защита)", 0),
        new Item("LegendSword", 'S', "Легендарный меч (+10 урон)", 3),
        new Item("Shield", 'D', "Щит (+3 защита)", 1)
    };
        public static char[] ItemEquipHud = new char[6] { ' ', ' ', ' ', ' ', ' ', ' ' };

        public static void InventoryUI()
        {
            // Отрисовка HUD экипировки
            Console.Clear();
            Console.WriteLine($"    [{ItemEquipHud[0]}]     - Слот для шлема");
            Console.WriteLine($"[{ItemEquipHud[1]}] [{ItemEquipHud[2]}] [{ItemEquipHud[3]}] - Слот для меча, щита и нагрудника");
            Console.WriteLine($"    [{ItemEquipHud[4]}]     - Слот для понож");
            Console.WriteLine($"    [{ItemEquipHud[5]}]     - Слот для ботинок");
            Console.WriteLine("Доступные предметы в для экиперовки:");
            int counter = 1; //счетчик для отрисовки доступных предметов
            foreach (Item item in ItemNotEquipList)
            {
                Console.WriteLine($"{counter}. [{item.Symbol}] - {item.Name}, {item.Description} ");
                counter++;
            }
            Console.Write("Любая клавиша выход");
            Console.Write("Выберите предмет который хотите экипировать:");
            try // логика выбора предметов
            {
                int PlayerChoise = int.Parse(Console.ReadLine());
                var SelectedItem = ItemNotEquipList[PlayerChoise - 1];//выбранный предмет из списка неэкеперованных
                if (ItemEquipHud[SelectedItem.HudIndex] == ' ')// проверка надет-ли предмет с такимже HudIndex
                {
                    ItemEquipList.Add(ItemNotEquipList[PlayerChoise - 1]);// добавляем его в список надетых

                    ItemNotEquipList.RemoveAt(PlayerChoise - 1); // удаляем его из списка НЕнадетых

                    ItemEquipHud[SelectedItem.HudIndex] = SelectedItem.Symbol;//добавляем его значек в массив для отображения
                }
                else
                {
                    // Найти предмет с таким же HudIndex в списке экипированных
                    Item oldItem = ItemEquipList.Find(item => item.HudIndex == SelectedItem.HudIndex);

                    // Снимаем старый предмет удаляя его из списка экиперованных
                    ItemEquipList.Remove(oldItem);

                    ItemNotEquipList.Add(oldItem);// добовляем его в список НЕэкиперованных

                    // Надеваем новый предмет
                    ItemEquipList.Add(SelectedItem);
                    ItemNotEquipList.RemoveAt(PlayerChoise - 1);
                    ItemEquipHud[SelectedItem.HudIndex] = SelectedItem.Symbol;
                }
                InventoryUI();
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void SaveInventory()
        {
            using (StreamWriter writer = new StreamWriter("SaveGameInventory.txt", false)) // Открываем файл для записи
            {
                writer.WriteLine(ItemEquipList.Count);// Записываем количество слотов из списка экиперованных
                writer.WriteLine(ItemNotEquipList.Count);// Записываем количество слотов из списка неэкиперованных предметов
                foreach (Item item in ItemEquipList)
                {
                    writer.WriteLine(item.Name);
                    writer.WriteLine(item.Symbol);
                    writer.WriteLine(item.Description);
                    writer.WriteLine(item.HudIndex);
                }
                foreach (Item item in ItemNotEquipList)
                {
                    writer.WriteLine(item.Name);
                    writer.WriteLine(item.Symbol);
                    writer.WriteLine(item.Description);
                    writer.WriteLine(item.HudIndex);
                }
                foreach (char hudsymbol in ItemEquipHud)
                {
                    writer.WriteLine(hudsymbol);
                }
            }

        }
        public static void LoadInventory()
        {
            ItemEquipList.Clear();  // Очищаем текущий список перед загрузкой
            ItemNotEquipList.Clear();  // Очищаем текущий список перед загрузкой
            using (StreamReader reader = new StreamReader("SaveGameInventory.txt")) // Открываем файл для записи
            {
                int ItemEquipListCount = int.Parse(reader.ReadLine());
                int ItemNotEquipListCount = int.Parse(reader.ReadLine());
                for (int i = 0; i < ItemEquipListCount; i++) // Читаем каждый слот
                {
                    string name = reader.ReadLine();// Читаем имя предмета
                    char symbol = (char)reader.ReadLine()[0];// Читаем символ предмета
                    string description = reader.ReadLine();
                    string hudindex = reader.ReadLine();
                    // Создаём слот и добавляем в список
                    Item item = new Item(name, symbol, description, int.Parse(hudindex));
                    ItemEquipList.Add(item);
                }
                for (int i = 0; i < ItemNotEquipListCount; i++) // Читаем каждый слот
                {
                    string name = reader.ReadLine();// Читаем имя предмета
                    char symbol = (char)reader.ReadLine()[0];// Читаем символ предмета
                    string description = reader.ReadLine();
                    string hudindex = reader.ReadLine();
                    // Создаём слот и добавляем в список
                    Item item = new Item(name, symbol, description, int.Parse(hudindex));
                    ItemNotEquipList.Add(item);
                }
                for (int i = 0; i < 6; i++)
                {
                    ItemEquipHud[i] = reader.ReadLine()[0];
                }
            }
        }
    }

    // ========== КЛАСС Monster ==========
    // Задаются расположение и характеристеки монстров
    class Monster
    {
        public static List<Monster> MonsterList = new();
        public int monster_hp { get; set; }
        public int monster_damage { get; set; }
        public int monster_armor { get; set; }
        public string monster_name { get; set; }
        public int monster_cordx { get; set; }
        public int monster_cordy { get; set; }
        public char monster_char = 'M';

        public Monster(int monster_hp, int monster_damage,int monster_armor, string monster_name, int monster_cordx, int monster_cordy)
        {
            this.monster_hp = monster_hp;
            this.monster_damage = monster_damage;
            this.monster_armor = monster_armor;// не может быть равно 0
            this.monster_name = monster_name;
            this.monster_cordx = monster_cordx;
            this.monster_cordy = monster_cordy;
        }

        //метод с окном битвы
        public static void MonsterFight(Monster monster)
        {
            Console.Clear();
            Console.WriteLine($"Вы встретили {monster.monster_name}");
            Console.WriteLine($"Здоровье монстра - {monster.monster_hp}      Ваше Здоровье - {Execution.Player_1.player_hp}");
            Console.WriteLine($"Урон монстра - {monster.monster_damage}      Ваш урон - {Execution.Player_1.PlayerRPGClass_1.class_state.damage}");
            Console.WriteLine($"Броня монстра - {monster.monster_armor}      Ваша броня - {Execution.Player_1.PlayerRPGClass_1.class_state.armor}");
            Console.WriteLine("Вы хотите сразиться с ним?");
            Console.WriteLine("1 - ДА");
            Console.WriteLine("Любая клавиша - НЕТ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // БОЙ: пока оба живы (HP > 0)
                    while (monster.monster_hp > 0 && Execution.Player_1.player_hp > 0)
                    {
                        // расчет урона игрока
                        int damageToMonster = (int)(Execution.Player_1.PlayerRPGClass_1.class_state.damage * (1 - monster.monster_armor * 0.08));
                        monster.monster_hp -= damageToMonster;

                        // расчет урона монстра
                        if (monster.monster_hp > 0)
                        {
                            int damageToPlayer = (int)(monster.monster_damage * (1 - Execution.Player_1.PlayerRPGClass_1.class_state.armor * 0.08));
                            Execution.Player_1.player_hp -= damageToPlayer;
                        }

                        // Показываем текущее HP
                        Console.WriteLine($"Здоровье монстра - {monster.monster_hp}      Ваше Здоровье - {Execution.Player_1.player_hp}");

                        // Проверка: кто-то умер? тогда выходим
                        if (monster.monster_hp <= 0 || Execution.Player_1.player_hp <= 0)
                        {
                            break;
                        }

                        // Предлагаем продолжить или сбежать
                        Console.WriteLine("Вы хотите продолжить бой?");
                        Console.WriteLine("1 - ДА");
                        Console.WriteLine("Любая клавиша - Сбежать");
                        var fightchoice = Console.ReadKey();
                        Console.WriteLine(); // перевод строки

                        if (fightchoice.Key == ConsoleKey.D1)
                        {
                            // Продолжаем бой
                        }
                        else
                        {
                            Console.WriteLine("Вы сбежали из боя!");
                            Console.ReadKey();
                            return; // выходим из метода
                        }
                    }

                    // === ПРОВЕРКА ПОБЕДИТЕЛЯ ===
                    Console.WriteLine("\n====================");
                    if (Execution.Player_1.player_hp > 0)
                    {
                        Console.WriteLine($"ПОБЕДА! Вы победили {monster.monster_name}!");
                        Console.WriteLine($"У вас осталось {Execution.Player_1.player_hp} HP");
                        MonsterList.Remove(monster);
                        GameState.map[monster.monster_cordy, monster.monster_cordx] = ' ';
                    }
                    else
                    {
                        Console.WriteLine("ПОРАЖЕНИЕ! Вы погибли...");
                        Console.WriteLine($"У монстра осталось {monster.monster_hp} HP");
                    }
                    Console.WriteLine("====================");
                    Console.WriteLine("Нажмите любую клавишу...");
                    Console.ReadKey();
                    break;

                default:
                    Console.WriteLine("Вы решили не сражаться.");
                    Console.ReadKey();
                    break;
            }
        }

        // метод проверки монстра на клетке шага и вызова битвы
        public static void OnPlayerStep(int y, int x, ref bool cancel)
        {
            Monster found = null;
            // Проходим по всем монстрам
            foreach (Monster monster in MonsterList)
            {
                // Если координаты совпадают - возвращаем монстра
                if (monster.monster_cordy == y && monster.monster_cordx == x)
                {
                    found = monster; break;
                }
            }
            if (found != null)
            {
                MonsterFight(found);
                cancel = true;
            }
        }

    }

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
            else {cancel = false;}
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

