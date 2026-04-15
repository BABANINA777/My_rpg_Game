using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static My_Game.MonsterFactory;

namespace My_Game
{
    internal class MonsterFactory
    {
        public enum Monster_type
        {
            //слабые
            Goblin,
            Skeleton,
            Slime,
            GiantRat,
            Wolf,
            //средние
            OrcWarrior,
            Zombie,
            ArmoredSkeleton,
            //сильные
            CaveTroll,
            LesserDemon,
        }
        public static Monster CreateMonster(Monster_type monster_type, int x, int y) 
        { 
            switch(monster_type)
            {
                case Monster_type.Goblin:
                    return new Monster(30, 5, 1, "Goblin", x, y);
                case Monster_type.Skeleton:
                    return new Monster(20, 10, 3, "Skeleton", x, y);
                case Monster_type.Slime:
                    return new Monster(15, 3, 0, "Slime", x, y);
                case Monster_type.GiantRat:
                    return new Monster(20, 4, 0, "Giant Rat", x, y);
                case Monster_type.Wolf:
                    return new Monster(25, 5, 1, "Wolf", x, y);
                
                case Monster_type.OrcWarrior:
                    return new Monster(50, 8, 2, "Orc Warrior", x, y);
                case Monster_type.Zombie:
                    return new Monster(45, 10, 1, "Zombie", x, y);
                case Monster_type.ArmoredSkeleton:
                    return new Monster(60, 12, 3, "Armored Skeleton", x, y);

                case Monster_type.CaveTroll:
                    return new Monster(120, 18, 5, "Cave Troll", x, y);
                case Monster_type.LesserDemon:
                    return new Monster(150, 25, 7, "Lesser Demon", x, y);



                default:
                    return new Monster(0, 0, 0, "Пустота", x, y);
                    
            }
        
        }
    }
}
