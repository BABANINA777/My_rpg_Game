namespace My_Game

{
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
}