using System.Drawing;

namespace BattleShipGame.Ships
{
    public class Submarine : Ship
    {
        public const string Name = "Submarine";
        public Submarine(Point[] location) : base(location, 5, Name)
        { }
    }
}
