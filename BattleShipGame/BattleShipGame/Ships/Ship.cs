using System.Linq;
using System.Collections.Generic;
using BattleShipGame.Identity;
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
        public ShipHitEvent OnShipHit;
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

        private void WhenHit(Player player, Coordinate location)
        {
            Hull[location].Status = TileStatus.Hit;
            OnShipHit?.Invoke(player);

            if (Hull.Any(x => x.Value.Status != TileStatus.Hit)) return;

            //Ship sinks if all tiles in the Hull have been hit
            foreach (var tile in Hull.Values)
                tile.Status = TileStatus.Sunk;

            Sunken = true;
            OnSinking?.Invoke(player);
            
        }
    }
}
