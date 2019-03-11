using GeomaceRL.Command;
using Optional;
using System.Linq;

namespace GeomaceRL.Actor
{
    public class Sprite : Actor
    {
        public Sprite(in Loc pos, Element element)
            :base(pos, Constants.SPRITE_HP, element, 'S')
        {
            Name = "Sprite";
        }

        public override Option<ICommand> GetAction()
        {
            int dist = Game.MapHandler.PlayerMap[Pos.X, Pos.Y];
            if (dist != -1 && dist < 10)
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
