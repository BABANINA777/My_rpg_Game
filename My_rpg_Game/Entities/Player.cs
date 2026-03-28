using System;

namespace My_Game

{
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
}