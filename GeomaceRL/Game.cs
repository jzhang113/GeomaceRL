using BearLib;
using GeomaceRL.Actor;
using GeomaceRL.Animation;
using GeomaceRL.Map;
using GeomaceRL.State;
using GeomaceRL.UI;
using Pcg;
using System;
using System.Collections.Generic;
using System.Linq;

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

        internal static TimeSpan Ticks;
        internal static TimeSpan FrameRate = new TimeSpan(TimeSpan.TicksPerSecond / 30);

        // HACK: how to communicate cancelled moved to the main loop?
        internal static bool PrevCancelled = false;

        private static bool _exiting;
        private static bool _playing;
        internal static int _level;
        internal static bool _dead;

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
            CurrentAnimations = new List<IAnimation>();
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

            StateHandler = new StateHandler(new Dictionary<Type, LayerInfo>
            {
                [typeof(NormalState)] = _mapLayer,
                [typeof(TargettingState)] = _mapLayer,
                [typeof(MenuState)] = _mainLayer,
            });

            MessagePanel = new MessagePanel(Constants.MESSAGE_HISTORY_COUNT);
            EventScheduler = new EventScheduler();

            _exiting = false;
            _playing = false;
            _dead = true;

            Terminal.Refresh();
            Run();
        }

        public static void NewGame()
        {
            StateHandler.Reset();
            MessagePanel.Clear();
            CurrentAnimations.Clear();
            Player = new Player(new Loc(0, 0));

            _playing = true;
            _dead = false;

            _level = 0;
            NextLevel();
            StateHandler.PushState(NormalState.Instance);
            _mainLayer.Clear();
        }

        internal static void NextLevel()
        {
            CurrentAnimations.Clear();
            EventScheduler.Clear();

            if (_level >= 5)
            {
                MessagePanel.AddMessage("This appears to be the end of the dungeon");
                MessagePanel.AddMessage("You win!");
                _dead = true;
            }
            else
            {
                MessagePanel.AddMessage($"You arrive at level {_level+1}");
            }

            var size = _levelSize[_level];
            var mapgen = new JaggedMapGenerator(size.Item1, size.Item2, _level);
            MapHandler = mapgen.Generate();
            _level++;
        }

        internal static void GameOver()
        {
            MessagePanel.AddMessage("Game over! Press any key to continue");
            _dead = true;
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
            IAnimation current = null;

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
                    if (current == null)
                    {
                        EventScheduler.ExecuteCommand(StateHandler.HandleInput(), () =>
                        {
                            if (!PrevCancelled)
                            {
                                MapHandler.Refresh();
                                EventScheduler.Update();
                            }
                            else
                            {
                                PrevCancelled = false;
                            }
                        });
                    }
                    else if (Terminal.HasInput())
                    {
                        Terminal.Read();
                    }

                    Ticks += FrameRate;
                    accum -= FrameRate;
                }

                double remaining = accum / FrameRate;

                if (current == null)
                {
                    current = CurrentAnimations.FirstOrDefault();
                }
                else if (current.Update(frameTime) || EventScheduler.Turn > current.Turn + 1)
                {
                    current.Cleanup();
                    CurrentAnimations.Remove(current);

                    if (current is MoveAnimation currMove
                        && MapHandler.Field[currMove._source.Pos].IsVisible)
                    {
                        currMove._source.ShouldDraw = true;

                        foreach (IAnimation animation in CurrentAnimations)
                        {
                            if (animation != current
                                && animation is MoveAnimation nextMove
                                && nextMove._source == currMove._source)
                            {
                                nextMove._source.ShouldDraw = false;
                                nextMove._multmove = false;
                            }
                        }
                    }

                    current = CurrentAnimations.FirstOrDefault();
                }

                Render();
            }

            Terminal.Close();
        }

        private static void Render()
        {
            Terminal.Clear();
            if (_playing)
            {
                InfoPanel.Draw(_infoLayer);
                ManaPanel.Draw(_manaLayer);
                Spellbar.Draw(_spellbarLayer);
                MessagePanel.Draw(_messageLayer);
            }
            StateHandler.Draw();

            foreach (IAnimation animation in CurrentAnimations)
            {
                animation.Draw(_mapLayer);
            }

            Terminal.Refresh();
        }
    }
}
