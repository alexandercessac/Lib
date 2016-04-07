using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BattleShipGame.Ships;

namespace BattleShipGame
{
    public class Map
    {
        public Dictionary<Point, Tile> Tiles;
        public List<Ship> Fleet;//TODO: make into its own object

        //PRIVATE MEMBERS
        public uint BoardWidth { get; }
        public uint BoardHeight { get; }
        
        public Map() : this(10, 10) { }
        public Map(uint width, uint height)
        {
            BoardHeight = height;
            BoardWidth = width;
            ResetMap();
        }

        public void ResetMap()
        {
            Tiles = new Dictionary<Point, Tile>();
            for (var x = 0; x < BoardWidth; x++)
            {
                for (var y = 0; y < BoardHeight; y++)
                {
                    Tiles.Add(new Point(x, y), new Tile());
                }
            }
        }

        public bool Fire(Point coord)
        {
        
            var target = Tiles[coord];

            target?.OnHit?.Invoke(coord);
            return (target?.Status) == TileStatus.Hit;
        }

        public bool AreaClear(Dictionary<Point, Tile>.KeyCollection area)
        {
            return area.All(coord => Tiles.Single(c => c.Key.X == coord.X && c.Key.Y == coord.Y).Value.Status == TileStatus.OpenOcean);
        }

        public bool SetShip(Ship newShip)
        {
            //Ships do not stack in this version
            if (!AreaClear(newShip.Hull.Keys))
                return false;
            //Place Ship
            foreach (var hullPiece in newShip.Hull)
            {

                Tiles[hullPiece.Key] = hullPiece.Value;
                
            }

            return true;
        }
    }
}
