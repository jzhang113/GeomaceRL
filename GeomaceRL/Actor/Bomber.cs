using GeomaceRL.Animation;
using GeomaceRL.Command;
using Optional;
using System.Linq;

namespace GeomaceRL.Actor
{
    internal class Bomber : Actor
    {
        public Bomber(in Loc pos, Element element) : base(pos, 1, element, 'B')
        {
            Name = "Bomber";
        }

        public override Option<ICommand> TriggerDeath()
        {
            Game.MapHandler.RemoveActor(this);

            if (Game.MapHandler.Field[Pos].IsVisible)
            {
                Game.MessagePanel.AddMessage("Bomber explodes");
                Game.MapHandler.Refresh();
            }

            var nearby = Game.MapHandler.GetPointsInRadius(Pos, 2);
            foreach (Loc point in nearby)
            {
                int newMana = (int)Game.Rand.NextNormal(2, 2);
                if (newMana < 0)
                    newMana = 0;

                (Element elem, int amount) = Game.MapHandler.Mana[point.X, point.Y];

                if (elem == Element)
                {
                    newMana += amount;
                    if (newMana > 9)
                        newMana = 9;
                }
                else if (newMana >= amount)
                {
                    elem = Element;
                }

                Game.MapHandler.Mana[point.X, point.Y] = (elem, newMana);
            }

            Game.CurrentAnimations.Add(new FlashAnimation(nearby, Element.Color()));
            return Option.Some<ICommand>(new AttackCommand(
                   this, (Element, Constants.GEN_ATTACK * 2), nearby));
        }

        public override Option<ICommand> GetAction()
        {
            int dist = Game.MapHandler.PlayerMap[Pos.X, Pos.Y];
            if (dist != -1 && dist <= 2)
            {
                return TriggerDeath();
            }
            else if (dist != -1 && dist < 10)
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
