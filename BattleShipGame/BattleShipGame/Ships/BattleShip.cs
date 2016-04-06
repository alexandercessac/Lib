﻿namespace BattleShipGame.Ships
{
    public class BattleShip : Ship
    {
        public const string Name = "BattleShip";
        public BattleShip(Coordinate[] location) : base(location, 5)
        { }
    }
}
