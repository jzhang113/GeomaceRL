using GeomaceRL.Actor;
using GeomaceRL.Command;
using GeomaceRL.Interface;
using Optional;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL
{
    public class EventScheduler
    {
        public IDictionary<ISchedulable, int> schedule;
        private bool _clearing;

        public static int Turn { get; private set; }

        public EventScheduler()
        {
            Turn = 0;
            _clearing = false;
            schedule = new Dictionary<ISchedulable, int>();
        }

        public void Clear()
        {
            _clearing = true;
        }

        internal void RemoveActor(Actor.Actor unit)
        {
            schedule.Add(unit, unit.Speed);
        }

        internal void AddActor(Actor.Actor unit)
        {
            schedule.Remove(unit);
        }

        public void Update()
        {
            foreach ((ISchedulable entity, int value) in schedule.ToList())
            {
                int timeTilAct = value - 1;
                if (timeTilAct < 0)
                {
                    schedule[entity] = entity.Speed;
                    entity.Act().MatchSome(command =>
                    {
                        Option<ICommand> retry;

                        do
                        {
                            retry = command.Execute();
                        } while (retry.HasValue);

                        if (entity is Player)
                        {
                            Turn++;
                        }
                    });
                }
                else
                {
                    schedule[entity] = timeTilAct;
                }
            }

            if (_clearing)
            {
                schedule.Clear();
                _clearing = false;
            }
        }
    }
}
