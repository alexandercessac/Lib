using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BattleShipGame.Interfaces;
using static BattleShipGame.Events.TileHitEvents;

namespace BattleShipGame.Ships
{
    public class Ship
    {
        public uint Size { get; }
        public Dictionary<Point, Tile> Hull { get; set; }
        public Point[] Location { get; }
        public SinkEvent OnSinking;

        public Ship(Point[] location, uint size)
        {
            Size = size;
            if (location.Length != Size)
            {
                throw new Exception($"Ship needs {Size} coordinates. Recieved {location.Length}");
            }

            Hull = new Dictionary<Point, Tile>();

            foreach (var coord in location)
            {
                Hull.Add(coord, new Tile {OnHit = OnHit, Status = TileStatus.Ship});
            }

            Location = location;
        }

        public void OnHit(Point location)
        {
            Hull[location].Status = TileStatus.Hit;

            if (Hull.Any(x => x.Value.Status != TileStatus.Hit)) return;
            //Sink the ship if all tiles in the Hull have been hit
            foreach (var tile in Hull.Values)
            {
                tile.Status = TileStatus.Sunk;
            }
            OnSinking?.Invoke();
        }

    }
}
