using BearLib;
using GeomaceRL.Spell;
using GeomaceRL.State;
using System.Drawing;

namespace GeomaceRL.UI
{
    internal static class Spellbar
    {
        private const char _manaChar = 'o';
        private const char _extraChar = '.';

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

            const int boxWidth = 7;
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
                    (int minMainCost, int maxMainCost) = spell.Cost.MainCost;
                    int minAltCost = spell.Cost.AltCost.Item1;

                    Terminal.Color(casting == x ? Colors.HighlightColor : Colors.Text);
                    string firstLine = $"{(char)(x + '1')} {spell.Abbrev}";
                    firstLine += spell.Abbrev.Length == 1 ? "  " : " ";
                    firstLine += spell.Charges > 0 ? spell.Charges.ToString() : "-";

                    layer.Print(
                        new Rectangle(startX, 0, boxWidth - 1, 1),
                        firstLine,
                        ContentAlignment.TopLeft);

                    DisplayManaCost(layer, startX, 1, spell.Cost.MainElem.Color(), spell.Cost.MainCost, spell.Cost.MainManaUsed());
                    DisplayManaCost(layer, startX, 2, spell.Cost.AltElem.Color(), spell.Cost.AltCost, spell.Cost.AltManaUsed());
                }
                else
                {
                    Terminal.Color(Colors.Text);
                    Terminal.Font("text");
                    layer.Put(startX, 0, x + '1');
                    Terminal.Font("");
                }

                Terminal.Color(Colors.BorderColor);
                for (int y = 0; y < layer.Height; y++) layer.Put(endX, y, '║');
                layer.Put(endX, -1, '╥');
                layer.Put(endX, layer.Height, '╨');
            }

            // fix right border
            for (int y = 0; y < layer.Height; y++)
                layer.Put(layer.Width, y, '│');
            layer.Put(layer.Width, -1, '┤');
            layer.Put(layer.Width, layer.Height, '┘');
        }

        private static void DisplayManaCost(LayerInfo layer, int startX, int startY, Color color, (int Min, int Max) cost, int used)
        {
            const double alpha = 0.7;

            // minimum casting cost
            Terminal.Color(color.Blend(Colors.Background, alpha));
            for (int dx = 0; dx < cost.Min; dx++)
                layer.Put(startX + dx, startY, _manaChar);

            if (cost.Max > cost.Min)
            {
                // variable casting cost
                Terminal.Color(color.Blend(Colors.Background, alpha));
                for (int dx = cost.Min; dx < cost.Max; dx++)
                    layer.Put(startX + dx, startY, _extraChar);
            }

            // actual current cost
            Terminal.Color(color);
            for (int dx = 0; dx < used; dx++)
                layer.Put(startX + dx, startY, _manaChar);
        }
    }
}
