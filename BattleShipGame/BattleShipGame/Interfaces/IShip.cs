using System.Security.Cryptography.X509Certificates;

namespace BattleShipGame.Interfaces
{
    public interface IShip
    {
        uint Size { get;}
        Tile[] Hull { get; set; }

    }
}
