using GeomaceRL.Animation;
using GeomaceRL.Interface;
using Optional;

namespace GeomaceRL.Command
{
    // Skip this turn
    internal class WaitCommand : ICommand
    {
        public ISchedulable Source { get; }
        public Option<IAnimation> Animation => Option.None<IAnimation>();

        public WaitCommand(ISchedulable source)
        {
            Source = source;
        }

        public Option<ICommand> Execute()
        {
            return Option.None<ICommand>();
        }
    }
}
