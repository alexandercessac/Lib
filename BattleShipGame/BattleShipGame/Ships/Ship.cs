using System.Linq;
using static BattleShipGame.Events.TileHitEvents;

namespace BattleShipGame.Ships
{
    public class Ship
    {
        public uint Size => 
            uint.Parse((Hull?.Keys.Count ?? 0).ToString());

        
        public TileDictionary Hull { get; set; }
        public Coordinate[] Location { get; }
        public bool Sunken;
        internal SinkEvent OnSinking;
        //public ShipHitEvent OnShipHit;
        internal HitEvent OnHit;
        public readonly string Name;

        public Ship(Coordinate[] location, string name)
        {
            Name = name;
            Location = location;
            Hull = new TileDictionary();
            
            foreach (var coord in location)
                Hull.Add(coord, new Tile {OnHit = WhenHit, Status = TileStatus.Ship});
        }

        internal void WhenHit(string playerName, Coordinate location)
        {
            Hull[location].Status = TileStatus.Hit;
            OnHit?.Invoke(playerName, location);

            if (Hull.Any(x => x.Value.Status != TileStatus.Hit)) return;

            //Ship sinks if all tiles in the Hull have been hit
            foreach (var tile in Hull.Values)
                tile.Status = TileStatus.Sunk;

            Sunken = true;
            OnSinking?.Invoke(playerName);
            
        }
    }
}
