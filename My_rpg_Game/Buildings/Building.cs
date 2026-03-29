namespace My_Game
{
    // ========== ÁÀÇÎÂÛÉ ÊËÀÑÑ ÇÄÀÍÈß ==========
    // Ðîäèòåëüñêèé êëàññ äëÿ âñåõ òèïîâ çäàíèé (êàçàðìà, çàìîê, ðåñóðñíîå çäàíèå)
    public abstract class Building
    {
        public int PosY { get; protected set; } // Êîîðäèíàòà Y çäàíèÿ
        public int PosX { get; protected set; } // Êîîðäèíàòà X çäàíèÿ
        public char Symbol { get; protected set; } // Ñèìâîë çäàíèÿ íà êàðòå

        // Àáñòðàêòíûé ìåòîä UI çäàíèÿ (ïåðåîïðåäåëÿåòñÿ â äî÷åðíèõ êëàññàõ)
        public abstract void BuildingUI();

        // Ñòàòè÷åñêèé ìåòîä ñòðîèòåëüñòâà íîâîãî çäàíèÿ ðÿäîì ñ èãðîêîì
        public static void BuildBuilding()
        {
            // Ïðîâåðÿåì, ñâîáîäíà ëè êëåòêà ñïðàâà îò èãðîêà
            if (GameState.map[Execution.cordy, Execution.cordx + 1] == ' ')
            {
                Console.WriteLine("Âûáåðèòå ïîñòðîéêó:");
                Console.WriteLine("1. Ïîñòðîèòü êàçàðìó (2 gold)");
                Console.WriteLine("2. Ïîñòðîèòü ðåñóðñíîå çäàíèå (1 gold)");
                Console.WriteLine("3. Ïîñòðîèòü çàìîê (3 gold)");
                Console.WriteLine("4. Ïîñòðîèòü ìàãàçèí (4 gold)");
                Console.WriteLine("Ëþáàÿ äðóãàÿ êëàâèøà - îòìåíà");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": // Ñòðîèì êàçàðìó
                        if (Execution.Player_1.gold >= 2)
                        {
                            Execution.Player_1.gold -= 2; // Ñïèñûâàåì ðåñóðñû
                            Barac newBarac = new Barac(Execution.cordy, Execution.cordx + 1);
                            GameState.map[Execution.cordy, Execution.cordx + 1] = 'H';
                            Execution.Player_1.BuildingList.Add(newBarac);
                            Console.WriteLine("Êàçàðìà ïîñòðîåíà!");
                        }
                        else
                        {
                            Console.WriteLine("Ó âàñ íåäîñòàòî÷íî çîëîòà");
                        }
                        break;

                    case "2": // Ñòðîèì ðåñóðñíîå çäàíèå
                        if (Execution.Player_1.gold >= 1)
                        {
                            Execution.Player_1.gold -= 1; // Ñïèñûâàåì ðåñóðñû
                            ResourceBuilding newResource = new ResourceBuilding(Execution.cordy, Execution.cordx + 1);
                            GameState.map[Execution.cordy, Execution.cordx + 1] = 'R';
                            Execution.Player_1.BuildingList.Add(newResource);
                            ResourceBuilding.ResourceBuildingCounter += 1;
                            Console.WriteLine("Ðåñóðñíîå çäàíèå ïîñòðîåíî!");
                        }
                        else
                        {
                            Console.WriteLine("Ó âàñ íåäîñòàòî÷íî çîëîòà");
                        }
                        break;

                    case "3": // Ñòðîèì çàìîê
                        if (Execution.Player_1.gold >= 3)
                        {
                            Execution.Player_1.gold -= 3; // Ñïèñûâàåì ðåñóðñû
                            Castle newCastle = new Castle(Execution.cordy, Execution.cordx + 1);
                            GameState.map[Execution.cordy, Execution.cordx + 1] = 'C';
                            Execution.Player_1.BuildingList.Add(newCastle);
                            Console.WriteLine("Çàìîê ïîñòðîåí!");
                        }
                        else
                        {
                            Console.WriteLine("Ó âàñ íåäîñòàòî÷íî çîëîòà");
                        }
                        break;
                    case "4": // Ñòðîèì ìàãàçèí
                        if (Execution.Player_1.gold >= 4)
                        {
                            Execution.Player_1.gold -= 4;
                            Shop newShop = new Shop(Execution.cordy, Execution.cordx + 1);
                            GameState.map[Execution.cordy, Execution.cordx + 1] = '$'; // Ðèñóåì ñèìâîë $ íà êàðòå
                            Execution.Player_1.BuildingList.Add(newShop);
                            Console.WriteLine("Ìàãàçèí ïîñòðîåí!");
                        }
                        else
                        {
                            Console.WriteLine("Ó âàñ íåäîñòàòî÷íî çîëîòà");
                        }
                        break;

                    default:
                        Console.WriteLine("Ñòðîèòåëüñòâî îòìåíåíî");
                        break;
                }

                Console.WriteLine("Íàæìèòå ëþáóþ êëàâèøó...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Êëåòêà ñïðàâà çàíÿòà! Íåëüçÿ ïîñòðîèòü çäàíèå.");
                Console.ReadKey();
            }
        }

        // ìåòîä ïðîâåðêè ïîñòðîéêè íà êëåòêå øàãà è âûçîâà UI
        public static void OnPlayerStep(int y, int x, ref bool cancel)
        {
            Building found = null;
            // Ïðîõîäèì ïî âñåì çäàíèÿì èãðîêà
            foreach (Building building in Execution.Player_1.BuildingList)
            {
                // Åñëè êîîðäèíàòû ñîâïàäàþò - âîçâðàùàåì çäàíèå
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