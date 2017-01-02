namespace BattleShipGame
{
    public class GameConfig
    {
        public int Players;
        public uint MapHeight;
        public uint MapWidth;
    }

    public class Game
    {
        public Map[] Maps;

        public Game(GameConfig config)
        {
            Maps = new Map[config.Players];

            for (var i = 0; i < config.Players; i++)
                Maps[i] = new Map(config.MapWidth, config.MapHeight);
        }

    }
}
