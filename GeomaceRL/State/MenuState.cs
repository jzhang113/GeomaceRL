using BearLib;
using GeomaceRL.Command;
using GeomaceRL.UI;
using Optional;
using System;
using System.Drawing;

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
                case Terminal.TK_KP_ENTER:
                    Game.NewGame();
                    return Option.None<ICommand>();
                case Terminal.TK_ESCAPE:
                    Game.Exit();
                    return Option.None<ICommand>();
                default:
                    return Option.None<ICommand>();
            }
        }

        public Option<ICommand> HandleMouseMove(int x, int y) => Option.None<ICommand>();

        public void Draw(LayerInfo layer)
        {
            Terminal.Clear();
            layer.Print(2, "GeomanceRL");

            string message = @"
Several days ago, you sensed a disturbance somewhere
in the distance. With your trusty staff and spellbook
in hand, you set out to investigate.

Controls:
Vi-keys, arrow keys, or number pad to move
1-6 to cast known spells
While casting, press [[Enter]] to confirm or [[Esc]] to cancer
[[Esc]] to quit to this menu

Press [[Enter]] to start";

            int startY = 4;
            layer.Print(new Rectangle(0, startY, layer.Width, layer.Height - startY), message, ContentAlignment.TopLeft);
        }
    }
}
