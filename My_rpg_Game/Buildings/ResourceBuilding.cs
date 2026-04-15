namespace My_Game

{
    // ========== ÊËÀÑÑ ÐÅÑÓÐÑÍÎÃÎ ÇÄÀÍÈß ==========
    // Çäàíèå, ïðèíîñÿùåå ðåñóðñû êàæäóþ íåäåëþ
    public class ResourceBuilding : Building
    {
        static public int NewResurseTimer = 0;
        static public int ResourceBuildingCounter = 0;

        // Êîíñòðóêòîð ðåñóðñíîãî çäàíèÿ
        public ResourceBuilding(int y, int x)
        {
            PosY = y;      // Óñòàíàâëèâàåì êîîðäèíàòó Y
            PosX = x;      // Óñòàíàâëèâàåì êîîðäèíàòó X
            Symbol = 'R';  // Ñèìâîë ðåñóðñíîãî çäàíèÿ íà êàðòå
        }

        // UI ðåñóðñíîãî çäàíèÿ
        public override void BuildingUI()
        {
            Console.WriteLine();
            Console.WriteLine("=== ÐÅÑÓÐÑÍÎÅ ÇÄÀÍÈÅ ===");
            Console.WriteLine("Ýòî ðåñóðñíàÿ ïîñòðîéêà");
            Console.WriteLine("Îíà ïðèíîñèò +1 gold, +1 wood, +1 stone êàæäóþ íåäåëþ");
            Console.WriteLine("Íàæìèòå ëþáóþ êëàâèøó...");
            Console.ReadKey();
        }
    }
}