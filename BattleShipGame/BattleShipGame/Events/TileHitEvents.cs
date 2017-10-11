using BattleShipGame.Identity;

namespace BattleShipGame.Events
{
    public class TileHitEvents
    {
        public delegate void ShipHitEvent(string playerName);
        public delegate void HitEvent(string playerName, Coordinate location);
        public delegate void SinkEvent(string playerName);
    }
}
