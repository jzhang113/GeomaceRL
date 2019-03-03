using BearLib;

namespace GeomaceRL.Input
{
    internal enum NormalInput
    {
        None,
        AttackE,
        AttackS,
        AttackN,
        AttackW,
        AttackNW,
        AttackNE,
        AttackSW,
        AttackSE,
        MoveE,
        MoveS,
        MoveN,
        MoveW,
        MoveNW,
        MoveNE,
        MoveSW,
        MoveSE,
        OpenApply,
        OpenDrop,
        OpenEquip,
        OpenInventory,
        OpenUnequip,
        OpenMenu,
        AutoExplore,
        ChangeLevel,
        Get,
        Throw,
        Wait
    }

    internal static partial class InputMapping
    {
        public static NormalInput GetNormalInput(int key)
        {
            if (Terminal.Check(Terminal.TK_SHIFT))
            {
                if (_keyMap.NormalMap.Shift.TryGetValue(key, out NormalInput action))
                    return action;
            }
            else if (_keyMap.NormalMap.None.TryGetValue(key, out NormalInput action))
            {
                return action;
            }

            return NormalInput.None;
        }
    }
}
