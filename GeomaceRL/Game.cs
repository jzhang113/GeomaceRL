using BearLib;
using GeomaceRL.Actor;
using GeomaceRL.Animation;
using GeomaceRL.Map;
using GeomaceRL.State;
using GeomaceRL.UI;
using Pcg;
using System;
using System.Collections.Generic;

namespace GeomaceRL
{
    internal static class Game
    {
        public static MapHandler MapHandler { get; private set; }
        public static StateHandler StateHandler { get; private set; }
        public static EventScheduler EventScheduler { get; private set; }
        public static MessagePanel MessagePanel { get; private set; }
        public static Player Player { get; private set; }

        public static PcgRandom Rand { get; private set; }
        public static PcgRandom VisRand { get; private set; }

        internal static ICollection<IAnimation> CurrentAnimations { get; private set; }
        private static ICollection<IAnimation> _finishedAnimations;

        internal static TimeSpan Ticks;
        internal static TimeSpan FrameRate = new TimeSpan(TimeSpan.TicksPerSecond / 30);

        private static int _level;
        private static bool _exiting;

        private static LayerInfo _mapLayer;
        private static LayerInfo _infoLayer;
        private static LayerInfo _messageLayer;
        private static LayerInfo _manaLayer;
        private static LayerInfo _spellbarLayer;
        private static LayerInfo _mainLayer;

        private static void Main(string[] args)
        {
            CurrentAnimations = new List<IAnimation>();
            _finishedAnimations = new List<IAnimation>();
            Rand = new PcgRandom();
            VisRand = new PcgRandom();

            _mapLayer = new LayerInfo("Map", 1,
                Constants.SIDEBAR_WIDTH + 2, 1,
                Constants.MAPVIEW_WIDTH, Constants.MAPVIEW_HEIGHT);

            _infoLayer = new LayerInfo("Info", 1,
                1, 1, Constants.SIDEBAR_WIDTH, Constants.MAPVIEW_HEIGHT);

            _manaLayer = new LayerInfo("Mana", 1,
                1, Constants.MAPVIEW_HEIGHT + 2,
                Constants.SIDEBAR_WIDTH, Constants.MESSAGE_HEIGHT + Constants.SPELLBAR_HEIGHT + 1);

            _messageLayer = new LayerInfo("Message", 1,
                Constants.SIDEBAR_WIDTH + 2, Constants.MAPVIEW_HEIGHT + 2,
                Constants.MAPVIEW_WIDTH, Constants.MESSAGE_HEIGHT);

            _spellbarLayer = new LayerInfo("Spellbar", 1,
                Constants.SIDEBAR_WIDTH + 2, Constants.MAPVIEW_HEIGHT + Constants.MESSAGE_HEIGHT + 3,
                Constants.MAPVIEW_WIDTH, Constants.SPELLBAR_HEIGHT);

            _mainLayer = new LayerInfo("Main", 11, 0, 0,
               Constants.SCREEN_WIDTH + 2, Constants.SCREEN_HEIGHT + 2);

            Terminal.Open();
            Terminal.Set(
                $"window: size={Constants.SCREEN_WIDTH + 2}x{Constants.SCREEN_HEIGHT + 2}," +
                $"cellsize=auto, title='GeomanceRL';");
            Terminal.Set("font: square.ttf, size = 24x24;");
            Terminal.Set("text font: square.ttf, size = 16x16;");

            _level = 1;
            _exiting = false;

            StateHandler = new StateHandler(new Dictionary<Type, LayerInfo>
            {
                //[typeof(ApplyState)] = _rightLayer,
                //[typeof(DropState)] = _rightLayer,
                //[typeof(EquipState)] = _rightLayer,
                //[typeof(InventoryState)] = _rightLayer,
                //[typeof(SubinvState)] = _rightLayer,
                //[typeof(ItemMenuState)] = _rightLayer,
                [typeof(NormalState)] = _mapLayer,
                [typeof(TargettingState)] = _mapLayer,
                //[typeof(TextInputState)] = _mapLayer,
                //[typeof(UnequipState)] = _rightLayer
            });

            MessagePanel = new MessagePanel(Constants.MESSAGE_HISTORY_COUNT);

            Player = new Player(new Loc(0, 0));
            EventScheduler = new EventScheduler();
            NextLevel();

            Terminal.Refresh();
            Run();
        }

        internal static void NextLevel()
        {
            CurrentAnimations.Clear();
            EventScheduler.Clear();

            var mapgen = new JaggedMapGenerator(Constants.MAP_WIDTH, Constants.MAP_HEIGHT, _level++);
            MapHandler = mapgen.Generate();
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
            DateTime currentTime = DateTime.UtcNow;
            TimeSpan accum = new TimeSpan();

            const int updateLimit = 10;
            TimeSpan maxDt = FrameRate * updateLimit;

            while (!_exiting)
            {
                DateTime newTime = DateTime.UtcNow;
                TimeSpan frameTime = newTime - currentTime;
                if (frameTime > maxDt)
                {
                    frameTime = maxDt;
                }

                currentTime = newTime;
                accum += frameTime;

                while (accum >= FrameRate)
                {
                    EventScheduler.ExecuteCommand(StateHandler.HandleInput(), () =>
                    {
                        MapHandler.Refresh();
                        EventScheduler.Update();
                    });

                    Ticks += FrameRate;
                    accum -= FrameRate;
                }

                double remaining = accum / FrameRate;

                foreach (IAnimation animation in CurrentAnimations)
                {
                    if (animation.Update() || EventScheduler.Turn > animation.Turn + 1)
                    {
                        _finishedAnimations.Add(animation);
                    }
                }

                foreach (IAnimation animation in _finishedAnimations)
                {
                    animation.Cleanup();
                    CurrentAnimations.Remove(animation);
                }

                Render();
                _finishedAnimations.Clear();
            }

            Terminal.Close();
        }

        private static void Render()
        {
            Terminal.Clear();
            InfoPanel.Draw(_infoLayer);
            ManaPanel.Draw(_manaLayer);
            Spellbar.Draw(_spellbarLayer);
            MessagePanel.Draw(_messageLayer);
            StateHandler.Draw();

            foreach (IAnimation animation in CurrentAnimations)
            {
                animation.Draw(_mapLayer);
            }

            Terminal.Refresh();
        }
    }
}
