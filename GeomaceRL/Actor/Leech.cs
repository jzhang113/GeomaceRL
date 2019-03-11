using GeomaceRL.Command;
using Optional;
using System.Linq;

namespace GeomaceRL.Actor
{
    internal class Leech : Actor
    {
        public Leech(in Loc pos, Element element) : base(pos, Constants.LEECH_HP, element, 'L')
        {
            Name = "Mana eater";
            Speed = 2;
        }

        public override Option<ICommand> GetAction()
        {
            // if too far, sleep
            int dist = Game.MapHandler.PlayerMap[Pos.X, Pos.Y];
            if (dist == -1 || dist > 7)
                return Option.None<ICommand>();

            // steal mana or move randomly
            (Element elem, int amount) = Game.MapHandler.Mana[Pos.X, Pos.Y];
            if (elem != Element && amount > 0)
            {
                int newAmount = amount - 3;
                if (newAmount < 0)
                    newAmount = 0;

                if (Game.MapHandler.Field[Pos].IsVisible)
                    Game.MessagePanel.AddMessage($"Mana eater consumes {amount - newAmount} mana");

                Game.MapHandler.Mana[Pos.X, Pos.Y] = (elem, newAmount);
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
