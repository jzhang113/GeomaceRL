using BearLib;

namespace GeomaceRL.Input
{
    internal enum TargettingInput
    {
        None,
        JumpE,
        JumpS,
        JumpN,
        JumpW,
        JumpNW,
        JumpNE,
        JumpSW,
        JumpSE,
        MoveE,
        MoveS,
        MoveN,
        MoveW,
        MoveNW,
        MoveNE,
        MoveSW,
        MoveSE,
        NextActor,
        Fire
    }

    internal static partial class InputMapping
    {
        public static TargettingInput GetTargettingInput(int key)
        {
            if (Terminal.Check(Terminal.TK_SHIFT))
            {
                if (_keyMap.TargettingMap.Shift.TryGetValue(key, out TargettingInput action))
                    return action;
            }
            else if (_keyMap.TargettingMap.None.TryGetValue(key, out TargettingInput action))
            {
                return action;
            }

            return TargettingInput.None;
        }
    }
}
