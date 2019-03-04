﻿using GeomaceRL.Animation;
using GeomaceRL.Interface;
using Optional;
using System.Collections.Generic;

namespace GeomaceRL.Command
{
    internal class AttackCommand : ICommand
    {
        public ISchedulable Source { get; }
        public Option<IAnimation> Animation => Option.None<IAnimation>();

        private readonly int _power;
        private readonly IEnumerable<Loc> _targets;

        public AttackCommand(ISchedulable source, int power, IEnumerable<Loc> targets)
        {
            Source = source;
            _power = power;
            _targets = targets;
        }

        public AttackCommand(ISchedulable source, int power, in Loc target) :
            this(source, power, new[] { target }) { }

        public Option<ICommand> Execute()
        {
            foreach (Loc point in _targets)
            {
                Game.MapHandler.GetActor(point).MatchSome(target => {
                    target.TakeDamage(_power);
                    Game.MessagePanel.AddMessage($"{Source.Name} attacks {target.Name} for {_power} damage");
                });
            }

            return Option.None<ICommand>();
        }
    }
}