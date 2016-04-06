namespace BattleShipGame.Ships
{
    public class Fleet
    {
        public Ship BattleShip { get; }
        public Ship Submarine { get; }
        public Ship Destroyer { get; }
        public Ship TugBoat { get; }
        public Ship Carrier { get; }

        public Fleet(Ship battleShip, Ship submarine, Ship destroyer, Ship tugBoat, Ship carrier)
        {
            BattleShip = battleShip;
            Submarine = submarine;
            Destroyer = destroyer;
            TugBoat = tugBoat;
            Carrier = carrier;
        }
    }
}
