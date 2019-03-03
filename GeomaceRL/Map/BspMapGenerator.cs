using Pcg;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL.Map
{
    internal class BspMapGenerator : MapGenerator
    {
        private const int _MIN_ROOM_SIZE = 4;
        private const int _MAX_ROOM_SIZE = 15;

        public BspMapGenerator(int width, int height, PcgRandom random)
            : base(width, height, random)
        {
        }

        protected override void CreateMap()
        {
            TreeNode<Room> root = new TreeNode<Room>(new Room(0, 0, Width, Height));
            TreeNode<Room> roomsPartition = PartitionMapBsp(root, _MAX_ROOM_SIZE, _MAX_ROOM_SIZE);
            ICollection<Room> roomsList = new List<Room>();

            MakeRoomsBsp(roomsPartition, ref roomsList);
            foreach (Room r in roomsList)
            {
                //PlaceDoors(r);
            }
        }

        private TreeNode<Room> PartitionMapBsp(TreeNode<Room> current, int minWidth, int minHeight, int dir = -1)
        {
            Room room = current.Value;
            if (dir == -1) dir = Rand.Next(2);

            if (dir % 2 == 0)
            {
                if (room.Width < minWidth)
                {
                    return Rand.Next(room.Height) > minHeight
                        ? PartitionMapBsp(current, minWidth, minHeight, 1)
                        : current;
                }

                int split = Rand.Next(_MIN_ROOM_SIZE, room.Width - _MIN_ROOM_SIZE + 1);
                Room left = new Room(room.X, room.Y, split, room.Height);
                TreeNode<Room> leftChild = PartitionMapBsp(new TreeNode<Room>(current, left),
                    minWidth, minHeight);
                current.AddChild(leftChild);

                Room right = new Room(room.X + split, room.Y, room.Width - split, room.Height);
                TreeNode<Room> rightChild = PartitionMapBsp(new TreeNode<Room>(current, right),
                    minWidth, minHeight);
                current.AddChild(rightChild);
            }
            else
            {
                if (room.Height < minHeight)
                {
                    return Rand.Next(room.Width) > minWidth
                        ? PartitionMapBsp(current, minWidth, minHeight, 0)
                        : current;
                }

                int split = Rand.Next(_MIN_ROOM_SIZE, room.Height - _MIN_ROOM_SIZE + 1);
                Room top = new Room(room.X, room.Y, room.Width, split);
                TreeNode<Room> topChild = PartitionMapBsp(new TreeNode<Room>(current, top),
                    minWidth, minHeight);
                current.AddChild(topChild);

                Room bottom = new Room(room.X, room.Y + split, room.Width, room.Height - split);
                TreeNode<Room> bottomChild = PartitionMapBsp(new TreeNode<Room>(current, bottom),
                    minWidth, minHeight);
                current.AddChild(bottomChild);
            }

            return current;
        }

        private IList<(int X, int Y)> MakeRoomsBsp(TreeNode<Room> root, ref ICollection<Room> roomsList)
        {
            IList<(int X, int Y)> allocated = new List<(int X, int Y)>();
            IList<(int X, int Y)> connections = new List<(int X, int Y)>();

            foreach (TreeNode<Room> child in root.Children)
            {
                IList<(int X, int Y)> suballocated = MakeRoomsBsp(child, ref roomsList);

                if (suballocated.Count <= 0)
                    continue;

                (int X, int Y) point = suballocated[Rand.Next(suballocated.Count)];
                connections.Add(point);
                allocated = allocated.Concat(suballocated).ToList();
            }

            if (root.Children.Count == 0)
            {
                Room boundary = root.Value;
                Room space = new Room(
                    width: Rand.Next(4, boundary.Width),
                    height: Rand.Next(4, boundary.Height));
                space.X = Rand.Next(boundary.X, boundary.X + boundary.Width - space.Width);
                space.Y = Rand.Next(boundary.Y, boundary.Y + boundary.Height - space.Height);

                CreateRoom(space);
                roomsList.Add(space);

                // Add the tiles on the walls which can become doors
                for (int i = space.Left; i < space.Right - 1; i++)
                {
                    allocated.Add((i, space.Top + 1));
                    allocated.Add((i, space.Bottom - 1));
                }

                for (int j = space.Top; j < space.Bottom - 1; j++)
                {
                    allocated.Add((space.Left + 1, j));
                    allocated.Add((space.Right - 1, j));
                }
            }
            else
            {
                var (x1, y1) = connections[0];
                var (x2, y2) = connections[1];
                CreateHallway(x1, y1, x2, y2);
            }

            return allocated;
        }
    }
}
