using GeomaceRL.Actor;
using GeomaceRL.Items;
using Optional;
using Pcg;
using System;
using System.Collections.Generic;

namespace GeomaceRL.Map
{
    public abstract class MapGenerator
    {
        protected int Width { get; }
        protected int Height { get; }
        protected PcgRandom Rand { get; } = Game.Rand;
        protected MapHandler Map { get; }

        protected IList<Room> RoomList { get; set; }
        protected ICollection<int>[] Adjacency { get; set; }

        protected MapGenerator(int width, int height, int level)
        {
            Width = width;
            Height = height;
            Map = new MapHandler(width, height, level);
        }

        public MapHandler Generate()
        {
            CreateMap();
            ComputeClearance();
            AddMana();

            PlaceStairs();
            PlaceActors();
            PlaceItems();

            return Map;
        }

        protected abstract void CreateMap();

        private void AddMana()
        {
            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    var element = (Element)(Rand.Next(4) + 1);
                    int amount = (int)Rand.NextNormal(3, 2);
                    if (amount < 0)
                        amount = 0;

                    Map.Mana[x, y] = (element, amount);
                }
            }
        }

        // Calculate and save how much space is around each square
        private void ComputeClearance()
        {
            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    int d = 0;
                    while (true)
                    {
                        for (int c = 0; c <= d; c++)
                        {
                            if (Map.Field[x + c, y].IsWall || Map.Field[x, y + c].IsWall)
                                goto done;
                        }

                        d++;
                    }

                    done:
                    Map.Clearance[x, y] = d;
                }
            }
        }

        protected void CreateRoom(Room room)
        {
            for (int i = room.Left; i < room.Right; i++)
            {
                for (int j = room.Top; j < room.Bottom; j++)
                {
                    // Don't excavate the edges of the map
                    if (PointOnMap(i, j))
                        Map.Field[i, j].IsWall = false;
                }
            }
        }

        // Similar to CreateRoom, but doesn't leave a border.
        protected void CreateRoomWithoutBorder(Room room)
        {
            for (int i = room.Left; i <= room.Right; i++)
            {
                for (int j = room.Top; j <= room.Bottom; j++)
                {
                    if (PointOnMap(i, j))
                        Map.Field[i, j].IsWall = false;
                }
            }
        }

        protected void CreateHallway(int x1, int y1, int x2, int y2)
        {
            int dx = Math.Abs(x1 - x2);
            int dy = Math.Abs(y1 - y2);

            if (x1 < x2)
            {
                if (y1 < y2)
                {
                    CreateRoom(new Room(x2 - dx, y2, dx + 1, 1));
                    CreateRoom(new Room(x1, y1, 1, dy + 1));
                }
                else
                {
                    CreateRoom(new Room(x1, y1, dx + 1, 1));
                    CreateRoom(new Room(x2, y2, 1, dy + 1));
                }
            }
            else
            {
                if (y1 < y2)
                {
                    CreateRoom(new Room(x2, y2, dx + 1, 1));
                    CreateRoom(new Room(x1, y1, 1, dy + 1));
                }
                else
                {
                    CreateRoom(new Room(x1 - dx, y1, dx + 1, 1));
                    CreateRoom(new Room(x2, y2, 1, dy + 1));
                }
            }
        }

        // HACK: ad-hoc placement code
        private void PlaceItems()
        {
            int count = Rand.Next(1, 3);
            for (int i = 0; i < count; i++) {
                var spell = new SpellScroll(Map.GetRandomOpenPoint(), new Spell.Heal());
                Map.AddItem(spell);
            }

            // TODO: ensure heal spells get generated when prev level was full cleared
        }

        // HACK: ad-hoc placement code
        private void PlaceActors()
        {
            Game.Player.Pos = Map.GetRandomOpenPoint();
            Map.AddActor(Game.Player);

            for (int i = 0; i < Game._enemyCount[Game._level]; i++)
            {
                var element = (Element)(Rand.Next(4) + 1);
                int enemyType = Rand.Next(4);
                Actor.Actor enemy;

                if (enemyType == 0)
                    enemy = new Sprite(Map.GetRandomOpenPoint(), element);
                else if (enemyType == 1)
                    enemy = new Elemental(Map.GetRandomOpenPoint(), element);
                else if (enemyType == 2)
                    enemy = new Bomber(Map.GetRandomOpenPoint(), element);
                else
                    enemy = new Leech(Map.GetRandomOpenPoint(), element);

                Map.AddActor(enemy);
            }

            Map.Refresh();
        }

        private void PlaceStairs()
        {
            Map.Exit = Option.Some(Map.GetRandomOpenPoint());
        }

        protected bool PointOnMap(int x, int y)
        {
            return x > 0 && y > 0 && x < Width - 1 && y < Height - 1;
        }
    }
}
