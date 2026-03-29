namespace My_Game

{
    // ========== ÊËÀÑÑ Monster ==========
    // Çàäàþòñÿ ðàñïîëîæåíèå è õàðàêòåðèñòåêè ìîíñòðîâ
    class Monster : IComparable<Monster>
    {

        public static List<Monster> MonsterList = new();
        public int monster_hp { get; set; }
        public int monster_damage { get; set; }
        public int monster_armor { get; set; }
        public string monster_name { get; set; }
        public int monster_cordx { get; set; }
        public int monster_cordy { get; set; }
        public char monster_char = 'M';

        // Âû÷èñëÿåìîå ñâîéñòâî äëÿ "Î÷êîâ ñèëû" ìîíñòðà
        // Îíî àâòîìàòè÷åñêè ñ÷èòàåò çíà÷åíèå ïî òâîåé ôîðìóëå
        public double PowerScore
        {
            get
            {
                // 1. Âû÷èñëÿåì ìíîæèòåëü ñíèæåíèÿ óðîíà (ïî ïðàâèëàì òâîåãî áîÿ)
                double damageReduction = 1.0 - (monster_armor * 0.08);

                // Çàùèòà îò áåññìåðòíûõ ìîíñòðîâ (åñëè áðîíÿ >= 13, ñíèæåíèå áóäåò <= 0)
                if (damageReduction <= 0.05)
                {
                    damageReduction = 0.05; // Îñòàâëÿåì ìîíñòðó ïîëó÷åíèå õîòÿ áû 5% óðîíà
                }

                // 2. Ñ÷èòàåì ýôôåêòèâíîå çäîðîâüå
                double effectiveHp = monster_hp / damageReduction;

                // 3. Èòîãîâàÿ ñèëà: Ýôôåêòèâíîå çäîðîâüå * Óðîí
                return effectiveHp * monster_damage;
            }
        }

        // ðåàëèçàöèÿ èíòåðôåéñà äëÿ ñîðòèðîâêè
        public int CompareTo(Monster other)
        {
            if (other == null) return 1;

            // Ñðàâíèâàåì çäîðîâüå òåêóùåãî þíèòà ñî çäîðîâüåì äðóãîãî
            return this.PowerScore.CompareTo(other.PowerScore);
        }

        //
        public int MonsterCheck(Monster monster)
        {
            if (monster.PowerScore > 30) return 1;
            else;
            return 0;
        }

        public Monster(int monster_hp, int monster_damage, int monster_armor, string monster_name, int monster_cordx, int monster_cordy)
        {
            this.monster_hp = monster_hp;
            this.monster_damage = monster_damage;
            this.monster_armor = monster_armor;// íå ìîæåò áûòü ðàâíî 0
            this.monster_name = monster_name;
            this.monster_cordx = monster_cordx;
            this.monster_cordy = monster_cordy;
        }

        //ìåòîä ñ îêíîì áèòâû
        public static void MonsterFight(Monster monster)
        {
            Console.Clear();
            Console.WriteLine($"Âû âñòðåòèëè {monster.monster_name}");
            Console.WriteLine($"Çäîðîâüå ìîíñòðà - {monster.monster_hp}      Âàøå Çäîðîâüå - {Execution.Player_1.player_hp}");
            Console.WriteLine($"Óðîí ìîíñòðà - {monster.monster_damage}      Âàø óðîí - {Execution.Player_1.PlayerRPGClass_1.class_state.damage}");
            Console.WriteLine($"Áðîíÿ ìîíñòðà - {monster.monster_armor}      Âàøà áðîíÿ - {Execution.Player_1.PlayerRPGClass_1.class_state.armor}");
            Console.WriteLine("Âû õîòèòå ñðàçèòüñÿ ñ íèì?");
            Console.WriteLine("1 - ÄÀ");
            Console.WriteLine("Ëþáàÿ êëàâèøà - ÍÅÒ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // ÁÎÉ: ïîêà îáà æèâû (HP > 0)
                    while (monster.monster_hp > 0 && Execution.Player_1.player_hp > 0)
                    {
                        // ðàñ÷åò óðîíà èãðîêà
                        int damageToMonster = (int)(Execution.Player_1.PlayerRPGClass_1.class_state.damage * (1 - monster.monster_armor * 0.08));
                        monster.monster_hp -= damageToMonster;

                        // ðàñ÷åò óðîíà ìîíñòðà
                        if (monster.monster_hp > 0)
                        {
                            int damageToPlayer = (int)(monster.monster_damage * (1 - Execution.Player_1.PlayerRPGClass_1.class_state.armor * 0.08));
                            Execution.Player_1.player_hp -= damageToPlayer;
                        }

                        // Ïîêàçûâàåì òåêóùåå HP
                        Console.WriteLine($"Çäîðîâüå ìîíñòðà - {monster.monster_hp}      Âàøå Çäîðîâüå - {Execution.Player_1.player_hp}");

                        // Ïðîâåðêà: êòî-òî óìåð? òîãäà âûõîäèì
                        if (monster.monster_hp <= 0 || Execution.Player_1.player_hp <= 0)
                        {
                            break;
                        }

                        // Ïðåäëàãàåì ïðîäîëæèòü èëè ñáåæàòü
                        Console.WriteLine("Âû õîòèòå ïðîäîëæèòü áîé?");
                        Console.WriteLine("1 - ÄÀ");
                        Console.WriteLine("Ëþáàÿ êëàâèøà - Ñáåæàòü");
                        var fightchoice = Console.ReadKey();
                        Console.WriteLine(); // ïåðåâîä ñòðîêè

                        if (fightchoice.Key == ConsoleKey.D1)
                        {
                            // Ïðîäîëæàåì áîé
                        }
                        else
                        {
                            Console.WriteLine("Âû ñáåæàëè èç áîÿ!");
                            Console.ReadKey();
                            return; // âûõîäèì èç ìåòîäà
                        }
                    }

                    // === ÏÐÎÂÅÐÊÀ ÏÎÁÅÄÈÒÅËß ===
                    Console.WriteLine("\n====================");
                    if (Execution.Player_1.player_hp > 0)
                    {
                        Console.WriteLine($"ÏÎÁÅÄÀ! Âû ïîáåäèëè {monster.monster_name}!");
                        Console.WriteLine($"Ó âàñ îñòàëîñü {Execution.Player_1.player_hp} HP");
                        MonsterList.Remove(monster);
                        GameState.map[monster.monster_cordy, monster.monster_cordx] = ' ';
                    }
                    else
                    {
                        GameState.GameOver();
                    }
                    Console.WriteLine("====================");
                    Console.WriteLine("Íàæìèòå ëþáóþ êëàâèøó...");
                    Console.ReadKey();

                    break;

                default:
                    Console.WriteLine("Âû ðåøèëè íå ñðàæàòüñÿ.");
                    Console.ReadKey();
                    break;
            }
        }

        // ìåòîä ïðîâåðêè ìîíñòðà íà êëåòêå øàãà è âûçîâà áèòâû
        public static void OnPlayerStep(int y, int x, ref bool cancel)
        {
            Monster found = null;
            // Ïðîõîäèì ïî âñåì ìîíñòðàì
            foreach (Monster monster in MonsterList)
            {
                // Åñëè êîîðäèíàòû ñîâïàäàþò - âîçâðàùàåì ìîíñòðà
                if (monster.monster_cordy == y && monster.monster_cordx == x)
                {
                    found = monster; break;
                }
            }
            if (found != null)
            {
                MonsterFight(found);
                cancel = true;
            }
        }

        

    }
}
