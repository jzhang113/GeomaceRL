using BearLib;
using System.Collections.Generic;

namespace GeomaceRL.Input
{
    internal static partial class InputMapping
    {
        private static readonly KeyMap _keyMap;

        static InputMapping()
        {
            _keyMap = new KeyMap()
            {
                NormalMap = new KeyMap.StateMap<NormalInput>()
                {
                    None = new Dictionary<int, NormalInput>()
                    {
                        [Terminal.TK_LEFT] =        NormalInput.MoveW,
                        [Terminal.TK_KP_4] =        NormalInput.MoveW,
                        [Terminal.TK_H] =           NormalInput.MoveW,
                        [Terminal.TK_DOWN] =        NormalInput.MoveS,
                        [Terminal.TK_KP_2] =        NormalInput.MoveS,
                        [Terminal.TK_J] =           NormalInput.MoveS,
                        [Terminal.TK_UP] =          NormalInput.MoveN,
                        [Terminal.TK_KP_8] =        NormalInput.MoveN,
                        [Terminal.TK_K] =           NormalInput.MoveN,
                        [Terminal.TK_RIGHT] =       NormalInput.MoveE,
                        [Terminal.TK_KP_6] =        NormalInput.MoveE,
                        [Terminal.TK_L] =           NormalInput.MoveE,
                        [Terminal.TK_KP_7] =        NormalInput.MoveNW,
                        [Terminal.TK_Y] =           NormalInput.MoveNW,
                        [Terminal.TK_KP_9] =        NormalInput.MoveNE,
                        [Terminal.TK_U] =           NormalInput.MoveNE,
                        [Terminal.TK_KP_1] =        NormalInput.MoveSW,
                        [Terminal.TK_B] =           NormalInput.MoveSW,
                        [Terminal.TK_KP_3] =        NormalInput.MoveSE,
                        [Terminal.TK_N] =           NormalInput.MoveSE,
                        [Terminal.TK_KP_5] =        NormalInput.Wait,
                        [Terminal.TK_PERIOD] =      NormalInput.Wait,
                        [Terminal.TK_COMMA] =       NormalInput.Get,
                        [Terminal.TK_BACKSLASH] =   NormalInput.ChangeLevel,
                        [Terminal.TK_A] =           NormalInput.OpenApply,
                        [Terminal.TK_D] =           NormalInput.OpenDrop,
                        [Terminal.TK_E] =           NormalInput.OpenEquip,
                        [Terminal.TK_I] =           NormalInput.OpenInventory,
                        [Terminal.TK_R] =           NormalInput.OpenUnequip,
                        [Terminal.TK_T] =           NormalInput.Throw,
                        [Terminal.TK_O] =           NormalInput.AutoExplore,
                        [Terminal.TK_ESCAPE] =      NormalInput.OpenMenu
                    },
                    Shift = new Dictionary<int, NormalInput>()
                    {
                        [Terminal.TK_LEFT] =        NormalInput.AttackW,
                        [Terminal.TK_KP_4] =        NormalInput.AttackW,
                        [Terminal.TK_H] =           NormalInput.AttackW,
                        [Terminal.TK_DOWN] =        NormalInput.AttackS,
                        [Terminal.TK_KP_2] =        NormalInput.AttackS,
                        [Terminal.TK_J] =           NormalInput.AttackS,
                        [Terminal.TK_UP] =          NormalInput.AttackN,
                        [Terminal.TK_KP_8] =        NormalInput.AttackN,
                        [Terminal.TK_K] =           NormalInput.AttackN,
                        [Terminal.TK_RIGHT] =       NormalInput.AttackE,
                        [Terminal.TK_KP_6] =        NormalInput.AttackE,
                        [Terminal.TK_L] =           NormalInput.AttackE,
                        [Terminal.TK_KP_7] =        NormalInput.AttackNW,
                        [Terminal.TK_Y] =           NormalInput.AttackNW,
                        [Terminal.TK_KP_9] =        NormalInput.AttackNE,
                        [Terminal.TK_U] =           NormalInput.AttackNE,
                        [Terminal.TK_KP_1] =        NormalInput.AttackSW,
                        [Terminal.TK_B] =           NormalInput.AttackSW,
                        [Terminal.TK_KP_3] =        NormalInput.AttackSE,
                        [Terminal.TK_N] =           NormalInput.AttackSE
                    }
                }
            };
        }
    }
}
