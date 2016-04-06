using System.Collections.Generic;
using System.Linq;
using BattleShipGame.Ships;

namespace BattleShipGame
{
    public class Map
    {
        public Dictionary<Coordinate, Tile> Tiles;
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
            Tiles = new Dictionary<Coordinate, Tile>();
            for (uint x = 0; x < BoardWidth; x++)
            {
                for (uint y = 0; y < BoardHeight; y++)
                {
                    Tiles.Add(new Coordinate(x, y), new Tile());
                }
            }
        }

        public bool AreaClear(Dictionary<Coordinate, Tile>.KeyCollection area)
        {
            return area.All(coord => Tiles.ContainsKey(coord) && Tiles[coord].Status == TileStatus.OpenOcean);
        }

        public bool SetShip(Ship newShip)
        {
            //Ships do not stack in this version
            if (!AreaClear(newShip.Hull.Keys))
                return false;
            //Place Ship
            foreach (var hullPiece in newShip.Hull)
                Tiles[hullPiece.Key] = hullPiece.Value;

            return true;
        }
    }
}
