using System;

namespace My_Game

{
    // ========== ИНТЕРФЕЙС ДЛЯ РЕСУРСОВ ==========
    // ========== КЛАСС С РЕАЛИЗАЦИЕЙ ИНТЕРФЕЙСА ==========

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
}