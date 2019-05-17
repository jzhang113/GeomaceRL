using GeomaceRL.Actor;
using GeomaceRL.Command;
using GeomaceRL.Input;
using GeomaceRL.UI;
using Optional;
using System;
using System.Linq;

namespace GeomaceRL.State
{
    internal sealed class NormalState : IState
    {
        private static readonly Lazy<NormalState> _instance = new Lazy<NormalState>(() => new NormalState());
        public static NormalState Instance => _instance.Value;

        private NormalState()
        {
        }

        // ReSharper disable once CyclomaticComplexity
        public Option<ICommand> HandleKeyInput(int key)
        {
            Player player = Game.Player;

            if (Game._dead)
            {
                Game.StateHandler.Reset();
                return Option.None<ICommand>();
            }

            switch (InputMapping.GetNormalInput(key))
            {
                case NormalInput.None:
                    return Option.None<ICommand>();

                #region Movement Keys
                case NormalInput.MoveW:
                    return Option.Some<ICommand>(new MoveCommand(player, player.Pos + Direction.W));
                case NormalInput.MoveS:
                    return Option.Some<ICommand>(new MoveCommand(player, player.Pos + Direction.S));
                case NormalInput.MoveN:
                    return Option.Some<ICommand>(new MoveCommand(player, player.Pos + Direction.N));
                case NormalInput.MoveE:
                    return Option.Some<ICommand>(new MoveCommand(player, player.Pos + Direction.E));
                case NormalInput.MoveNW:
                    return Option.Some<ICommand>(new MoveCommand(player, player.Pos + Direction.NW));
                case NormalInput.MoveNE:
                    return Option.Some<ICommand>(new MoveCommand(player, player.Pos + Direction.NE));
                case NormalInput.MoveSW:
                    return Option.Some<ICommand>(new MoveCommand(player, player.Pos + Direction.SW));
                case NormalInput.MoveSE:
                    return Option.Some<ICommand>(new MoveCommand(player, player.Pos + Direction.SE));
                case NormalInput.Wait:
                    return Option.Some<ICommand>(new WaitCommand(player));
                #endregion

                case NormalInput.Cast:
                    int spellnum = key - BearLib.Terminal.TK_1;
                    if (spellnum >= player.SpellList.Count)
                        return Option.None<ICommand>();

                    Spell.ISpell spell = player.SpellList[spellnum];
                    int mainMana = spell.Cost.MainManaUsed();
                    int altMana = spell.Cost.AltManaUsed();

                    if (mainMana == -1 || altMana == -1)
                    {
                        Game.MessagePanel.AddMessage("Not enough mana");
                        return Option.None<ICommand>();
                    }
                    else
                    {
                        Game.StateHandler.PushState(
                            new TargettingState(player, spell.Zone, spellnum, targets =>
                            {
                                if (!targets.Any())
                                    return Option.None<ICommand>();

                                Game.MessagePanel.AddMessage($"{player.Name} casts {spell.Name}");
                                Game.MapHandler.UpdateAllMana(player.Pos, spell.Cost.MainElem, mainMana);
                                spell.Cost.AltElem.MatchSome(altElem =>
                                    Game.MapHandler.UpdateAllMana(player.Pos, altElem, altMana));
                                Game.StateHandler.PopState();
                                return Option.Some(spell.Evoke(player, targets, (mainMana, altMana)));
                            }));
                        return Option.None<ICommand>();
                    }
                case NormalInput.Get:
                    return Game.MapHandler.GetItem(player.Pos)
                        .FlatMap(item => Option.Some<ICommand>(new PickupCommand(player, item)));
                case NormalInput.OpenMenu:
                    Game.Exit();
                    return Option.None<ICommand>();
            }

            return Option.None<ICommand>();
        }

        public void Draw(LayerInfo layer) => Game.MapHandler.Draw(layer);
    }
}
