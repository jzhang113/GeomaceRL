using GeomaceRL.Command;
using Optional;

namespace GeomaceRL.Interface
{
    public interface ISchedulable
    {
        int Id { get; }
        string Name { get; }
        int Speed { get; }

        Option<ICommand> Act();
    }
}
