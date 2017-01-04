using BattleShipGame.Identity;

namespace BattleShipGame.Events
{
    public class TileHitEvents
    {
        public delegate void ShipHitEvent(Player player);
        public delegate void HitEvent(Player player, Coordinate location);
        public delegate void SinkEvent(Player player);
    }
}
