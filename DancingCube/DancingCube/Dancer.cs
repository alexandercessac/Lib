using System;
using DancingCube.Interfaces;

namespace DancingCube
{
    public class Dancer : ITangible, IIdentifiable
    {
        public string Name;
        public string Identity { get; set; }
        public ConsoleColor Color;
        public Point Position { get; set; }
    }
}