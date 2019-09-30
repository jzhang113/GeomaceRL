using GeomaceRL.Animation;
using GeomaceRL.Command;
using Optional;
using System.Linq;

namespace GeomaceRL.Actor
{
    internal class Bomber : Actor
    {
        public Bomber(in Loc pos, Element element) : base(pos, element)
        {
        }

        public override Option<ICommand> TriggerDeath()
        {
            Game.MapHandler.RemoveActor(this);
            Game.Player.KillCount[GetType()]++;

            if (Game.MapHandler.Field[Pos].IsVisible)
            {
                Game.MessagePanel.AddMessage("Bomber explodes");
                Game.MapHandler.Refresh();
            }

            var nearby = Game.MapHandler.GetPointsInRadius(Pos, 2);
            foreach (Loc point in nearby)
            {
                Game.MapHandler.Mana[point.X, point.Y] = Element;
            }

            Game.Animations.Add(Id, new FlashAnimation(nearby, Element.Color()));
            return Option.Some<ICommand>(new AttackCommand(
                   this, (Element, Constants.GEN_ATTACK * 2), nearby));
        }

        public override Option<ICommand> GetAction()
        {
            int dist = Game.MapHandler.PlayerMap[Pos.X, Pos.Y];
            if (dist != -1 && dist <= 2 && Game.MapHandler.Field[Pos].IsVisible)
            {
                return TriggerDeath();
            }
            else if (Game.MapHandler.Field[Pos].IsVisible)
            {
                Loc move = AnnAStar.Search(Pos, Game.Player.Pos, 1).FirstOrDefault();
                return move == default
                    ? Option.None<ICommand>()
                    : Option.Some<ICommand>(new MoveCommand(this, move));
            }
            else
            {
                return Option.Some<ICommand>(new WaitCommand(this));
            }
        }
    }
}
