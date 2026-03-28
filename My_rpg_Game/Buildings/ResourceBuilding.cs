using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

namespace My_Game

{
    // ========== КЛАСС РЕСУРСНОГО ЗДАНИЯ ==========
    // Здание, приносящее ресурсы каждую неделю
    public class ResourceBuilding : Building
    {
        static public int NewResurseTimer = 0;
        static public int ResourceBuildingCounter = 0;

        // Конструктор ресурсного здания
        public ResourceBuilding(int y, int x)
        {
            PosY = y;      // Устанавливаем координату Y
            PosX = x;      // Устанавливаем координату X
            Symbol = 'R';  // Символ ресурсного здания на карте
        }

        // UI ресурсного здания
        public override void BuildingUI()
        {
            Console.WriteLine();
            Console.WriteLine("=== РЕСУРСНОЕ ЗДАНИЕ ===");
            Console.WriteLine("Это ресурсная постройка");
            Console.WriteLine("Она приносит +1 gold, +1 wood, +1 stone каждую неделю");
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}