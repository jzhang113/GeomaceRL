using BearLib;
using GeomaceRL.Actor;
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
        public static AnimationHandler Animations { get; private set; }

        public static PcgRandom Rand { get; private set; }
        public static PcgRandom VisRand { get; private set; }

        internal static TimeSpan Ticks;
        internal static TimeSpan FrameRate = new TimeSpan(TimeSpan.TicksPerSecond / 30);

        internal static int GlobalId = 0;
        // HACK: how to communicate cancelled moved to the main loop?
        internal static bool PrevCancelled = false;

        internal static IDictionary<Type, (string Name, char Symbol, int Hp, int Speed)> ActorData { get; private set; }

        private static bool _exiting;
        private static bool _playing;
        private static int _level;

        private static LayerInfo _mapLayer;
        private static LayerInfo _infoLayer;
        private static LayerInfo _messageLayer;
        private static LayerInfo _manaLayer;
        private static LayerInfo _spellbarLayer;
        private static LayerInfo _mainLayer;

        internal static readonly (int, int)[] _levelSize = {
            (30, 30), (30, 30), (45, 45), (60, 60), (60, 60), (10, 10) };

        internal static readonly int[] _enemyCount = { 5, 15, 25, 35, 45, 0 };

        private static void Main(string[] args)
        {
            ActorData = new Dictionary<Type, (string, char, int, int)>
            {
                [typeof(Actor.Actor)]   = ("Monster", '_', 1, 2),
                [typeof(Bomber)]        = ("Bomber", 'B', 1, 1),
                [typeof(Elemental)]     = ("Elemental", 'E', 2, 2),
                [typeof(Leech)]         = ("Leech", 'L', 1, 2),
                [typeof(Pillar)]        = ("Pillar", 'O', 1, 2),
                [typeof(Player)]        = ("Player", '@', 8, 2),
                [typeof(Sprite)]        = ("Sprite", 'S', 1, 1),
            };

            Animations = new AnimationHandler();
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
            Terminal.Set("input.filter = [keyboard, mouse]");

            StateHandler = new StateHandler(new Dictionary<Type, LayerInfo>
            {
                [typeof(NormalState)] = _mapLayer,
                [typeof(TargettingState)] = _mapLayer,
                [typeof(SpellReplaceState)] = _mapLayer,
                [typeof(MenuState)] = _mainLayer,
                [typeof(DeathState)] = _mainLayer,
            });

            MessagePanel = new MessagePanel(Constants.MESSAGE_HISTORY_COUNT);
            EventScheduler = new EventScheduler();

            _exiting = false;
            _playing = false;

            Terminal.Refresh();
            Run();
        }

        public static void NewGame()
        {
            StateHandler.Reset();
            MessagePanel.Clear();
            Animations.Clear();
            GlobalId = 0;

            Player = new Player(new Loc(0, 0));

            Colors.RandomizeMappings();
            _playing = true;

            _level = 0;
            NextLevel();
            StateHandler.PushState(NormalState.Instance);
            _mainLayer.Clear();
        }

        internal static void NextLevel()
        {
            Animations.Clear();
            EventScheduler.Clear();

            if (_level >= 5)
            {
                MessagePanel.AddMessage("This appears to be the end of the dungeon");
                MessagePanel.AddMessage("You win!");
            }
            else
            {
                MessagePanel.AddMessage($"You arrive at level {_level + 1}");
            }

            (int, int) size = _levelSize[_level];
            var mapgen = new JaggedMapGenerator(size.Item1, size.Item2, _level);
            MapHandler = mapgen.Generate();
            MapHandler.Refresh();
            _level++;
        }

        internal static void Exit() => _exiting = true;

        private static void Run()
        {
            DateTime currentTime = DateTime.UtcNow;
            var accum = new TimeSpan();

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
                    if (Animations.IsDone())
                    {
                        EventScheduler.ExecuteCommand(0, StateHandler.HandleInput(), ResolveTurn);
                    }
                    else if (Terminal.HasInput())
                    {
                        Terminal.Read();
                    }

                    Ticks += FrameRate;
                    accum -= FrameRate;
                }

                double remaining = accum / FrameRate;
                Animations.Run(frameTime, remaining);

                Render();
            }

            Terminal.Close();
        }

        private static void ResolveTurn()
        {
            if (!PrevCancelled)
            {
                MapHandler.Refresh();
                EventScheduler.Update();

                if (Player.IsDead)
                {
                    Player.TriggerDeath();
                    StateHandler.Reset();
                    StateHandler.PushState(DeathState.Instance);
                }
            }
            else
            {
                PrevCancelled = false;
            }
        }

        private static void Render()
        {
            Terminal.Clear();
            if (_playing)
            {
                ViewPanel.Draw(_infoLayer);
                InfoPanel.Draw(_manaLayer);
                Spellbar.Draw(_spellbarLayer);
                MessagePanel.Draw(_messageLayer);
            }
            StateHandler.Draw();
            Animations.Draw(_mapLayer);

            Terminal.Refresh();
        }
    }
}
