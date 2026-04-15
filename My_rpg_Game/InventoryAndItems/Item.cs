using System.Diagnostics;

namespace My_Game

{
    // ========== ÊËÀÑÑ ÏÐÅÄÌÅÒÎÂ ==========
    public class Item
    {
        public string Name { get; set; }
        public char Symbol { get; set; }
        public string Description { get; set; }
        public int HudIndex { get; set; }
        public int Price { get; set; }

        public Item(string name, char symbol, string description, int hudindex, int price)
        {
            Name = name;
            Symbol = symbol;
            Description = description;
            HudIndex = hudindex;
            Price = price;
        }

    }
}