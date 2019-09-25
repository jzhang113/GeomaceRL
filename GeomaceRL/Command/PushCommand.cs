using GeomaceRL.Animation;
using Optional;

namespace GeomaceRL.Command
{
    internal class PushCommand : ICommand
    {
        public Option<IAnimation> Animation => Option.None<IAnimation>();

        private readonly Actor.Actor _source;
        private readonly Actor.Actor _target;

        public PushCommand(Actor.Actor source, Actor.Actor target)
        {
            _source = source;
            _target = target;
        }

        public Option<ICommand> Execute()
        {
            Loc dir = Distance.GetNearestDirection(_target.Pos, _source.Pos);
            Loc prev = _source.Pos;
            Loc curr = _target.Pos;
            Loc next = curr + dir;

            if (Game.MapHandler.Field[next].IsWalkable)
            {
                Game.MessagePanel.AddMessage($"{_source.Name} pushes {_target.Name}");
                Game.MapHandler.SetActorPosition(_target, next);
                Game.MapHandler.SetActorPosition(_source, curr);
                Game.Animations.Add(_target.Id, new MoveAnimation(_target, curr));
                Game.Animations.Add(_source.Id, new MoveAnimation(_source, prev));
            }
            else if (_source is Actor.Player)
            {
                Game.MessagePanel.AddMessage("There's something in the way");
            }

            return Option.None<ICommand>();
        }
    }
}