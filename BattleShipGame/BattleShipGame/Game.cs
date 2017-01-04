﻿using BattleShipGame.Identity;

namespace BattleShipGame
{
    public class GameConfig
    {
        public Player[] Players;
        public uint MapHeight { get; private set; }
        public uint MapWidth { get; private set; }

        public GameConfig WithMapHeight(uint height)
        {
            //Todo: add limits
            MapHeight = height;
            return this;
        }

        public GameConfig WithMapWidth(uint width)
        {
            //Todo: add limits
            MapWidth = width;
            return this;
        }
    }

    public class Game
    {
        //public Map[] Maps;
        public Player[] Players;

        public Game(GameConfig config)
        {
            //Maps = new Map[config.Players.Length];

            Players = config.Players;
            //for (var i = 0; i < config.Players.Length; i++)
            //    Maps[i] = new Map(config.Players[i], config.MapWidth, config.MapHeight);

            for (var i = 0; i < Players.Length; i++)
                Players[i].Map = new Map(Players[i], config.MapWidth, config.MapHeight);
        }

    }
}
