namespace My_Game

{
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
            GameState.Instance.Player_1.HireUnits(unitType, count);

            // Пауза для чтения сообщения
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}