namespace My_Game

{
    // ========== ÈÍÒÅÐÔÅÉÑ ÄËß ÐÅÑÓÐÑÎÂ ==========
    // ========== ÊËÀÑÑ Ñ ÐÅÀËÈÇÀÖÈÅÉ ÈÍÒÅÐÔÅÉÑÀ ==========

    public class Resourses : ICollectable
    {
        public enum ResourceType
        {
            Gold,
            Wood,
            Stone,
            Bonus
        }
        public ResourceType Type { get; }    // Òèï ðåñóðñà

        // Ñèìâîë âû÷èñëÿåòñÿ êàæäûé ðàç èç Type
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

        // Èìÿ âû÷èñëÿåòñÿ èç Type
        public string Name
        {
            get
            {
                switch (Type)
                {
                    case ResourceType.Gold: return "Çîëîòî";
                    case ResourceType.Wood: return "Äåðåâî";
                    case ResourceType.Stone: return "Êàìåíü";
                    case ResourceType.Bonus: return "Áîíóñ";
                    default: return "Íåèçâåñòíî";
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
            cancel = true; // âñåãäà ðàçðåøàåò ïåðåìåùåíèå
            char cell = GameState.map[y, x]; // ÷òî íà êëåòêå

            // Áûñòðûé ôèëüòð: åñëè ýòî íå ðåñóðñ — âûõîäèì
            if (cell != 'G' && cell != 'W' && cell != 'S' && cell != 'B')
                return;

            // Íàõîäèì ðåñóðñ, êîòîðûé ñîîòâåòñòâóåò ñèìâîëó êëåòêè
            Resourses found = null;
            foreach (var res in ResourseList)
            {
                if (res.Symbol == cell)
                {
                    found = res;
                    break; // íàøëè — äàëüøå íå èùåì
                }
            }
            // Âûäà¸ì ýôôåêò â çàâèñèìîñòè îò ñèìâîëà êëåòêè
            if (cell == 'G') { Execution.Player_1.gold++; Console.WriteLine($"Ïîëó÷åíî +1{found.Name}"); Console.ReadKey(); }
            else if (cell == 'W') { Execution.Player_1.wood++; Console.WriteLine($"Ïîëó÷åíî +1{found.Name}"); Console.ReadKey(); }
            else if (cell == 'S') { Execution.Player_1.stone++; Console.WriteLine($"Ïîëó÷åíî +1{found.Name}"); Console.ReadKey(); }
            else if (cell == 'B') { Execution.Player_1.PlayerRPGClass_1.class_state.damage += 2; Console.WriteLine($"Ïîëó÷åí áîíóñ +2 ê óðîíó"); Console.ReadKey(); }// áîíóñ ê óðîíó

            // Óäàëÿåì ðåñóðñ ñ êàðòû
            GameState.map[Execution.cordy, Execution.cordx] = ' ';

            //óáåðàåì îáúåêò èç ñïèñêà
            ResourseList.Remove(found);
        }
    }
}