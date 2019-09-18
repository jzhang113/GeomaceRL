using GeomaceRL.Command;
using GeomaceRL.UI;
using Optional;

namespace GeomaceRL.State
{
    public interface IState
    {
        // Handle keyboard inputs.
        Option<ICommand> HandleKeyInput(int key);

        // Handle mouse movement. x and y are the new cursor position in cells.
        Option<ICommand> HandleMouseMove(int x, int y);

        // Update state information.
        // void Update(ICommand command);

        // Draw to the screen.
        void Draw(LayerInfo layer);
    }
}
