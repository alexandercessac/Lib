using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BattleShipGame.Ships;

namespace BattleShipGame
{
    public class Map
    {
        public Dictionary<Coordinate, Tile> Tiles;
        public Fleet Fleet;//TODO: make into its own object

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
            Tiles = new Dictionary<Coordinate, Tile>();
            for (var x = 0; x < BoardWidth; x++)
            {
                for (var y = 0; y < BoardHeight; y++)
                {
                    Tiles.Add(new Coordinate(x, y), new Tile());
                }
            }
        }

        public bool Fire(Coordinate coord)
        {
        
            var target = Tiles[coord];

            target?.OnHit?.Invoke(coord);

            if (target == null) return false;
            switch (target.Status)
            {
                case TileStatus.Hit:
                case TileStatus.Sunk:
                    return true;
            }
            return false;
        }

        public bool AreaClear(Dictionary<Coordinate, Tile>.KeyCollection area)
        {
            return area.All((Coordinate) => Tiles[Coordinate].Status == TileStatus.OpenOcean);

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

        public bool IsOpenOcean(Coordinate tile)
        {
            return  Tiles[tile].Status == TileStatus.OpenOcean;
        }
    }
}
