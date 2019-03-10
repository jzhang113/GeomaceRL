using BearLib;
using GeomaceRL.Command;
using GeomaceRL.UI;
using Optional;
using System;

namespace GeomaceRL.State
{
    internal sealed class MenuState : IState
    {
        private static readonly Lazy<MenuState> _instance = new Lazy<MenuState>(() => new MenuState());
        public static MenuState Instance => _instance.Value;

        private MenuState()
        {
        }

        public Option<ICommand> HandleKeyInput(int key)
        {
            switch (key)
            {
                case Terminal.TK_ENTER:
                    Game.NewGame();
                    return Option.None<ICommand>();
                case Terminal.TK_Q:
                    Game.Exit();
                    return Option.None<ICommand>();
                default:
                    return Option.None<ICommand>();
            }
        }

        public void Draw(LayerInfo layer)
        {
            Terminal.Clear();
            layer.Print(2, "GeomanceRL");

            int y = 4;
            layer.Print(y++, "You sense a disturbance somewhere in the distance");
            layer.Print(y++, "With your trusty staff and spellbook in hand, you set");
            layer.Print(y++, "out to investigate.");

            y++;
            layer.Print(y++, "Controls:");
            layer.Print(y++, "Vi-keys, arrow keys, or number pad to move");
            layer.Print(y++, "1-6 to cast known spells");
            layer.Print(y++, "Enter to go down stairs");
            layer.Print(y++, "Esc to quit to this menu");

            layer.Print(++y, "Press Enter to start");
        }
    }
}
