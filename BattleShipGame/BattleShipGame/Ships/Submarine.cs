namespace BattleShipGame.Ships
{
    public class Submarine : Ship
    {
        public const string Name = "Submarine";
        public Submarine(Coordinate[] location) : base(location, 5)
        { }
    }
}
