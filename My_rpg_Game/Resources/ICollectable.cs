using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

namespace My_Game

{
    public interface ICollectable
    {
        char Symbol { get; }
        string Name { get; }
    }
}