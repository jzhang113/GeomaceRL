using BearLib;
using GeomaceRL.Spell;
using GeomaceRL.State;
using System.Drawing;

namespace GeomaceRL.UI
{
    internal static class Spellbar
    {
        public static void Draw(LayerInfo layer)
        {
            // draw borders, right border will be fixed at the end
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo
            {
                TopLeftChar = '├',
                BottomLeftChar = '┴',
                TopChar = '─',
                BottomChar = '─',
                LeftChar = '│'
            });

            const int boxWidth = 5;
            int casting = -1;
            Game.StateHandler.Peek().MatchSome(state =>
            {
                if (state is TargettingState targetting)
                    casting = targetting.CurrentSpell;
            });

            for (int x = 0; x < 6; x++)
            {
                int startX = boxWidth * x;
                int endX = boxWidth * (x + 1) - 1;

                if (x < Game.Player.SpellList.Count)
                {
                    ISpell spell = Game.Player.SpellList[x];

                    Terminal.Color(casting == x ? Colors.HighlightColor : Colors.Text);
                    layer.Print(
                        new Rectangle(startX, 0, 4, 1),
                        $"{(char)(x + '1')} {spell.Abbrev}",
                        ContentAlignment.TopLeft);
                    layer.Print(
                        new Rectangle(startX, 1, 4, 1),
                        FormatCost(spell.Cost),
                        ContentAlignment.TopLeft);
                }
                else
                {
                    Terminal.Color(Colors.Text);
                    Terminal.Font("text");
                    layer.Put(startX, 0, x + '1');
                    Terminal.Font("");
                }

                Terminal.Color(Colors.BorderColor);
                layer.Put(endX, -1, '╥');
                layer.Put(endX, 0, '║');
                layer.Put(endX, 1, '║');
                layer.Put(endX, 2, '╨');
            }

            // fix right border
            layer.Put(layer.Width, -1, '┤');
            layer.Put(layer.Width, 0, '│');
            layer.Put(layer.Width, 1, '│');
            layer.Put(layer.Width, 2, '┘');
        }

        private static string FormatCost((Element elem, int amount) cost)
        {
            Terminal.Color(cost.elem.Color());
            return $"{cost.elem.Abbrev()}{cost.amount}";
        }
    }
}
