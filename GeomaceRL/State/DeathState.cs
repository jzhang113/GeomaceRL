using BearLib;
using GeomaceRL.Command;
using GeomaceRL.UI;
using Optional;
using System;
using System.Drawing;
using System.Text;

namespace GeomaceRL.State
{
    internal sealed class DeathState : IState
    {
        private static readonly Lazy<DeathState> _instance = new Lazy<DeathState>(() => new DeathState());
        public static DeathState Instance => _instance.Value;

        private DeathState()
        {
        }

        public Option<ICommand> HandleKeyInput(int key)
        {
            Game.StateHandler.PopState();
            return Option.None<ICommand>();
        }

        public Option<ICommand> HandleMouseMove(int x, int y) => Option.None<ICommand>();

        public void Draw(LayerInfo layer)
        {
            Terminal.Clear();

            var message = new StringBuilder($@"
Farewell great sage

You made it to level {Game.MapHandler.Level + 1}
You knew {Game.Player.SpellsKnown} spells
You killed:");

            message.Append("\n");
            int typeCount = 0;

            foreach ((Type type, int count) in Game.Player.KillCount)
            {
                if (count > 0)
                {
                    message.Append($"\t{count} {Game.ActorData[type].Name}\n");
                    typeCount++;
                }
            }

            if (typeCount == 0)
            {
                message.Append($"\tNothing!\n");
            }

            message.Append("\nPress any key to continue");

            int startY = 2;
            layer.Print(new Rectangle(0, startY, layer.Width, layer.Height - startY), message.ToString());
        }
    }
}
