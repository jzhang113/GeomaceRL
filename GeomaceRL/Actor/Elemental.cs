using GeomaceRL.Animation;
using GeomaceRL.Command;
using Optional;
using System.Linq;

namespace GeomaceRL.Actor
{
    internal class Elemental : Actor
    {
        public Elemental(in Loc pos, Element element)
            : base(pos, Constants.ELEMENTAL_HP, element, 'E')
        {
            Name = "Elemental";
            Speed = 2;
        }

        public override Option<ICommand> GetAction()
        {
            int dist = Game.MapHandler.PlayerMap[Pos.X, Pos.Y];
            if (dist > 4 && dist < 10)
            {
                // out of range, move in
                Loc move = AnnAStar.Search(Pos, Game.Player.Pos, 1).FirstOrDefault();
                return move == default
                    ? Option.None<ICommand>()
                    : Option.Some<ICommand>(new MoveCommand(this, move));
            }
            else if (dist <= 5)
            {
                // in range, has mana
                if (Game.MapHandler.Mana[Pos.X, Pos.Y].Item1 == Element)
                {
                    // check LOS exists
                    if (Game.MapHandler.Field[Pos].IsVisible)
                    {
                        Game.MapHandler.UpdateAllMana(Pos, Element, 2);
                        Game.CurrentAnimations.Add(new FlashAnimation(
                            new Loc[] { Game.Player.Pos }, Element.Color()));
                        return Option.Some<ICommand>(new AttackCommand(
                           this,
                           (Element, Constants.GEN_ATTACK),
                           Game.Player.Pos));
                    }
                    else
                    {
                        return Option.Some<ICommand>(new WaitCommand(this));
                    }
                }
                else
                {
                    // no mana, move to nearest mana
                    var nearby = Game.MapHandler.GetPointsInRadius(Pos, 1).Where(point =>
                        Game.MapHandler.Field[point].IsWalkable);
                    foreach (Loc point in nearby)
                    {
                        if (Game.MapHandler.Mana[point.X, point.Y].Item1 == Element)
                            return Option.Some<ICommand>(new MoveCommand(this, point));
                    }

                    // no mana in range 1, move randomly
                    // TODO: proper pathing to farther mana sources
                    int index = Game.Rand.Next(nearby.Count());
                    return Option.Some<ICommand>(new MoveCommand(this, nearby.ElementAt(index)));
                }
            }
            else
            {
                return Option.Some<ICommand>(new WaitCommand(this));
            }
        }
    }
}
