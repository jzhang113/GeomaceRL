using GeomaceRL.Animation;
using GeomaceRL.Interface;
using Optional;
using System.Collections.Generic;

namespace GeomaceRL.Command
{
    internal class AttackCommand : ICommand
    {
        public ISchedulable Source { get; }
        public Option<IAnimation> Animation => Option.None<IAnimation>();

        private readonly (Element, int) _attack;
        private readonly IEnumerable<Loc> _targets;

        public AttackCommand(ISchedulable source, (Element, int) attack, IEnumerable<Loc> targets)
        {
            Source = source;
            _attack = attack;
            _targets = targets;
        }

        public AttackCommand(ISchedulable source, (Element, int) attack, in Loc target) :
            this(source, attack, new[] { target })
        { }

        public Option<ICommand> Execute()
        {
            foreach (Loc point in _targets)
            {
                Game.MapHandler.GetActor(point).MatchSome(target =>
                {
                    Loc attackFrom = Source is Actor.Actor actor ? actor.Pos : target.Pos;
                    int damage = target.TakeDamage(_attack, attackFrom);
                    Game.MessagePanel.AddMessage($"{Source.Name} hits {target.Name} for {damage} hp");
                });
            }

            return Option.None<ICommand>();
        }
    }
}