using GeomaceRL.Actor;
using GeomaceRL.Animation;
using Optional;
using System.Collections.Generic;

namespace GeomaceRL.Command
{
    internal class AttackMoveCommand : ICommand
    {
        public Actor.Actor Source { get; }
        public Option<IAnimation> Animation { get; private set; }

        private readonly (Element, int) _attack;
        private readonly IEnumerable<Loc> _targets;
        private readonly Loc _dir;
        private readonly int _distance;

        public AttackMoveCommand(
            Actor.Actor source,
            (Element, int) attack,
            IEnumerable<Loc> targets,
            in Loc dir,
            int distance)
        {
            Source = source;
            _attack = attack;
            _targets = targets;
            _dir = dir;
            _distance = distance;
        }

        public Option<ICommand> Execute()
        {
            foreach (Loc point in _targets)
            {
                Game.MapHandler.GetActor(point).MatchSome(target =>
                {
                    int damage = target.TakeDamage(_attack, Source.Pos);
                    Game.MessagePanel.AddMessage($"{Source.Name} attacks {target.Name} for {damage} damage");
                });
            }

            // Don't move through walls or enemies
            Loc nextPos = Source.Pos;
            bool collided = false;

            for (int i = 1; i <= _distance; i++)
            {
                nextPos += _dir;
                if (!Game.MapHandler.Field[nextPos].IsWalkable)
                {
                    collided = true;
                    break;
                }
            }

            if (!collided)
            {
                return MakeMoveCommand(nextPos);
            }
            else
            {
                // check if we hit an enemy
                Loc newPos = nextPos - _dir;
                return Game.MapHandler.GetActor(nextPos).Match(
                    some: target =>
                    {
                        // collision damage
                        // TODO: scale collision damage by weight and distance travelled
                        int damage = target.TakeDamage((Source.Element, Constants.COLLISION_DAMAGE), Source.Pos);
                        Game.MessagePanel.AddMessage($"{Source.Name} slams into {target.Name} for {damage} hp");
                        return MakeMoveCommand(newPos);
                    },
                    none: () => MakeMoveCommand(newPos));
            }
        }

        private Option<ICommand> MakeMoveCommand(in Loc nextPos)
        {
            if (Source is Player)
            {
                Game.MapHandler.Exit.MatchSome(exit =>
                {
                    if (exit == Source.Pos)
                        Game.MessagePanel.AddMessage("You see an exit here");
                });
            }

            Loc prevLoc = Source.Pos;
            Game.MapHandler.SetActorPosition(Source, nextPos);
            Animation = Option.Some<IAnimation>(new MoveAnimation(Source, prevLoc));
            return Option.None<ICommand>();
        }
    }
}