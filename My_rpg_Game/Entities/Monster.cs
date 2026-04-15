namespace My_Game

{
    // ========== КЛАСС Monster ==========
    // Задаются расположение и характеристеки монстров
    public class Monster : IComparable<Monster>
    {

        
        public int monster_hp { get; set; }
        public int monster_damage { get; set; }
        public int monster_armor { get; set; }
        public string monster_name { get; set; }
        public int monster_cordx { get; set; }
        public int monster_cordy { get; set; }
        public char monster_char = 'M';
        // Вычисляемое свойство для "Очков силы" монстра
        // Оно автоматически считает значение поформуле
        public double PowerScore
        {
            get
            {
                // 1. Вычисляем множитель снижения урона (по правилам твоего боя)
                double damageReduction = 1.0 - (monster_armor * 0.08);

                // Защита от бессмертных монстров (если броня >= 13, снижение будет <= 0)
                if (damageReduction <= 0.05)
                {
                    damageReduction = 0.05; // Оставляем монстру получение хотя бы 5% урона
                }

                // 2. Считаем эффективное здоровье
                double effectiveHp = monster_hp / damageReduction;

                // 3. Итоговая сила: Эффективное здоровье * Урон
                return (effectiveHp * monster_damage)/4;
            }
        }
        public int MonsterCoast
        {
            get
            {
                return Convert.ToInt32(PowerScore);
            }
        }

        // реализация метода для сортировки
        public int CompareTo(Monster other)
        {
            if (other == null) return 1;

            // Сравниваем здоровье текущего юнита со здоровьем другого
            return this.PowerScore.CompareTo(other.PowerScore);
        }


        public Monster(int monster_hp, int monster_damage, int monster_armor, string monster_name, int monster_cordx, int monster_cordy)
        {
            this.monster_hp = monster_hp;
            this.monster_damage = monster_damage;
            this.monster_armor = monster_armor;// не может быть равно 0
            this.monster_name = monster_name;
            this.monster_cordx = monster_cordx;
            this.monster_cordy = monster_cordy;
        }

        //метод с окном битвы
        public static void MonsterFight(Monster monster)
        {
            int rewardGold = (Convert.ToInt32(monster.PowerScore / 4));
            Console.Clear();
            Console.WriteLine($"Вы встретили {monster.monster_name}");
            Console.WriteLine($"Здоровье монстра - {monster.monster_hp}      Ваше Здоровье - {GameState.Instance.Player_1.player_hp}");
            Console.WriteLine($"Урон монстра - {monster.monster_damage}      Ваш урон - {GameState.Instance.Player_1.PlayerRPGClass_1.class_state.damage}");
            Console.WriteLine($"Броня монстра - {monster.monster_armor}      Ваша броня - {GameState.Instance.Player_1.PlayerRPGClass_1.class_state.armor}");
            Console.WriteLine($"Вы хотите сразиться с ним?");
            Console.WriteLine($"Сила монстра - {monster.PowerScore}");
            Console.WriteLine("1 - ДА");
            Console.WriteLine("Любая клавиша - НЕТ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // БОЙ: пока оба живы (HP > 0)
                    while (monster.monster_hp > 0 && GameState.Instance.Player_1.player_hp > 0)
                    {
                        // расчет урона игрока
                        int damageToMonster = (int)(GameState.Instance.Player_1.PlayerRPGClass_1.class_state.damage * (1 - monster.monster_armor * 0.08));
                        monster.monster_hp -= damageToMonster;

                        // расчет урона монстра
                        if (monster.monster_hp > 0)
                        {
                            int damageToPlayer = (int)(monster.monster_damage * (1 - GameState.Instance.Player_1.PlayerRPGClass_1.class_state.armor * 0.08));
                            GameState.Instance.Player_1.player_hp -= damageToPlayer;
                        }

                        // Показываем текущее HP
                        Console.WriteLine($"Здоровье монстра - {monster.monster_hp}      Ваше Здоровье - {GameState.Instance.Player_1.player_hp}");

                        // Проверка: кто-то умер? тогда выходим
                        if (monster.monster_hp <= 0 || GameState.Instance.Player_1.player_hp <= 0)
                        {
                            break;
                        }

                        // Предлагаем продолжить или сбежать
                        Console.WriteLine("Вы хотите продолжить бой?");
                        Console.WriteLine("1 - ДА");
                        Console.WriteLine("Любая клавиша - Сбежать");
                        var fightchoice = Console.ReadKey();
                        Console.WriteLine(); // перевод строки

                        if (fightchoice.Key == ConsoleKey.D1)
                        {
                            // Продолжаем бой
                        }
                        else
                        {
                            Console.WriteLine("Вы сбежали из боя!");
                            Console.ReadKey();
                            return; // выходим из метода
                        }
                    }

                    // === ПРОВЕРКА ПОБЕДИТЕЛЯ ===
                    Console.WriteLine("\n====================");
                    if (GameState.Instance.Player_1.player_hp > 0)
                    {
                        Console.WriteLine($"ПОБЕДА! Вы победили {monster.monster_name}!");
                        Console.WriteLine($"У вас осталось {GameState.Instance.Player_1.player_hp} HP");
                        Console.WriteLine($"Вы заработали {rewardGold}");
                        GameState.Instance.Player_1.gold += rewardGold;
                        GameState.Instance.MonstersList.Remove(monster);
                        GameState.Instance.map[monster.monster_cordy, monster.monster_cordx] = ' ';
                    }
                    else
                    {
                        GameState.GameOver();
                    }
                    Console.WriteLine("====================");
                    Console.WriteLine("Нажмите любую клавишу...");
                    Console.ReadKey();

                    break;

                default:
                    Console.WriteLine("Вы решили не сражаться.");
                    Console.ReadKey();
                    break;
            }
        }

        // метод проверки монстра на клетке шага и вызова битвы
        public static void OnPlayerStep(int y, int x, ref bool cancel)
        {
            Monster found = null;
            // Проходим по всем монстрам
            foreach (Monster monster in GameState.Instance.MonstersList)
            {
                // Если координаты совпадают - возвращаем монстра
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
