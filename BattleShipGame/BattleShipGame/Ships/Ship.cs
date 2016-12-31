using System.Linq;
using System.Collections.Generic;
using static BattleShipGame.Events.TileHitEvents;

namespace BattleShipGame.Ships
{
    public class Ship
    {
        public uint Size => 
            uint.Parse((Hull?.Keys.Count ?? 0).ToString());

        public Dictionary<Coordinate, Tile> Hull { get; set; }
        public Coordinate[] Location { get; }
        public bool Sunken;
        public SinkEvent OnSinking;
        public HitEvent OnHit;
        public readonly string Name;

        public Ship(Coordinate[] location, string name)
        {
            Name = name;
            Location = location;
            Hull = new Dictionary<Coordinate, Tile>();
            
            foreach (var coord in location)
                Hull.Add(coord, new Tile {OnHit = WhenHit, Status = TileStatus.Ship});
        }

        private void WhenHit(Coordinate location)
        {
            Hull[location].Status = TileStatus.Hit;
            OnHit?.Invoke(location); //?? why location?
            if (Hull.Any(x => x.Value.Status != TileStatus.Hit)) return;

            OnSinking?.Invoke();
            //Sink the ship if all tiles in the Hull have been hit
            foreach (var tile in Hull.Values)
                tile.Status = TileStatus.Sunk;

            Sunken = true;
        }
    }
}
