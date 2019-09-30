using GeomaceRL.Command;
using Optional;
using System.Linq;

namespace GeomaceRL.Actor
{
    internal class Leech : Actor
    {
        public Leech(in Loc pos, Element element) : base(pos, element)
        {
        }

        public override Option<ICommand> GetAction()
        {
            // if too far, sleep
            int dist = Game.MapHandler.PlayerMap[Pos.X, Pos.Y];
            if (dist == -1 || dist > 7)
                return Option.None<ICommand>();

            // snack on mana of their own element
            Element elem = Game.MapHandler.Mana[Pos.X, Pos.Y];
            if (elem == Element)
            {
                if (Game.MapHandler.Field[Pos].IsVisible)
                    Game.MessagePanel.AddMessage($"Mana eater consumes a {elem} mana");

                Game.MapHandler.Mana[Pos.X, Pos.Y] = Element.None;
                return Option.Some<ICommand>(new WaitCommand(this));
            }
            else
            {
                var nearby = Game.MapHandler.GetPointsInRadius(Pos, 1).Where(point =>
                    Game.MapHandler.Field[point].IsWalkable);

                int index = Game.Rand.Next(nearby.Count());
                return Option.Some<ICommand>(new MoveCommand(this, nearby.ElementAt(index)));
            }
        }
    }
}
