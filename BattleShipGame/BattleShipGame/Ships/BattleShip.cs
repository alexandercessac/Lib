using System.Drawing;

namespace BattleShipGame.Ships
{
    public class BattleShip : Ship
    {
        public const string Name = "BattleShip";
        public BattleShip(Point[] location) : base(location, 5, Name)
        { }
    }
}
