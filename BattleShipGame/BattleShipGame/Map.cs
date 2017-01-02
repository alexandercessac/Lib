using System.Linq;
using BattleShipGame.Ships;
using System.Collections.Generic;

namespace BattleShipGame
{
    public class Map
    {
        public int TotalShips = 0;
        public int ActiveShips = 0;
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
                for (var y = 0; y < BoardHeight; y++)
                    Tiles.Add(new Coordinate(x, y), new Tile());
        }

        public bool Fire(Coordinate coord)
        {
            //Attempt to raise event on specified tile
            var tile = Tiles[coord];

            tile?.OnHit?.Invoke(coord);
            
            //Check the resulting status of the specified tile
            switch (tile?.Status ?? default(TileStatus))
            {
                case TileStatus.Hit:
                case TileStatus.Sunk:
                    return true;
                case TileStatus.OpenOcean:
                case TileStatus.Ship:
                case TileStatus.Miss:
                default:
                    if (tile != null)
                        tile.Status = TileStatus.Miss;
                    return false;
            }
            
        }

        public bool AreaClear(Dictionary<Coordinate, Tile> area) => 
            area.Keys.All(OpenOcean);

        public bool OpenOcean(Coordinate coord) => 
            Tiles[coord].Status == TileStatus.OpenOcean;

        public bool SetShip(params Ship[] newShips)
        {
            //Ships do not stack in this version
            if (!newShips.All(s => AreaClear(s.Hull))) return false;

            for (var i = 0; i < newShips.Length - 1; i++)
                Set(newShips[i]);

            
            return true;
        }

        public bool SetShip(Ship newShip)
        {
            //Ships do not stack in this version
            if (!AreaClear(newShip.Hull)) return false;

            Set(newShip);

            return true;
        }

        //Place Ship on map
        private void Set(Ship newShip)
        {
            foreach (var hullPiece in newShip.Hull)
                Tiles[hullPiece.Key] = hullPiece.Value;
            ActiveShips++;
            TotalShips++;
        }
        
    }
}
