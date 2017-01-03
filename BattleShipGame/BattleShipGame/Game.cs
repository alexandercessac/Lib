using BattleShipGame.Identity;

namespace BattleShipGame
{
    public class GameConfig
    {
        public Player[] Players;
        public uint MapHeight;
        public uint MapWidth;
    }

    public class Game
    {
        public Map[] Maps;

        public Game(GameConfig config)
        {
            Maps = new Map[config.Players.Length];

            for (var i = 0; i < config.Players.Length; i++)
                Maps[i] = new Map(config.Players[i], config.MapWidth, config.MapHeight);
        }

    }
}
