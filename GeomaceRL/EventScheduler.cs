using GeomaceRL.Actor;
using GeomaceRL.Interface;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL
{
    public class EventScheduler
    {
        private readonly IDictionary<ISchedulable, int> _schedule;
        private bool _clearing;

        public static int Turn { get; private set; }

        public EventScheduler()
        {
            Turn = 0;
            _clearing = false;
            _schedule = new Dictionary<ISchedulable, int>();
        }

        public void Clear()
        {
            _clearing = true;
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
            while (!done)
            {
                foreach ((ISchedulable entity, int value) in _schedule.ToList())
                {
                    int timeTilAct = value - 1;
                    if (timeTilAct <= 0)
                    {
                        _schedule[entity] = entity.Speed;
                        if (entity is Player)
                        {
                            Turn++;
                            done = true;
                        }
                        else
                        {
                            entity.Act().MatchSome(command =>
                            {
                                var retry = command.Execute();
                                while (retry.HasValue)
                                {
                                    retry.MatchSome(c => retry = c.Execute());
                                }
                            });
                        }
                    }
                    else
                    {
                        _schedule[entity] = timeTilAct;
                    }
                }

                if (_clearing)
                {
                    _schedule.Clear();
                    _clearing = false;
                }
            }
        }
    }
}
