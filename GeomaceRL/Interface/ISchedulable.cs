using GeomaceRL.Command;
using Optional;

namespace GeomaceRL.Interface
{
    public interface ISchedulable
    {
        string Name { get; }
        int Speed { get; }

        Option<ICommand> Act();
    }
}
