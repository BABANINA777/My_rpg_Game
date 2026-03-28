using System;

namespace My_Game

{
    // ========== КЛАСС ИНВЕНТАРЯ ==========
    // Управляет сохранением и загрузкой юнитов игрока
    public class Inventory
    {
        public static List<Item> ItemEquipList = new();
        public static List<Item> ItemNotEquipList = new()
    {
        new Item("EpicHelmet", 'H', "Эпический шлем (+10 защита)", 0),
        new Item("RareHelmet", 'H', "Редкий шлем (+5 защита)", 0),
        new Item("LegendSword", 'S', "Легендарный меч (+10 урон)", 3),
        new Item("Shield", 'D', "Щит (+3 защита)", 1)
    };
        public static char[] ItemEquipHud = new char[6] { ' ', ' ', ' ', ' ', ' ', ' ' };

        public static void InventoryUI()
        {
            // Отрисовка HUD экипировки
            Console.Clear();
            Console.WriteLine($"    [{ItemEquipHud[0]}]     - Слот для шлема");
            Console.WriteLine($"[{ItemEquipHud[1]}] [{ItemEquipHud[2]}] [{ItemEquipHud[3]}] - Слот для меча, щита и нагрудника");
            Console.WriteLine($"    [{ItemEquipHud[4]}]     - Слот для понож");
            Console.WriteLine($"    [{ItemEquipHud[5]}]     - Слот для ботинок");
            Console.WriteLine("Доступные предметы в для экиперовки:");
            int counter = 1; //счетчик для отрисовки доступных предметов
            foreach (Item item in ItemNotEquipList)
            {
                Console.WriteLine($"{counter}. [{item.Symbol}] - {item.Name}, {item.Description} ");
                counter++;
            }
            Console.Write("Любая клавиша выход");
            Console.Write("Выберите предмет который хотите экипировать:");
            try // логика выбора предметов
            {
                int PlayerChoise = int.Parse(Console.ReadLine());
                var SelectedItem = ItemNotEquipList[PlayerChoise - 1];//выбранный предмет из списка неэкеперованных
                if (ItemEquipHud[SelectedItem.HudIndex] == ' ')// проверка надет-ли предмет с такимже HudIndex
                {
                    ItemEquipList.Add(ItemNotEquipList[PlayerChoise - 1]);// добавляем его в список надетых

                    ItemNotEquipList.RemoveAt(PlayerChoise - 1); // удаляем его из списка НЕнадетых

                    ItemEquipHud[SelectedItem.HudIndex] = SelectedItem.Symbol;//добавляем его значек в массив для отображения
                }
                else
                {
                    // Найти предмет с таким же HudIndex в списке экипированных
                    Item oldItem = ItemEquipList.Find(item => item.HudIndex == SelectedItem.HudIndex);

                    // Снимаем старый предмет удаляя его из списка экиперованных
                    ItemEquipList.Remove(oldItem);

                    ItemNotEquipList.Add(oldItem);// добовляем его в список НЕэкиперованных

                    // Надеваем новый предмет
                    ItemEquipList.Add(SelectedItem);
                    ItemNotEquipList.RemoveAt(PlayerChoise - 1);
                    ItemEquipHud[SelectedItem.HudIndex] = SelectedItem.Symbol;
                }
                InventoryUI();
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void SaveInventory()
        {
            using (StreamWriter writer = new StreamWriter("SaveGameInventory.txt", false)) // Открываем файл для записи
            {
                writer.WriteLine(ItemEquipList.Count);// Записываем количество слотов из списка экиперованных
                writer.WriteLine(ItemNotEquipList.Count);// Записываем количество слотов из списка неэкиперованных предметов
                foreach (Item item in ItemEquipList)
                {
                    writer.WriteLine(item.Name);
                    writer.WriteLine(item.Symbol);
                    writer.WriteLine(item.Description);
                    writer.WriteLine(item.HudIndex);
                }
                foreach (Item item in ItemNotEquipList)
                {
                    writer.WriteLine(item.Name);
                    writer.WriteLine(item.Symbol);
                    writer.WriteLine(item.Description);
                    writer.WriteLine(item.HudIndex);
                }
                foreach (char hudsymbol in ItemEquipHud)
                {
                    writer.WriteLine(hudsymbol);
                }
            }

        }
        public static void LoadInventory()
        {
            ItemEquipList.Clear();  // Очищаем текущий список перед загрузкой
            ItemNotEquipList.Clear();  // Очищаем текущий список перед загрузкой
            using (StreamReader reader = new StreamReader("SaveGameInventory.txt")) // Открываем файл для записи
            {
                int ItemEquipListCount = int.Parse(reader.ReadLine());
                int ItemNotEquipListCount = int.Parse(reader.ReadLine());
                for (int i = 0; i < ItemEquipListCount; i++) // Читаем каждый слот
                {
                    string name = reader.ReadLine();// Читаем имя предмета
                    char symbol = (char)reader.ReadLine()[0];// Читаем символ предмета
                    string description = reader.ReadLine();
                    string hudindex = reader.ReadLine();
                    // Создаём слот и добавляем в список
                    Item item = new Item(name, symbol, description, int.Parse(hudindex));
                    ItemEquipList.Add(item);
                }
                for (int i = 0; i < ItemNotEquipListCount; i++) // Читаем каждый слот
                {
                    string name = reader.ReadLine();// Читаем имя предмета
                    char symbol = (char)reader.ReadLine()[0];// Читаем символ предмета
                    string description = reader.ReadLine();
                    string hudindex = reader.ReadLine();
                    // Создаём слот и добавляем в список
                    Item item = new Item(name, symbol, description, int.Parse(hudindex));
                    ItemNotEquipList.Add(item);
                }
                for (int i = 0; i < 6; i++)
                {
                    ItemEquipHud[i] = reader.ReadLine()[0];
                }
            }
        }
    }
}
