using static BattleShipGame.Events.TileHitEvents;

namespace BattleShipGame
{
    public class Tile
    {
        public TileStatus Status;
        internal HitEvent OnHit;

        public Tile()
        {
            OnHit = null;
            Status = TileStatus.OpenOcean;
        }
    }



}
