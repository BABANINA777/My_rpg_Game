namespace My_Game
{
    class Shop : Building
    {
        // Статический список товаров (один ассортимент на все магазины)
        public static List<Item> ShopItems = new List<Item>()
        {
            // === ШЛЕМЫ (HudIndex: 0) ===
            new Item("IronHelmet", 'H', "Железный шлем (+3 защита)", 0, 5),
            new Item("SteelHelmet", 'H', "Стальной шлем (+6 защита)", 0, 12),
            new Item("MithrilHelmet", 'H', "Мифриловый шлем (+12 защита)", 0, 25),
            new Item("DragonCrown", 'H', "Корона Дракона (+20 защита)", 0, 50),

            // === ЩИТЫ (HudIndex: 1) ===
            new Item("WoodenShield", 'D', "Деревянный щит (+2 защита)", 1, 4),
            new Item("IronShield", 'D', "Железный щит (+5 защита)", 1, 10),
            new Item("TowerShield", 'D', "Башенный щит (+10 защита)", 1, 22),
            new Item("Aegis", 'D', "Эгида (+18 защита)", 1, 45),

            // === ОРУЖИЕ (HudIndex: 2) ===
            new Item("IronSword", 'W', "Железный меч (+4 урон)", 2, 8),
            new Item("SteelAxe", 'W', "Стальной топор (+7 урон)", 2, 15),
            new Item("SilverBow", 'W', "Серебряный лук (+11 урон)", 2, 24),
            new Item("MagicStaff", 'W', "Магический посох (+15 урон)", 2, 35),
            new Item("Excalibur", 'W', "Экскалибур (+25 урон)", 2, 70),

            // === НАГРУДНИКИ (HudIndex: 3) ===
            new Item("LeatherArmor", 'C', "Кожаная броня (+3 защита)", 3, 6),
            new Item("Chainmail", 'C', "Кольчуга (+7 защита)", 3, 16),
            new Item("PlateArmor", 'C', "Латная броня (+14 защита)", 3, 30),
            new Item("ShadowCloak", 'C', "Плащ теней (+10 защита, уклонение)", 3, 40),

            // === ПОНОЖИ (HudIndex: 4) ===
            new Item("LeatherPants", 'P', "Кожаные штаны (+2 защита)", 4, 5),
            new Item("SteelGreaves", 'P', "Стальные поножи (+6 защита)", 4, 18),

            // === БОТИНКИ (HudIndex: 5) ===
            new Item("LeatherBoots", 'B', "Кожаные сапоги (+1 защита)", 5, 4),
            new Item("WingedBoots", 'B', "Крылатые сапоги (+2 защита, скорость)", 5, 20)
        };

        // Конструктор магазина
        public Shop(int y, int x)
        {
            PosY = y;
            PosX = x;
            Symbol = '$'; // Символ магазина на карте
        }

        // Интерфейс магазина
        public override void BuildingUI()
        {
            while (true) // Цикл, чтобы можно было купить несколько вещей подряд
            {
                Console.Clear();
                Console.WriteLine("=== ТОРГОВАЯ ЛАВКА ===");
                Console.WriteLine($"Ваше золото: {GameState.Instance.Player_1.gold}");
                Console.WriteLine("======================\n");


                // Выводим список предметов
                for (int i = 0; i < ShopItems.Count; i++)
                {
                    var item = ShopItems[i];
                    Console.WriteLine($"{i + 1}. [{item.Symbol}] {item.Name}");
                    Console.WriteLine($"   {item.Description} | Цена: {item.Price} gold");
                }


                Console.WriteLine("\n0. Выйти из магазина");
                Console.Write("\nВведите номер товара для покупки: ");

                string input = Console.ReadLine();

                if (input == "0") break; // Выход из магазина

                // Проверяем, ввел ли игрок число и есть ли такой товар
                if (int.TryParse(input, out int choice) && choice > 0 && choice <= ShopItems.Count)
                {
                    Item selectedItem = ShopItems[choice - 1];

                    // Проверка золота
                    if (GameState.Instance.Player_1.gold >= selectedItem.Price)
                    {
                        GameState.Instance.Player_1.gold -= selectedItem.Price; // Списываем золото
                        GameState.Instance.ItemNotEquipList.Add(selectedItem);  // Добавляем в инвентарь игрока
                        ShopItems.RemoveAt(choice - 1);                // Убираем товар с витрины

                        Console.WriteLine($"\nУСПЕХ! Вы купили {selectedItem.Name}.");
                    }
                    else
                    {
                        Console.WriteLine("\nОШИБКА: У вас недостаточно золота!");
                    }
                }
                else
                {
                    Console.WriteLine("\nОШИБКА: Неверный номер товара.");
                }

                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }
        }

    }
}
