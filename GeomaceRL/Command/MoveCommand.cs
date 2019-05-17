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
                return Option.None<ICommand>();

            // Don't walk into walls, unless the Actor is currently phasing or we are already
            // inside a wall (to prevent getting stuck).
            if (Game.MapHandler.Field[_nextPos.X, _nextPos.Y].IsWall)
            {
                // Don't penalize the player for walking into walls, but monsters should wait if 
                // they will walk into a wall.
                if (Source is Player)
                    Game.PrevCancelled = true;

                return Option.None<ICommand>();
            }

            // Check if the destination is already occupied.
            return Game.MapHandler.GetActor(_nextPos).Match(
                some: target =>
                {
                    if (target == Source)
                        return Option.None<ICommand>();
                    else if (target is Pillar)
                        return Option.Some<ICommand>(new PushCommand(Source, target));
                    else
                        return Option.Some(Source.GetBasicAttack(_nextPos));
                },
                none: () =>
                {
                    var action = Option.None<ICommand>();

                    if (Source is Player)
                    {
                        // autodescend
                        Game.MapHandler.Exit.MatchSome(exitPos =>
                        {
                            if (exitPos == _nextPos)
                            {
                                Game.MessagePanel.AddMessage("You descend the stairs");
                                Game.NextLevel();
                                Game.PrevCancelled = true; // don't activate enemies on the next floor
                            }
                        });

                        // autopickup
                        action = Game.MapHandler.GetItem(_nextPos)
                            .FlatMap(item => Option.Some<ICommand>(new PickupCommand(Source, item)));
                    }

                    Loc prevLoc = Source.Pos;
                    Game.MapHandler.SetActorPosition(Source, _nextPos);

                    if (!(Source is Player))
                    {
                        Animation = Option.Some<IAnimation>(new MoveAnimation(Source, prevLoc, Source.Moving));
                        Source.Moving = true;
                    }

                    return action;
                });
        }
    }
}