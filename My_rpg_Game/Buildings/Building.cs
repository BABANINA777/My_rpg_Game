using System;
namespace My_Game
{
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

}