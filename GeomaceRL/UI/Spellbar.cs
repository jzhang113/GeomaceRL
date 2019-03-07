using BearLib;
using GeomaceRL.Spell;
using System.Drawing;

namespace GeomaceRL.UI
{
    internal static class Spellbar
    {
        public static void Draw(LayerInfo layer)
        {
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo
            {
                TopLeftChar = '├',
                TopRightChar = '┤',
                BottomLeftChar = '┴',
                BottomRightChar = '┘',
                TopChar = '─',
                BottomChar = '─',
                LeftChar = '│',
                RightChar = '│'
            });

            const int boxWidth = 5;
            int x = 0;
            foreach (ISpell spell in Game.Player.SpellList)
            {
                int startX = boxWidth * x;
                int endX = boxWidth * (x + 1) - 1;

                Terminal.Color(Colors.Text);
                layer.Print(
                    new Rectangle(startX, 0, 4, 1),
                    $"{(char)(x + '1')} {spell.Abbrev}",
                    ContentAlignment.TopLeft);
                layer.Print(
                    new Rectangle(startX, 1, 4, 1),
                    FormatCost(spell.Cost),
                    ContentAlignment.TopLeft);

                Terminal.Color(Colors.BorderColor);
                layer.Put(endX, -1, '╥');
                layer.Put(endX, 0, '║');
                layer.Put(endX, 1, '║');
                layer.Put(endX, 2, '╨');
                x++;
            }
        }

        private static string FormatCost((Element elem, int amount) cost)
        {
            Terminal.Color(cost.elem.Color());
            return $"{cost.elem.Abbrev()}{cost.amount}";
        }
    }
}
