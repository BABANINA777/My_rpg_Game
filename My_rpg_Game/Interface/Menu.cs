namespace My_Game
{
    class Menu
    {
        public static void DrawMenu()
        {
            Console.Clear();
            Console.WriteLine("Основное меню игры");
            Console.WriteLine("1. Ваши квесты");
            Console.WriteLine("2. Управление");
            Console.WriteLine("3. Сохранить и выйти");
            Console.WriteLine("4. Выход из игры");

            int choice = int.Parse(Console.ReadLine()); // Читаем выбор игрока

            switch (choice)
            {
                case 1:
                    DrawQuest(GameState.Instance.ActiveQuests[0]);
                    break;
                case 2:
                    DrawControlInfo();
                    break;
                case 3:
                    GameState.Instance.SaveGame();
                    Environment.Exit(0);
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
                default:
                    return;
            }
        }
        public static void DrawQuest(Quest quest)
        {
            quest.QuestInfo();
            Console.ReadKey();
        }

        public static void DrawControlInfo()
        {
            // Подсказки управления
            Console.WriteLine("_______________________");
            Console.WriteLine("Управление: Стрелки - движение, C - строить, Enter - новый день, F5 - сохранить, F9 - загрузить, I - инвентарь");
            Console.ReadKey();
        }


    }
}
