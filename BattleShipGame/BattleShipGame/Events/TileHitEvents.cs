using System.Drawing;

namespace BattleShipGame.Events
{
    public class TileHitEvents
    {
        public delegate void ShipHitEvent();
        public delegate void HitEvent(Coordinate location);
        public delegate void SinkEvent();
    }
}
