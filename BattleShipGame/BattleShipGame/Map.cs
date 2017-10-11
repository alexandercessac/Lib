using System.Linq;
using BattleShipGame.Ships;
using System.Collections.Generic;
using BattleShipGame.Identity;

namespace BattleShipGame
{
    public class Map
    {
        public string CaptainName;
        public int TotalShips = 0;
        public int ActiveShips = 0;
        internal bool HasActiveShips => ActiveShips > 0;
        //public Fleet Fleet;//TODO: make into its own object
        public List<Ship> Fleet = new List<Ship>();

        public uint BoardWidth { get; set; }
        public uint BoardHeight { get; set; }
        public TileDictionary TileDictionary;

        public Map() { }
        public Map(Player captain) : this(captain, 10, 10) { }
        public Map(Player captain, uint width, uint height)
        {
            BoardHeight = height;
            CaptainName = captain.Name;
            BoardWidth = width;
            ResetMap();
        }

        public void ResetMap()
        {
            TileDictionary = new TileDictionary();
            for (var x = 0; x < BoardWidth; x++)
                for (var y = 0; y < BoardHeight; y++)
                    TileDictionary.Add(new Coordinate(x, y), new Tile());
        }


        public bool Fire(Player player, Coordinate coord) => Fire(new Shot {Coordinate = coord, PlayerName = player.Name});
        public bool Fire(Shot shot)
        {
            //Attempt to raise event on specified tile
            if (!TileDictionary.ContainsKey(shot.Coordinate)) return false;

            var tile = TileDictionary[shot.Coordinate];

            tile?.OnHit?.Invoke(shot.PlayerName, shot.Coordinate);
            
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
            TileDictionary[coord].Status == TileStatus.OpenOcean;

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
                TileDictionary[hullPiece.Key] = hullPiece.Value;
            ActiveShips++;
            TotalShips++;
            Fleet.Add(newShip);
        }
        
    }
}
