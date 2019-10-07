using BearLib;
using GeomaceRL.Command;
using GeomaceRL.Spell;
using GeomaceRL.UI;
using Optional;

namespace GeomaceRL.State
{
    class SpellReplaceState : IState
    {
        private ISpell _newSpell;

        public SpellReplaceState(ISpell spell)
        {
            _newSpell = spell;
        }

        public Option<ICommand> HandleKeyInput(int key)
        {
            // TODO: Y/*
            Game.MessagePanel.AddMessage("Do you wish to replace a spell? (Y/N/*)");

            if (key == Terminal.TK_N)
            {
                Game.MessagePanel.AddMessage("Nevermind");
                Game.StateHandler.PopState();
            }
            else
            {
                int slot = key - 0x1E;

                if (slot >= 0 && slot < Constants.MAX_SPELLS)
                {
                    Game.MessagePanel.AddMessage("You learned a new spell!");
                    Game.Player.SpellList[slot] = _newSpell;
                    Game.StateHandler.PopState();
                }
            }

            return Option.None<ICommand>();
        }

        public Option<ICommand> HandleMouseMove(int x, int y) => Option.None<ICommand>();

        public void Draw(LayerInfo layer) => Game.MapHandler.Draw(layer);
    }
}
