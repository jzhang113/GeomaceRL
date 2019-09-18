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

            const int boxWidth = 7;
            const int halfBox = 3;
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
                        new Rectangle(startX, 0, boxWidth - 1, 1),
                        $"{(char)(x + '1')} {spell.Abbrev}",
                        ContentAlignment.TopLeft);
                    layer.Print(
                        new Rectangle(startX, 1, halfBox + 1, 1),
                        spell.Cost.GetMainString(),
                        ContentAlignment.TopLeft);
                    layer.Print(
                        new Rectangle(startX + halfBox, 1, halfBox, 1),
                        spell.Cost.GetAltString(),
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

            // Display more spell info when player mouses over the spellbar
            int mouseX = Terminal.State(Terminal.TK_MOUSE_X);
            int mouseY = Terminal.State(Terminal.TK_MOUSE_Y);

            if (!layer.PointInside(mouseX, mouseY))
                return;

            int adjX = mouseX - layer.X;
            int currentBox = adjX / boxWidth;

            if (currentBox >= Game.Player.SpellList.Count)
                return;

            layer.Print(new Rectangle(adjX, -1, 10, 1), Game.Player.SpellList[currentBox].Name, ContentAlignment.TopCenter);
        }
    }
}
