using GeomaceRL.Command;
using GeomaceRL.UI;
using Optional;

namespace GeomaceRL.State
{
    public interface IState
    {
        // Handle keyboard inputs.
        Option<ICommand> HandleKeyInput(int key);

        // Update state information.
        // void Update(ICommand command);

        // Draw to the screen.
        void Draw(LayerInfo layer);
    }
}
