﻿using BearLib;
using GeomaceRL.Actor;
using GeomaceRL.Map;
using GeomaceRL.State;
using GeomaceRL.UI;
using RogueSharp.Random;
using System;
using System.Collections.Generic;

namespace GeomaceRL
{
    internal static class Game
    {
        public static MapHandler MapHandler { get; private set; }
        public static StateHandler StateHandler { get; private set; }
        public static EventScheduler EventScheduler { get; private set; }
        public static Player Player { get; private set; }

        private static bool _exiting;

        private static LayerInfo _mapLayer;
        private static LayerInfo _infoLayer;
        private static LayerInfo _mainLayer;

        private static void Main(string[] args)
        {
            _mapLayer = new LayerInfo("Map", 1,
                Constants.SIDEBAR_WIDTH + 1, 1,
                Constants.MAPVIEW_WIDTH, Constants.MAPVIEW_HEIGHT);

            _infoLayer = new LayerInfo("Info", 1,
                1, 1, Constants.SIDEBAR_WIDTH, Constants.SCREEN_HEIGHT + 2);

            _mainLayer = new LayerInfo("Main", 11, 0, 0,
                Constants.SCREEN_WIDTH + 2, Constants.SCREEN_HEIGHT + 2);

            Terminal.Open();
            Terminal.Set(
                $"window: size={Constants.SCREEN_WIDTH + 1}x{Constants.SCREEN_HEIGHT + 2}," +
                $"cellsize=auto, title='GeomanceRL';");
            Terminal.Set("font: square.ttf, size = 12x12;");

            _exiting = false;

            Player = new Player(new Loc(0, 0));
            var rand = new Pcg.PcgRandom();
            EventScheduler = new EventScheduler();
            var mapgen = new BspMapGenerator(60, 60, rand);
            MapHandler = mapgen.Generate();

            StateHandler = new StateHandler(new Dictionary<Type, LayerInfo>
            {
                //[typeof(ApplyState)] = _rightLayer,
                //[typeof(AutoexploreState)] = _mapLayer,
                //[typeof(DropState)] = _rightLayer,
                //[typeof(EquipState)] = _rightLayer,
                //[typeof(InventoryState)] = _rightLayer,
                //[typeof(SubinvState)] = _rightLayer,
                //[typeof(ItemMenuState)] = _rightLayer,
                [typeof(NormalState)] = _mapLayer,
                //[typeof(TargettingState)] = _mapLayer,
                //[typeof(TextInputState)] = _mapLayer,
                //[typeof(UnequipState)] = _rightLayer
            });

            var dumdum = new Actor.Actor(Player.Pos - (1, 1), 10, Colors.Water, 'D');
            MapHandler.AddActor(dumdum);

            Run();
        }

        internal static void GameOver()
        {
            Console.WriteLine("Game over!");
        }

        internal static void Exit()
        {
            _exiting = true;
        }

        private static void Run()
        {
            while (!_exiting)
            {
                StateHandler.HandleInput().MatchSome(command =>
                {
                    var retry = command.Execute();
                    while (retry.HasValue)
                    {
                        retry.MatchSome(c => retry = c.Execute());
                    }

                    MapHandler.Refresh();
                    EventScheduler.Update();
                });
                Render();
            }

            Terminal.Close();
        }

        private static void Render()
        {
            Terminal.Clear();
            StateHandler.Draw();
            Terminal.Refresh();
        }
    }
}
