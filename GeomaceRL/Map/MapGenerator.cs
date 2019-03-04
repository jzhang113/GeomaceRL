using GeomaceRL.Actor;
using Pcg;
using System;
using System.Collections.Generic;

namespace GeomaceRL.Map
{
    public abstract class MapGenerator
    {
        protected int Width { get; }
        protected int Height { get; }
        protected PcgRandom Rand { get; }
        protected MapHandler Map { get; }

        protected IList<Room> RoomList { get; set; }
        protected ICollection<int>[] Adjacency { get; set; }

        protected MapGenerator(int width, int height, PcgRandom random)
        {
            Width = width;
            Height = height;
            Rand = random;
            Map = new MapHandler(width, height);
        }

        public MapHandler Generate()
        {
            CreateMap();
            ComputeClearance();
            AddElements();

            PlaceActors();
            PlaceItems();
            PlaceStairs();

            return Map;
        }

        protected abstract void CreateMap();

        private void AddElements()
        {
            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    Element element = (Element)Rand.Next(5);
                    int amount = (int)Rand.NextNormal(4, 2);
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

        //protected void PlaceDoors(Room room)
        //{
        //    for (int i = room.Left; i < room.Right; i++)
        //    {
        //        if (i <= 0 || i >= Width - 1)
        //            continue;

        //        if (room.Top > 1 && IsDoorLocation(i, room.Top - 1))
        //        {
        //            Map.AddDoor(new Door(new Loc(i, room.Top - 1)));
        //        }

        //        if (room.Bottom < Height - 1 && IsDoorLocation(i, room.Bottom))
        //        {
        //            Map.AddDoor(new Door(new Loc(i, room.Bottom)));
        //        }
        //    }

        //    for (int j = room.Top; j < room.Bottom; j++)
        //    {
        //        if (j <= 0 || j >= Height - 1)
        //            continue;

        //        if (room.Left > 1 && IsDoorLocation(room.Left - 1, j))
        //        {
        //            Map.AddDoor(new Door(new Loc(room.Left - 1, j)));
        //        }

        //        if (room.Right < Width - 1 && IsDoorLocation(room.Right, j))
        //        {
        //            Map.AddDoor(new Door(new Loc(room.Right, j)));
        //        }
        //    }
        //}

        private bool IsDoorLocation(int x, int y)
        {
            bool current = Map.Field[x, y].IsWall;
            bool left = Map.Field[x - 1, y].IsWall;
            bool right = Map.Field[x + 1, y].IsWall;
            bool up = Map.Field[x, y - 1].IsWall;
            bool down = Map.Field[x, y + 1].IsWall;

            if (current)
                return false;

            if (left && right && !up && !down)
                return true;

            if (!left && !right && up && down)
                return true;

            return false;
        }

        // HACK: ad-hoc placement code
        private void PlaceItems()
        {
        }

        // HACK: ad-hoc placement code
        private void PlaceActors()
        {
            do
            {
                Game.Player.Pos = new Loc(Rand.Next(1, Width - 1), Rand.Next(1, Height - 1));
            }
            while (!Map.Field[Game.Player.Pos].IsWalkable);
            Map.AddActor(Game.Player);

            for (int i = 0; i < 10; i++)
            {
                int xPos = 0, yPos = 0;
                do
                {
                    xPos = Rand.Next(1, Width - 1);
                    yPos = Rand.Next(1, Height - 1);
                } while (!Map.Field[xPos, yPos].IsWalkable);
                var sprite = new Sprite(new Loc(xPos, yPos), Element.Fire);
                Map.AddActor(sprite);
            }

            Map.Refresh();
        }

        private void PlaceStairs()
        {
            //foreach (LevelId id in Exits)
            //{
            //    LevelId current = Game.World.CurrentLevel;
            //    char symbol = '*';
            //    if (id.Name == current.Name)
            //        symbol = id.Depth > current.Depth ? '>' : '<';

            //    Exit exit = new Exit(id, symbol);
            //    bool done = false;
            //    while (!Map.Field[exit.Loc].IsWalkable && !done)
            //    {
            //        Map.GetExit(exit.Loc).Match(
            //            some: _ => exit.Loc = new Loc(Rand.Next(1, Width - 1), Rand.Next(1, Height - 1)),
            //            none: () => done = true);
            //    }

            //    Map.AddExit(exit);
            //}
        }

        protected bool PointOnMap(int x, int y)
        {
            return x > 0 && y > 0 && x < Width - 1 && y < Height - 1;
        }
    }
}
