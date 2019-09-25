using GeomaceRL.Actor;
using GeomaceRL.Command;
using GeomaceRL.Interface;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL
{
    public class EventScheduler
    {
        private readonly IDictionary<ISchedulable, int> _schedule;

        public static int Turn { get; private set; }

        public EventScheduler()
        {
            Turn = 0;
            _schedule = new Dictionary<ISchedulable, int>();
        }

        public void Clear()
        {
            _schedule.Clear();
        }

        internal void AddActor(Actor.Actor unit)
        {
            _schedule.Add(unit, unit.Speed);
        }

        internal void RemoveActor(Actor.Actor unit)
        {
            _schedule.Remove(unit);
        }

        public void Update()
        {
            bool done = false;
            while (!done && _schedule.Count > 0)
            {
                foreach ((ISchedulable entity, int value) in _schedule.ToList())
                {
                    int timeTilAct = value - 1;
                    if (timeTilAct <= 0)
                    {
                        _schedule[entity] = entity.Speed;
                        if (entity is Player player)
                        {
                            Turn++;
                            done = true;
                        }
                        else
                        {
                            ExecuteCommand(entity.Id, entity.Act(), () => { });
                        }
                    }
                    else
                    {
                        _schedule[entity] = timeTilAct;
                    }
                }
            }
        }

        internal static void ExecuteCommand(int sourceId, Option<ICommand> action, Action after)
        {
            action.MatchSome(command =>
            {
                var retry = command.Execute();
                var animation = command.Animation;

                while (retry.HasValue)
                {
                    retry.MatchSome(c =>
                    {
                        retry = c.Execute();
                        animation = c.Animation;
                    });
                }

                animation.MatchSome(anim => Game.Animations.Add(sourceId, anim));
                after();
            });
        }
    }
}
