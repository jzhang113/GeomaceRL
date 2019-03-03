﻿using GeomaceRL.Command;
using Optional;

namespace GeomaceRL.Actor
{
    public class Sprite : Actor
    {
        public Sprite(in Loc pos, Element element)
            :base(pos, Constants.SPRITE_HP, element.Color(), 'S')
        {
        }

        public override Option<ICommand> GetAction()
        {
            if (Game.MapHandler.PlayerMap[Pos.X, Pos.Y] < 5)
            {
                var move = Game.MapHandler.MoveTowardsTarget(Pos, Game.MapHandler.PlayerMap);
                return Option.Some<ICommand>(new MoveCommand(this, move.Loc));
            }
            else
            {
                return Option.Some<ICommand>(new WaitCommand(this));
            }
        }
    }
}
