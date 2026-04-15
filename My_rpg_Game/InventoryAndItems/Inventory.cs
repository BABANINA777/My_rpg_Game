using System.Diagnostics;

namespace My_Game

{
    // ========== КЛАСС ИНВЕНТАРЯ ==========
    // Управляет сохранением и загрузкой юнитов игрока
    public class Inventory
    {
        
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
            foreach (Item item in GameState.Instance.ItemNotEquipList)
            {
                Console.WriteLine($"{counter}. [{item.Symbol}] - {item.Name}, {item.Description} ");
                counter++;
            }
            Console.Write("Любая клавиша выход");
            Console.Write("Выберите предмет который хотите экипировать:");
            try // логика выбора предметов
            {
                int PlayerChoise = int.Parse(Console.ReadLine());
                var SelectedItem = GameState.Instance.ItemNotEquipList[PlayerChoise - 1];//выбранный предмет из списка неэкеперованных
                if (ItemEquipHud[SelectedItem.HudIndex] == ' ')// проверка надет-ли предмет с такимже HudIndex
                {
                    GameState.Instance.ItemEquipList.Add(GameState.Instance.ItemNotEquipList[PlayerChoise - 1]);// добавляем его в список надетых

                    //Execution.Player_1.PlayerRPGClass_1.class_state.damage += SelectedItem.;

                    GameState.Instance.ItemNotEquipList.RemoveAt(PlayerChoise - 1); // удаляем его из списка НЕнадетых

                    ItemEquipHud[SelectedItem.HudIndex] = SelectedItem.Symbol;//добавляем его значек в массив для отображения
                }
                else
                {
                    // Найти предмет с таким же HudIndex в списке экипированных
                    Item oldItem = GameState.Instance.ItemEquipList.Find(item => item.HudIndex == SelectedItem.HudIndex);

                    // Снимаем старый предмет удаляя его из списка экиперованных
                    GameState.Instance.ItemEquipList.Remove(oldItem);

                    GameState.Instance.ItemNotEquipList.Add(oldItem);// добовляем его в список НЕэкиперованных

                    // Надеваем новый предмет
                    GameState.Instance.ItemEquipList.Add(SelectedItem);
                    GameState.Instance.ItemNotEquipList.RemoveAt(PlayerChoise - 1);
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
                writer.WriteLine(GameState.Instance.ItemEquipList.Count);// Записываем количество слотов из списка экиперованных
                writer.WriteLine(GameState.Instance.ItemNotEquipList.Count);// Записываем количество слотов из списка неэкиперованных предметов
                foreach (Item item in GameState.Instance.ItemEquipList)
                {
                    writer.WriteLine(item.Name);
                    writer.WriteLine(item.Symbol);
                    writer.WriteLine(item.Description);
                    writer.WriteLine(item.HudIndex);
                    writer.WriteLine(item.Price);
                }
                foreach (Item item in GameState.Instance.ItemNotEquipList)
                {
                    writer.WriteLine(item.Name);
                    writer.WriteLine(item.Symbol);
                    writer.WriteLine(item.Description);
                    writer.WriteLine(item.HudIndex);
                    writer.WriteLine(item.Price);
                }
                foreach (char hudsymbol in ItemEquipHud)
                {
                    writer.WriteLine(hudsymbol);
                }
            }

        }
        public static void LoadInventory()
        {
            GameState.Instance.ItemEquipList.Clear();  // Очищаем текущий список перед загрузкой
            GameState.Instance.ItemNotEquipList.Clear();  // Очищаем текущий список перед загрузкой
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
                    string price = reader.ReadLine();
                    // Создаём слот и добавляем в список
                    Item item = new Item(name, symbol, description, int.Parse(hudindex), int.Parse(price));
                    GameState.Instance.ItemEquipList.Add(item);
                }
                for (int i = 0; i < ItemNotEquipListCount; i++) // Читаем каждый слот
                {
                    string name = reader.ReadLine();// Читаем имя предмета
                    char symbol = (char)reader.ReadLine()[0];// Читаем символ предмета
                    string description = reader.ReadLine();
                    string hudindex = reader.ReadLine();
                    string price = reader.ReadLine();
                    // Создаём слот и добавляем в список
                    Item item = new Item(name, symbol, description, int.Parse(hudindex), int.Parse(price));
                    GameState.Instance.ItemNotEquipList.Add(item);
                }
                for (int i = 0; i < 6; i++)
                {
                    ItemEquipHud[i] = reader.ReadLine()[0];
                }
            }
        }
    }
}
