namespace My_Game

{
    // ========== ÊËÀÑÑ ÇÀÌÊÀ ==========
    // Çäàíèå äëÿ ïîëó÷åíèÿ âðåìåííûõ áîíóñîâ ê õàðàêòåðèñòèêàì
    public class Castle : Building
    {
        static public int CastleCounter = 0;
        // Êîíñòðóêòîð çàìêà

        // Òàéìåð áîíóñà çàìêà (â äíÿõ)
        public static int bonusTimer = 0;
        public Castle(int y, int x)
        {
            PosY = y;      // Óñòàíàâëèâàåì êîîðäèíàòó Y
            PosX = x;      // Óñòàíàâëèâàåì êîîðäèíàòó X
            Symbol = 'C';  // Ñèìâîë çàìêà íà êàðòå
        }

        // Ìåòîä àêòèâàöèè áîíóñà (âûçûâàåòñÿ ïðè íàæàòèè Space â çàìêå)
        public static void ActivateBonus()
        {
            // Ïðîâåðÿåì, íå àêòèâèðîâàí ëè óæå áîíóñ
            if (Castle.bonusTimer <= 0)
            {
                // Óñòàíàâëèâàåì òàéìåð íà 7 äíåé
                Castle.bonusTimer = 7;

                // Ïðèìåíÿåì áîíóñû
                Execution.Player_1.PlayerRPGClass_1.class_state.speed += 2;
                Execution.Player_1.PlayerRPGClass_1.class_state.damage += 2;
                Execution.Player_1.PlayerRPGClass_1.class_state.unit_quantity += 1;

                Console.WriteLine("Áîíóñ àêòèâèðîâàí íà 7 äíåé!");
            }
            else
            {
                Console.WriteLine($"Áîíóñ óæå àêòèâåí! Îñòàëîñü äíåé: {Castle.bonusTimer}");
            }
        }

        // Ìåòîä ñíÿòèÿ áîíóñà (âûçûâàåòñÿ êîãäà òàéìåð çàêàí÷èâàåòñÿ)
        public static void DeactivateBonus()
        {
            // Óáèðàåì áîíóñû
            Execution.Player_1.PlayerRPGClass_1.class_state.speed -= 2;
            Execution.Player_1.PlayerRPGClass_1.class_state.damage -= 2;
            Execution.Player_1.PlayerRPGClass_1.class_state.unit_quantity -= 1;

            Console.WriteLine("Áîíóñ çàìêà çàêîí÷èëñÿ!");
        }

        // UI çàìêà
        public override void BuildingUI()
        {
            Console.WriteLine();
            Console.WriteLine("=== ÇÀÌÎÊ ===");
            Console.WriteLine("Äîáðî ïîæàëîâàòü â çàìîê!");
            Console.WriteLine("Íàæìèòå Space ÷òîáû ïîëó÷èòü áîíóñ:");
            Console.WriteLine("+2 ñêîðîñòü, +2 óðîí, +1 ñëîò þíèòîâ (íà 7 äíåé)");
            Console.WriteLine($"Îñòàëîñü äíåé áîíóñà: {Castle.bonusTimer}");
            Console.WriteLine("Ëþáàÿ äðóãàÿ êëàâèøà - âûõîä");

            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.Spacebar)
            {
                ActivateBonus();
            }

            Console.WriteLine("\nÍàæìèòå ëþáóþ êëàâèøó...");
            Console.ReadKey();
        }
    }
}