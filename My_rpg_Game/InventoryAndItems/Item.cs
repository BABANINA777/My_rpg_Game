using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

namespace My_Game

{
    // ========== йкюяя опедлернб ==========
    public class Item
    {
        public string Name { get; set; }
        public char Symbol { get; set; }
        public string Description { get; set; }
        public int HudIndex { get; set; }

        public Item(string name, char symbol, string description, int hudindex)
        {
            Name = name;
            Symbol = symbol;
            Description = description;
            HudIndex = hudindex;
        }

    }
}