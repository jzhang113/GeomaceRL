using GeomaceRL.Actor;
using GeomaceRL.Animation;
using Optional;

namespace GeomaceRL.Command
{
    internal class MoveCommand : ICommand
    {
        public Actor.Actor Source { get; }
        public Option<IAnimation> Animation { get; private set; }

        private readonly Loc _nextPos;

        public MoveCommand(Actor.Actor source, in Loc pos)
        {
            Source = source;
            _nextPos = pos;
        }

        public Option<ICommand> Execute()
        {
            // Cancel out of bound moves.
            if (!Game.MapHandler.Field.IsValid(_nextPos))
                return Option.Some<ICommand>(new WaitCommand(Source));

            // Don't walk into walls, unless the Actor is currently phasing or we are already
            // inside a wall (to prevent getting stuck).
            if (Game.MapHandler.Field[_nextPos.X, _nextPos.Y].IsWall)
            {
                // Don't penalize the player for walking into walls, but monsters should wait if 
                // they will walk into a wall.
                if (Source is Player)
                    return Option.None<ICommand>();
                else
                    return Option.Some<ICommand>(new WaitCommand(Source));
            }

            // Check if the destination is already occupied.
            return Game.MapHandler.GetActor(_nextPos).Match(
                some: target => target == Source
                    ? Option.Some<ICommand>(new WaitCommand(Source))
                    : Option.Some(Source.GetBasicAttack(_nextPos)),
                none: () =>
                {
                    if (Source is Player)
                    {
                        // TODO: better handling of move over popups
                        //Game.MapHelper.GetStack(_nextPos).MatchSome(stack =>
                        //    Game.MessageHandler.AddMessage(stack.Count == 1
                        //        ? $"You see {stack.First()} here."
                        //        : "You see several items here."));

                        //Game.MapHelper.GetExit(_nextPos)
                        //    .MatchSome(exit => Game.MessageHandler.AddMessage($"You see an exit to {exit.Destination}."));
                    }

                    Loc prevLoc = Source.Pos;
                    Game.MapHandler.SetActorPosition(Source, _nextPos);
                    Animation = Option.Some<IAnimation>(new MoveAnimation(Source, prevLoc));
                    return Option.None<ICommand>();
                });
        }
    }
}