using GeomaceRL.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

namespace GeomaceRL
{
    internal enum EndType
    {
        None,
        Few,
        Many
    }

    internal class JaggedMapGenerator : MapGenerator
    {
        private const int _ROOM_SIZE = 4;
        private const int _ROOM_VARIANCE = 2;

        private const double _BETA = _ROOM_SIZE / _ROOM_VARIANCE;
        private const double _ALPHA = _ROOM_SIZE * _BETA;

        private const double _FILL_PERCENT = 0.05;
        private const double _LOOP_CHANCE = 0.15;

        private const EndType _END_TYPE = EndType.None;

        public JaggedMapGenerator(int width, int height, int level)
            : base(width, height, level)
        {
        }

        protected override void CreateMap()
        {
            // Maintain a list of points bordering the current list of rooms so we can attach
            // more rooms. Also track of the facing of the wall the point comes from.
            IList<int> openPoints = new List<int>();

            // Also keep track of the rooms and where they are.
            IList<Room> initialRooms = new List<Room>();
            int[,] occupied = new int[Width, Height];

            // Set up the initial placement of the map. Include special features, if any.
            int counter = InitialPlacement(initialRooms, openPoints, ref occupied);
            IList<Room> allRooms = new List<Room>(initialRooms);

            while (openPoints.Count > 0)
            {
                // Choose a random point to create a room around;
                int index = openPoints[Rand.Next(openPoints.Count)];
                int availX = index % Width;
                int availY = index / Width;

                // Fit a room around the point as best as possible. AdjustRoom should avoid most
                // collisions between rooms.
                int width = (int)Rand.NextGamma(_ALPHA, _BETA);
                int height = (int)Rand.NextGamma(_ALPHA, _BETA);
                Room room = AdjustRoom(availX, availY, width, height, occupied);

                // Update the room list, the open point list, and the location grid.
                RemoveOpenPoints(room, openPoints);
                if (TrackRoom(room, allRooms, counter + 1, ref occupied))
                {
                    counter++;
                }

                AddOpenPoints(room, openPoints, occupied);
            }

            // Use the largest areas as rooms and triangulate them to calculate hallways.
            RoomList = allRooms
                .OrderByDescending(r => r.Area)
                .Take((int)(allRooms.Count * _FILL_PERCENT))
                .ToList();

            // Ensure that prefab elements are included
            foreach (Room initial in initialRooms)
            {
                if (!RoomList.Contains(initial))
                {
                    RoomList.Add(initial);
                }
            }

            // If we don't get enough rooms, we can't (and don't need to) triangulate, so just
            // draw in what we have.
            if (RoomList.Count == 1)
            {
                CreateRoomWithoutBorder(RoomList[0]);
            }
            else if (RoomList.Count == 2)
            {
                // Add the only hallway to the hall list
                Adjacency = new ICollection<int>[] { new[] { 1 }, new[] { 0 } };
                ClearRoomsBetween(RoomList[0], RoomList[1], allRooms, occupied);
            }
            else
            {
                Polygon polygon = new Polygon();
                foreach (Room room in RoomList)
                {
                    polygon.Add(new Vertex(room.Center.X, room.Center.Y));
                }
                IMesh delaunay = polygon.Triangulate();
                
                // Reduce the number of edges and clear out rooms along the remaining edges.
                ICollection<int>[] adj = BuildAdjacencyList(delaunay.Edges, RoomList.Count);
                List<Edge> edges = TrimEdges(adj, RoomList).ToList();

                // Restore some edges, so exploring is a bit more interesting
                RestoreEdges(edges, delaunay.Edges.ToList(), adj, _END_TYPE);
                Adjacency = BuildAdjacencyList(edges, RoomList.Count);
                
                foreach (Edge edge in edges)
                {
                    ClearRoomsBetween(RoomList[edge.P0], RoomList[edge.P1], allRooms, occupied);
                }
            }

            PostProcess();
        }

        // Set up any special features on the level. After setup is complete, return the value the
        // room ID counter should start at.
        private int InitialPlacement(ICollection<Room> roomList,
            ICollection<int> openPoints, ref int[,] occupied)
        {
            // Default initialization, first room is like all other rooms
            Room first = new Room(
                Rand.Next(Width - _ROOM_SIZE), Rand.Next(Height - _ROOM_SIZE),
                (int)Rand.NextGamma(_ALPHA, _BETA),
                (int)Rand.NextGamma(_ALPHA, _BETA));

            // Ensure that the first room is on the map.
            if (first.Right >= Width)
            {
                first.X -= first.Right - Width + 1;
            }

            if (first.Bottom >= Height)
            {
                first.Y -= first.Bottom - Height + 1;
            }

            if (first.Left < 0)
            {
                first.X = 0;
            }

            if (first.Top < 0)
            {
                first.Y = 0;
            }

            // Can't do anything if the first room doesn't get placed
            bool success = TrackRoom(first, roomList, 1, ref occupied);
            System.Diagnostics.Debug.Assert(success);
            AddOpenPoints(first, openPoints, occupied);
            return 1;
        }

        private void ClearRoomsBetween(Room r1, Room r2, IList<Room> roomList, int[,] occupied)
        {
            int x0 = Rand.Next(Math.Min(r1.Left + 1, r1.Right), r1.Right);
            int y0 = Rand.Next(Math.Min(r1.Top + 1, r1.Bottom), r1.Bottom);
            int x1 = Rand.Next(Math.Min(r2.Left + 1, r2.Right), r2.Right);
            int y1 = Rand.Next(Math.Min(r2.Top + 1, r2.Bottom), r2.Bottom);

            foreach (Loc point in Map.GetStraightLinePath(new Loc(x0, y0), new Loc(x1, y1)))
            {
                Tile tile = Map.Field[point];
                if (!tile.IsWall)
                {
                    continue;
                }

                // Zero corresponds to an unfilled tile, so we need to reduce the ID by 1.
                int roomID = occupied[tile.X, tile.Y] - 1;
                if (roomID >= 0)
                {
                    CreateRoomWithoutBorder(roomList[roomID]);
                }
                else
                {
                    // Map may not be fully tiled, so clear out any untouched squares to avoid
                    // disconnected regions.
                    tile.IsWall = false;
                }
            }
        }

        // Clean up the map by removing stray walls.
        // TODO: fill in 1-tile holes without cutting the map
        // TODO: identify and mark islands
        private void PostProcess()
        {
            // Sweep from top to bottom.
            for (int x = 1; x < Width - 1; x++)
            {
                // Keep a running count of consecutive walls
                // Start the wall count at an arbitrarily high number so walls near edges don't get
                // removed unnecessarily.
                int wallCount = 10;
                for (int y = 3; y < Height - 3; y++)
                {
                    if (Map.Field[x, y].IsWall)
                    {
                        wallCount++;
                    }
                    else
                    {
                        if (wallCount == 1)
                        {
                            Map.Field[x, y - 1].IsWall = false;
                        }

                        wallCount = 0;
                    }
                }
            }

            // Sweep from left to right.
            for (int y = 1; y < Height - 1; y++)
            {
                int wallCount = 10;
                for (int x = 3; x < Width - 3; x++)
                {
                    if (Map.Field[x, y].IsWall)
                    {
                        wallCount++;
                    }
                    else
                    {
                        if (wallCount == 1)
                        {
                            Map.Field[x - 1, y].IsWall = false;
                        }

                        wallCount = 0;
                    }
                }
            }
        }

        // Try to fit the largest room possible up to width x height around (availX, availY).
        // TODO: rewrite AdjustRoom to grow the room with a radial sweepline to avoid collision
        private Room AdjustRoom(int availX, int availY, int width, int height,
            int[,] occupied)
        {
            int left = availX;
            int right = availX;
            int top = availY;
            int bottom = availY;

            for (int dx = 1; dx < width; dx++)
            {
                if (PointOnMap(availX + dx, availY) && occupied[availX + dx, availY] == 0)
                {
                    right = availX + dx;
                }
                else
                {
                    break;
                }
            }

            for (int dx = 1; dx < width - right + availX; dx++)
            {
                if (PointOnMap(availX - dx, availY) && occupied[availX - dx, availY] == 0)
                {
                    left = availX - dx;
                }
                else
                {
                    break;
                }
            }

            for (int dy = 1; dy < height; dy++)
            {
                if (PointOnMap(availX, availY + dy) && occupied[availX, availY + dy] == 0)
                {
                    bottom = availY + dy;
                }
                else
                {
                    break;
                }
            }

            for (int dy = 1; dy < height - bottom + availY; dy++)
            {
                if (PointOnMap(availX, availY - dy) && occupied[availX, availY - dy] == 0)
                {
                    top = availY - dy;
                }
                else
                {
                    break;
                }
            }

            int newWidth = right - left;
            int newHeight = bottom - top;
            return new Room(left, top, newWidth, newHeight);
        }

        // Add a non-zero size room to the room list and update the occupied matrix. Returns false
        // if a room was not added.
        private static bool TrackRoom(Room room, ICollection<Room> roomList, int counter,
            ref int[,] occupied)
        {
            int area = 0;
            for (int x = room.Left; x <= room.Right; x++)
            {
                for (int y = room.Top; y <= room.Bottom; y++)
                {
                    if (occupied[x, y] != 0)
                    {
                        continue;
                    }

                    occupied[x, y] = counter;
                    area++;
                }
            }

            if (area > 0)
            {
                roomList.Add(room);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddOpenPoints(Room room,
            ICollection<int> openPoints, int[,] occupied)
        {
            for (int x = room.Left; x <= room.Right; x++)
            {
                int yTop = room.Top - 1;
                int yBottom = room.Bottom + 1;

                if (PointOnMap(x, yTop) && occupied[x, yTop] == 0)
                {
                    openPoints.Add(ToIndex(x, yTop));
                }

                if (PointOnMap(x, yBottom) && occupied[x, yBottom] == 0)
                {
                    openPoints.Add(ToIndex(x, yBottom));
                }
            }

            for (int y = room.Top; y <= room.Bottom; y++)
            {
                int xLeft = room.Left - 1;
                int xRight = room.Right + 1;

                if (PointOnMap(xLeft, y) && occupied[xLeft, y] == 0)
                {
                    openPoints.Add(ToIndex(xLeft, y));
                }

                if (PointOnMap(xRight, y) && occupied[xRight, y] == 0)
                {
                    openPoints.Add(ToIndex(xRight, y));
                }
            }
        }

        private void RemoveOpenPoints(Room room,
            ICollection<int> openPoints)
        {
            // Only need to check the edges since the adjust step already fits the rectangles.
            for (int x = room.Left; x <= room.Right; x++)
            {
                openPoints.Remove(ToIndex(x, room.Top));
                openPoints.Remove(ToIndex(x, room.Bottom));
            }

            for (int y = room.Top; y <= room.Bottom; y++)
            {
                openPoints.Remove(ToIndex(room.Left, y));
                openPoints.Remove(ToIndex(room.Right, y));
            }
        }

        // Use Prim's algorithm to generate a MST of edges.
        private IEnumerable<Edge> TrimEdges(ICollection<int>[] adjacency, IList<Room> rooms)
        {
            // Comparator for MapVertex is defined to give negated, so this is actually a minheap
            MaxHeap<MapVertex> pq = new MaxHeap<MapVertex>(rooms.Count);

            var (firstX, firstY) = rooms[0].Center;
            pq.Add(new MapVertex(0, firstX, firstY, 0));

            bool[] inMst = new bool[rooms.Count];
            double[] weight = new double[rooms.Count];
            int[] parent = new int[rooms.Count];

            for (int i = 0; i < rooms.Count; i++)
            {
                weight[i] = double.MaxValue;
                parent[i] = -1;
            }

            while (pq.Count > 0)
            {
                MapVertex min = pq.PopMax();
                inMst[min.ID] = true;

                foreach (int neighborID in adjacency[min.ID])
                {
                    if (inMst[neighborID])
                    {
                        continue;
                    }

                    var (neighborX, neighborY) = rooms[neighborID].Center;
                    double newWeight = Distance.EuclideanSquared(min.X, min.Y,
                        neighborX, neighborY);

                    if (weight[neighborID] > newWeight)
                    {
                        weight[neighborID] = newWeight;
                        pq.Add(new MapVertex(neighborID, neighborX, neighborY, newWeight));
                        parent[neighborID] = min.ID;
                    }
                }
            }

            ICollection<Edge> graph = new HashSet<Edge>();
            for (int i = 0; i < rooms.Count; i++)
            {
                if (parent[i] != -1)
                {
                    graph.Add(new Edge(i, parent[i]));
                }
            }

            return graph;
        }

        // Add back some edges so that there loops, preferring rooms at the end of hallways
        private void RestoreEdges(ICollection<Edge> minimal, IList<Edge> full, ICollection<int>[] adjacency,
            EndType endRooms)
        {
            ICollection<int>[] minAdj = BuildAdjacencyList(minimal, RoomList.Count);
            ICollection<int> endSet = new List<int>();
            int maxAdd = (int)(full.Count * _LOOP_CHANCE);
            int added = 0;

            // identify end rooms (only one connection)
            for (int i = 0; i < minAdj.Length; i++)
            {
                if (minAdj[i].Count != 1)
                {
                    continue;
                }

                if (endRooms == EndType.None)
                {
                    // if we don't want end rooms, try to add a loop to everything
                    endSet.Add(i);
                }
                else
                {
                    int adjNode = minAdj[i].First();
                    if (minAdj[adjNode].Count <= 2)
                    {
                        // alternatively, if we want more end rooms, add a loop before the last room
                        if (endRooms == EndType.Many)
                        {
                            endSet.Add(adjNode);
                        }
                        else
                        {
                            endSet.Add(i);
                        }
                    }
                }
            }

            foreach (int index in endSet)
            {
                int adjNode = minAdj[index].First();
                (int currX, int currY) = RoomList[index].Center;
                (int adjX, int adjY) = RoomList[adjNode].Center;
                double angle = Math.Atan2(currY - adjY, currX - adjX);

                foreach (int elem in adjacency[index])
                {
                    (int nextX, int nextY) = RoomList[elem].Center;
                    double newAngle = Math.Atan2(currY - nextY, currX - nextX);

                    // get the minimum angle between the existing and new hallways
                    double diff = Math.Abs(newAngle - angle);
                    if (diff > Math.PI)
                    {
                        diff = (2 * Math.PI) - diff;
                    }

                    // add the new hallway if the angle is >= 60 degrees
                    if (diff >= Math.PI / 3)
                    {
                        minimal.Add(new Edge(index, elem));
                        added++;
                        break;
                    }
                }

                if (added >= maxAdd)
                {
                    break;
                }
            }

            // randomly add the rest of the edges
            for (; added < maxAdd; added++)
            {
                Edge edge = full[Rand.Next(full.Count)];
                minimal.Add(edge);
            }
        }

        private static ICollection<int>[] BuildAdjacencyList(IEnumerable<Edge> edges, int size)
        {
            ICollection<int>[] adjacency = new ICollection<int>[size];
            for (int i = 0; i < size; i++)
            {
                adjacency[i] = new List<int>();
            }

            foreach (Edge edge in edges)
            {
                adjacency[edge.P0].Add(edge.P1);
                adjacency[edge.P1].Add(edge.P0);
            }

            return adjacency;
        }

        private int ToIndex(int x, int y)
        {
            return x + (Width * y);
        }

        private readonly struct MapVertex : IComparable<MapVertex>
        {
            public int ID { get; }
            public int X { get; }
            public int Y { get; }
            private double Weight { get; }

            public MapVertex(int id, int x, int y, double weight)
            {
                ID = id;
                X = x;
                Y = y;
                Weight = weight;
            }

            public int CompareTo(MapVertex other)
            {
                return (int)(other.Weight - Weight);
            }
        }
    }
}
