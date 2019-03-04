using GeomaceRL.Actor;
using GeomaceRL.Command;
using GeomaceRL.Input;
using GeomaceRL.UI;
using Optional;
using System;

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

            switch (InputMapping.GetNormalInput(key))
            {
                //case NormalInput.None:
                //    return null;

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
                    var spell = player.SpellList[0];
                    (Element costElem, int costAmount) = spell.Cost;

                    if (player.Mana[costElem] < costAmount)
                    {
                        Game.MessagePanel.AddMessage("Insufficient mana!");
                        return Option.None<ICommand>();
                    }
                    else
                    {
                        // take mana from the current square if possible
                        int remaining = Game.MapHandler.UpdateMana(player.Pos, costElem, costAmount);

                        // take mana from surrounding squares
                        foreach (Loc nearby in Game.MapHandler.GetPointsInRadius(player.Pos, 1))
                        {
                            if (remaining <= 0)
                                break;

                            remaining = Game.MapHandler.UpdateMana(nearby, costElem, remaining);
                        }

                        return Option.Some(spell.Evoke(player, player.Pos - (1, 1)));
                    }

                //case NormalInput.ChangeLevel:
                //    return Option.Some<ICommand>(new ChangeLevelCommand(Game.MapHelper.TryChangeLocation(player)));
                //case NormalInput.OpenApply:
                //    Game.StateHandler.PushState(ApplyState.Instance);
                //    return Option.None<ICommand>();
                //case NormalInput.OpenDrop:
                //    Game.StateHandler.PushState(DropState.Instance);
                //    return Option.None<ICommand>();
                //case NormalInput.OpenEquip:
                //    Game.StateHandler.PushState(EquipState.Instance);
                //    return Option.None<ICommand>();
                //case NormalInput.OpenInventory:
                //    Game.StateHandler.PushState(InventoryState.Instance);
                //    return Option.None<ICommand>();
                //case NormalInput.OpenUnequip:
                //    Game.StateHandler.PushState(UnequipState.Instance);
                //    return Option.None<ICommand>();
                //case NormalInput.AutoExplore:
                //    Game.StateHandler.PushState(AutoexploreState.Instance);
                //    return Option.None<ICommand>();
                case NormalInput.OpenMenu:
                    Game.Exit();
                    return Option.None<ICommand>();
            }

            return Option.None<ICommand>();
        }

        public void Draw(LayerInfo layer)
        {
            Game.MapHandler.Draw(layer);
        }
    }
}
