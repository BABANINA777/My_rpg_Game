namespace My_Game

{
    // ========== ÊËÀÑÑ ÊÀÇÀÐÌÛ ==========
    // Çäàíèå äëÿ íàéìà þíèòîâ
    public class Barac : Building
    {
        static public int BaracCounter = 0;
        // Êîíñòðóêòîð êàçàðìû
        public Barac(int y, int x)
        {
            PosY = y;      // Óñòàíàâëèâàåì êîîðäèíàòó Y
            PosX = x;      // Óñòàíàâëèâàåì êîîðäèíàòó X
            Symbol = 'H';  // Ñèìâîë êàçàðìû íà êàðòå
        }

        // Ìåòîä UI êàçàðìû (ìåíþ íàéìà þíèòîâ)
        public override void BuildingUI()
        {
            Console.WriteLine();
            Console.WriteLine("=== ÊÀÇÀÐÌÀ ===");
            Console.WriteLine("Âû âîøëè â êàçàðìó, âûáåðèòå òèï íàíèìàåìîãî þíèòà:");
            Console.WriteLine("1. Íàíÿòü âàðâàðîâ");
            Console.WriteLine("2. Íàíÿòü ðûöàðåé");
            Console.WriteLine("3. Íàíÿòü ìàãîâ");
            Console.WriteLine("Ëþáàÿ äðóãàÿ êëàâèøà - âûõîä èç êàçàðìû");

            string choice = Console.ReadLine(); // ×èòàåì âûáîð èãðîêà

            // Åñëè âûáîð íåêîððåêòíûé - âûõîä
            if (choice != "1" && choice != "2" && choice != "3")
            {
                return;
            }

            // Ïðåîáðàçóåì âûáîð â òèï þíèòà
            UnitSlot.UnitRPG_class unitType = (UnitSlot.UnitRPG_class)int.Parse(choice);

            // Ñïðàøèâàåì êîëè÷åñòâî
            Console.WriteLine("Êîëè÷åñòâî íàíèìàåìûõ âîèíîâ (ìàêñ â ñëîòå 99):");
            int count = int.Parse(Console.ReadLine());

            // Âûçûâàåì ìåòîä íàéìà ÷åðåç îáúåêò èãðîêà
            Execution.Player_1.HireUnits(unitType, count);

            // Ïàóçà äëÿ ÷òåíèÿ ñîîáùåíèÿ
            Console.WriteLine("Íàæìèòå ëþáóþ êëàâèøó äëÿ ïðîäîëæåíèÿ...");
            Console.ReadKey();
        }
    }
}