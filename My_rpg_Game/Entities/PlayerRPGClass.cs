namespace My_Game

{
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
}