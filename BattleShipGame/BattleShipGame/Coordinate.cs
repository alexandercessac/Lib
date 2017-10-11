using System;
using Newtonsoft.Json;

namespace BattleShipGame
{
    //[JsonConverter(typeof(CoordinateConverter))]
    public struct Coordinate
    {
        public int X;
        public int Y;

        [JsonConstructor]
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"{X},{Y}";
    }

  
}
